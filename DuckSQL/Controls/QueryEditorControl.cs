using DuckDB.NET.Data;
using DuckSQL.Exceptions;
using DuckSQL.Helpers;
using DuckSQL.Types;
using FastColoredTextBoxNS;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;

namespace DuckSQL.Controls
{
    internal partial class QueryEditorControl : UserControl
    {
        private const string QUERY_FORMAT =
@"SELECT {0}
FROM {1}
LIMIT {2}
OFFSET {3} ";

        private Pen _splitterColor = new(Brushes.DarkGray, 2) { DashStyle = System.Drawing.Drawing2D.DashStyle.Solid };
        private static bool _wasByteArrayConversionErrorShown = false; //TODO: This should ideally be kept per tab/instance
        private CancellationTokenSource? _queryCancellationToken = null;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public EventHandler? QueryTextChanged { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public EventHandler? QueryExecutionStarted { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public EventHandler<QueryExecutedEventArgs>? QueryExecuted { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string QueryText
        {
            get => this.queryRichTextBox.Text;
            set => this.queryRichTextBox.Text = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? CurrentFilePath { get; set; }

        public QueryEditorControl()
        {
            InitializeComponent();
            mainSplitContainer.Paint += MainSplitContainer_Paint;
            resultsGridView.ShowCopyAsWhereContextMenuItem = true;
            resultsGridView.CopyToClipboardIcon = Properties.Resources.copy_clipboard_icon.ToBitmap();
            resultsGridView.ColumnNameEscapeFormat = "\"{0}\"";
            resultsGridView.DateValueEscapeFormat = "'{0}'";
            resultsGridView.DragDrop += (s, args) =>
            {
                OnDragDrop(args);
            };
            queryRichTextBox.AllowDrop = true;
            queryRichTextBox.DragEnter += (s, args) =>
            {
                OnDragEnter(args);
            };
            queryRichTextBox.DragDrop += (s, args) =>
            {
                OnDragDrop(args);
            };
            AllowDrop = true;
        }

        public QueryEditorControl(string filePath, int offset = 0, int limit = 1000) : this()
        {
            if (Path.GetExtension(filePath).Equals(".sql"))
            {
                var fileContent = File.ReadAllText(filePath);
                QueryText = fileContent;
            }
            else
            {
                var queryFields = "*";
                var fromClause = $"'{filePath}'";
                QueryText = QUERY_FORMAT.Format(queryFields, fromClause, limit, offset);
            }
            executeQueryButton.Enabled = true;
        }

        private void QueryEditorControl_Load(object sender, EventArgs e)
        {
            RefreshControl();
        }

        public void RefreshControl()
        {
            SetZoom(AppSettings.QueryEditorZoomLevel ?? 100);
            resultsGridView.UpdateDateFormats();
            resultsGridView.Refresh();
        }

        public DataTable GetDataSource() => this.resultsGridView.DataSource as DataTable ?? new DataTable();

        private async Task<DataTable> ExecuteQueryAsync(string query, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(query);

            QueryExecutionStarted?.Invoke(this, EventArgs.Empty);

            var result = new DataTable();
            try
            {
                await Task.Run(() =>
                {
                    using var connection = new DuckDBConnection("Data Source=:memory:");
                    connection.Open();

                    using var command = connection.CreateCommand();
                    command.CommandText = query;

                    using var reader = command.ExecuteReader();
                    result.Load(reader);

                    cancellationToken.ThrowIfCancellationRequested();
                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return result;
            }
            catch (Exception ex)
            {
                result.Dispose();

                if (ex is InvalidCastException && ex.Message.Contains("The list contains null value"))
                {
                    throw new QueryExecuteException(Resources.Errors.ListsWithNullsErrorTitle, Resources.Errors.ListsWithNullsErrorMessage) { Handled = true };
                }
                else if (ex is DuckDBException && ex.Message.StartsWith("Parser Error:"))
                {
                    throw new QueryExecuteException(Resources.Errors.InvalidQueryErrorTitle,
                        $"{Resources.Errors.InvalidQueryErrorMessage}{Env.DoubleNewLine}{ex.Message}")
                    { Handled = true };
                }
                else if (ex is OverflowException && ex.Message.Contains("Value was either too large or too small for a Decimal"))
                {
                    throw new QueryExecuteException(Resources.Errors.DecimalValueTooLargeErrorTitle,
                        Resources.Errors.DecimalValueUnknownSizeTooLargeErrorMessageFormat
                        .Format(DecimalOverflowException.MAX_DECIMAL_PRECISION, DecimalOverflowException.MAX_DECIMAL_SCALE))
                    { Handled = true };
                }
                else
                {
                    throw new QueryExecuteException(Resources.Errors.QueryExecutionErrorTitle, ex.Message);
                }
            }

            return result;
        }

        private async void executeQueryButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.queryRichTextBox.Text))
                return;

            //Cleanup previous results
            if (this.resultsGridView.DataSource is DataTable dt)
            {
                dt.Dispose();
                this.resultsGridView.DataSource = null;
            }

            _queryCancellationToken = new CancellationTokenSource();
            Cursor = Cursors.WaitCursor;
            queryExecutionStatusLabel.Text = Resources.Strings.QueryRunningStatusText;
            timeElapsedLabel.Text = "00:00";
            showingCountLabel.Text = "0";
            executeQueryButton.Enabled = false;
            resultsGridView.Enabled = false;
            cancelQueryButton.Enabled = true;

            var stopwatch = Stopwatch.StartNew();
            var queryTask = ExecuteQueryAsync(this.queryRichTextBox.Text, _queryCancellationToken.Token);

            while (!queryTask.IsCompleted && !_queryCancellationToken.IsCancellationRequested)
            {
                await Task.Delay(50);
                this.timeElapsedLabel.Text = stopwatch.Elapsed.ToString("mm\\:ss");
            }

            stopwatch.Stop();
            var queryDuration = stopwatch.Elapsed;
            var result = new DataTable();
            try
            {
                if (_queryCancellationToken.IsCancellationRequested)
                {
                    return;
                }

                result = await queryTask;

                stopwatch.Restart();
                try
                {
                    //We need to convert complex types like: lists, structs, maps, and byte arrays so we can render them properly
                    result = ConvertValues(result);
                }
                catch (Exception ex)
                {
                    throw new QueryExecuteException(Resources.Errors.RenderResultsErrorTitle, $"{ex.Message}{Env.DoubleNewLine}{ex.StackTrace}");
                }
                stopwatch.Stop();
                var conversionDuration = stopwatch.Elapsed;

                stopwatch.Restart();
                this.resultsGridView.DataSource = result;
                stopwatch.Stop();
                var renderDuration = stopwatch.Elapsed;

                QueryExecuted?.Invoke(this, new QueryExecutedEventArgs(queryDuration, conversionDuration, renderDuration));
            }
            catch (QueryExecuteException ex)
            {
                if (ex.Handled)
                {
                    //Don't throw/log errors we can't do anything about
                    MessageBox.Show(ex.Message, ex.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                queryExecutionStatusLabel.Text = _queryCancellationToken.IsCancellationRequested ?
                    Resources.Strings.QueryCancelledStatusText : Resources.Strings.QueryFinishedStatusText;
                timeElapsedLabel.Text = stopwatch.Elapsed.ToString("mm\\:ss");
                showingCountLabel.Text = result.Rows.Count.ToString();

                executeQueryButton.Enabled = true;
                resultsGridView.Enabled = true;
                cancelQueryButton.Enabled = false;
                Cursor = Cursors.Default;
            }
        }

        private void queryRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.queryRichTextBox.Text))
                executeQueryButton.Enabled = true;

            QueryTextChanged?.Invoke(this, EventArgs.Empty);
        }

        private void queryRichTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift && e.KeyCode == Keys.Enter || e.KeyCode == Keys.F5)
            {
                executeQueryButton.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                _queryCancellationToken?.Cancel();
                e.Handled = true;
            }
        }

        private void copyTextMenuItem_Click(object sender, EventArgs e)
        {
            this.queryRichTextBox.Copy();
        }

        private void pasteTextMenuItem_Click(object sender, EventArgs e)
        {
            this.queryRichTextBox.Paste();
        }

        private void MainSplitContainer_Paint(object? sender, PaintEventArgs e)
        {
            //Draw a dashed splitter so it's easier to see and grab.
            if (sender is SplitContainer splitContainer)
            {
                var rectangle = splitContainer.SplitterRectangle;
                rectangle.Offset(0, 2);
                e.Graphics.DrawLine(_splitterColor, rectangle.X, rectangle.Y,
                    rectangle.X + rectangle.Width, rectangle.Y);
            }
        }

        private static DataTable ConvertValues(DataTable intermediateResult)
        {
            var hasComplexType = false;
            foreach (DataColumn column in intermediateResult.Columns)
            {
                if (column.DataType.ImplementsInterface<IDictionary>()
                    || column.DataType.ImplementsInterface<IList>()
                    || column.DataType == typeof(Stream))
                {
                    hasComplexType = true;
                    break;
                }
            }

            if (!hasComplexType)
            {
                return intermediateResult;
            }

            var result = new DataTable();
            foreach (DataColumn column in intermediateResult.Columns)
            {
                if (column.DataType == typeof(Dictionary<string, object?>))
                {
                    result.Columns.Add(new DataColumn(column.ColumnName, typeof(StructValue)));
                }
                else if (column.DataType.ImplementsInterface<IList>())
                {
                    result.Columns.Add(new DataColumn(column.ColumnName, typeof(ListValue)));
                }
                else if (column.DataType.ImplementsInterface<IDictionary>())
                {
                    result.Columns.Add(new DataColumn(column.ColumnName, typeof(MapValue)));
                }
                else if (column.DataType == typeof(Stream))
                {
                    result.Columns.Add(new DataColumn(column.ColumnName, typeof(ByteArrayValue)));
                }
                else
                {
                    result.Columns.Add(new DataColumn(column.ColumnName, column.DataType));
                }
            }

            result.BeginLoadData();
            foreach (DataRow row in intermediateResult.Rows)
            {
                var newRow = result.NewRow();
                for (var i = 0; i < row.ItemArray.Length; i++)
                {
                    var value = row.ItemArray[i];
                    newRow[i] = ConvertValue(value!);
                }
                result.Rows.Add(newRow);
            }
            result.EndLoadData();

            return result;
        }

        private static object ConvertValue(object? value)
        {
            if (value == DBNull.Value || value is null)
            {
                return DBNull.Value;
            }
            else if (value is Dictionary<string, object?> structDictionary)
            {
                var dataRow = new QueryResultDataRow(structDictionary.Keys.ToList(),
                    structDictionary.Values.Select(ConvertValue).ToArray());

                var structValue = new StructValue(dataRow);
                return structValue;
            }
            else if (value is IList list)
            {
                var arrayList = new ArrayList(list.Count);
                var listType = typeof(object);
                for (var i = 0; i < list.Count; i++)
                {
                    var convertedListValue = ConvertValue(list[i]);
                    arrayList.Add(convertedListValue);

                    if (convertedListValue != DBNull.Value)
                    {
                        listType = convertedListValue.GetType();
                    }
                }

                var listValue = new ListValue(arrayList, listType);
                return listValue;
            }
            else if (value is IDictionary dictionary)
            {
                var keysList = new ArrayList(dictionary.Keys.Count);
                var valuesList = new ArrayList(dictionary.Values.Count);
                var keysType = typeof(object);
                var valuesType = typeof(object);
                foreach (var keyValuePair in UtilityMethods.PairEnumerables(
                    dictionary.Keys.OfType<object?>(),
                    dictionary.Values.OfType<object?>()))
                {
                    var convertedKey = ConvertValue(keyValuePair.Item1);
                    var convertedValue = ConvertValue(keyValuePair.Item2);
                    keysList.Add(convertedKey);
                    valuesList.Add(convertedValue);
                    if (convertedKey != DBNull.Value)
                    {
                        keysType = convertedKey.GetType();
                    }
                    if (convertedValue != DBNull.Value)
                    {
                        valuesType = convertedValue.GetType();
                    }
                }
                var mapValue = new MapValue(
                    keysList, keysType,
                    valuesList, valuesType);
                return mapValue;
            }
            else if (value is Stream byteArray)
            {
                if (!_wasByteArrayConversionErrorShown)
                {
                    _wasByteArrayConversionErrorShown = true;
                    MessageBox.Show(
                        Resources.Errors.ByteArraysNotSupportedErrorMessage,
                        Resources.Errors.ByteArraysNotSupportedErrorTitle,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return DBNull.Value;
            }
            else
            {
                return value;
            }
        }

        private void zoomPercentage_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem && int.TryParse(menuItem.Tag?.ToString(), out int zoomPercentage))
            {
                SetZoom(zoomPercentage);
                AppSettings.QueryEditorZoomLevel = zoomPercentage;
            }
        }

        private void SetZoom(int zoomPercentage)
        {
            zoomPercentage = Math.Clamp(zoomPercentage, 100, 150);
            foreach (ToolStripMenuItem item in this.zoomPercentageDropDown.DropDownItems)
            {
                item.Checked = zoomPercentage.ToString().Equals(item.Tag?.ToString()) == true;
            }

            zoomPercentageDropDown.Text = Resources.Strings.QueryZoomStatusTextFormat.Format(zoomPercentage);
            queryRichTextBox.Zoom = zoomPercentage;
        }

        private void cancelQueryButton_Click(object sender, EventArgs e)
        {
            _queryCancellationToken?.Cancel();
        }

        private void QueryEditorControl_Resize(object sender, EventArgs e)
        {
            if (this.ParentForm?.WindowState == FormWindowState.Minimized)
            {
                //Hide context menu on minimize to avoid a glitch where
                //the context menu won't go away until you click on it.
                resultsGridView.CloseContextMenu();
            }
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
                e.Effect = DragDropEffects.Copy;
        }

        internal class QueryExecutedEventArgs : EventArgs
        {
            public TimeSpan QueryDuration { get; }
            public TimeSpan ConversionDuration { get; }
            public TimeSpan RenderDuration { get; }

            public QueryExecutedEventArgs(TimeSpan queryDuration, TimeSpan resultConversionDuration, TimeSpan renderDuration)
            {
                QueryDuration = queryDuration;
                ConversionDuration = resultConversionDuration;
                RenderDuration = renderDuration;
            }
        }
    }
}