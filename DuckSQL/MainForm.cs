using DuckSQL.Analytics;
using DuckSQL.Controls;
using DuckSQL.Exceptions;
using DuckSQL.Helpers;
using DuckSQL.Resources;
using DuckDB.NET.Data;
using MiniExcelLibs;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DuckSQL
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            tabControl1.TabPages.Clear();
            tabControl1.NewQuery();
            tabControl1.QueryExecuted += QueryExecutedEventHandler;
            tabControl1.QueryExecutionStarted += QueryExecutionStartedEventHandler;
            tabControl1.DragDrop += MainForm_DragDrop;
            tabControl1.TabDirtyStateChanged += TabControl1_TabDirtyStateChanged;

            this.RefreshDateFormatMenuItemSelection();
            shareUsageDataToolStripMenuItem.Checked = AppSettings.AnalyticsDataGatheringConsent;

            //Restore tabs from last session if any
            tabControl1.ResumeLastSession();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.NewQuery();
        }

        private void fileOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                MenuBarClickEvent.FireAndForget(MenuBarClickEvent.ActionId.FileOpen);
                tabControl1.OpenFilesOrFolders(openFileDialog.FileName);
            }
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                MenuBarClickEvent.FireAndForget(MenuBarClickEvent.ActionId.FolderOpen);
                tabControl1.OpenFilesOrFolders(openFolderDialog.SelectedPath);
            }
        }

        private async void saveResultsAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await ExportResultsAsync(FileType.CSV);
        }

        private async void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var eventTask = new MenuBarClickEvent() { Action = MenuBarClickEvent.ActionId.Exit }.Record();
            var timeout = Task.Delay(200);
            
            await Task.WhenAny(timeout, eventTask);

            Application.Exit();
        }

        private void dateFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag is string tag)
            {
                var selectedDateFormat = (DateFormat)int.Parse(tag);

                if (selectedDateFormat != DateFormat.Custom)
                {
                    AppSettings.DateTimeDisplayFormat = selectedDateFormat;
                }
                else
                {
                    string? customDateFormat = null;
                    if (AppSettings.DateTimeDisplayFormat == DateFormat.Custom)
                    {
                        customDateFormat = AppSettings.CustomDateFormat;
                    }

                    using var customDateFormatInputForm = new CustomDateFormatInputForm(customDateFormat);
                    if (customDateFormatInputForm.ShowDialog(this) == DialogResult.OK)
                    {
                        AppSettings.DateTimeDisplayFormat = DateFormat.Custom;
                        AppSettings.CustomDateFormat = customDateFormatInputForm.UserEnteredDateFormat;
                    }
                }

                this.RefreshDateFormatMenuItemSelection();

                // Refresh all query editor controls
                foreach (TabPage tabPage in tabControl1.TabPages)
                {
                    if (tabPage.Controls[0] is QueryEditorControl editor)
                    {
                        editor.RefreshControl();
                    }
                }
            }
        }

        private void RefreshDateFormatMenuItemSelection()
        {
            this.defaultToolStripMenuItem.Checked = false;
            this.iSO8601ToolStripMenuItem.Checked = false;
            this.customToolStripMenuItem.Checked = false;

            switch (AppSettings.DateTimeDisplayFormat)
            {
                case DateFormat.Default:
                    this.defaultToolStripMenuItem.Checked = true;
                    break;
                case DateFormat.ISO8601:
                    this.iSO8601ToolStripMenuItem.Checked = true;
                    break;
                case DateFormat.Custom:
                    this.customToolStripMenuItem.Checked = true;
                    break;
                default:
                    break;
            }
        }

        private void userGuideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuBarClickEvent.FireAndForget(MenuBarClickEvent.ActionId.UserGuide);
            Process.Start(new ProcessStartInfo(Constants.WikiURL) { UseShellExecute = true });
        }

        private void shareUsageDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shareUsageDataToolStripMenuItem.Checked = !this.shareUsageDataToolStripMenuItem.Checked;
            AppSettings.AnalyticsDataGatheringConsent = this.shareUsageDataToolStripMenuItem.Checked;
            AppSettings.ConsentLastAskedOnVersion = Env.AssemblyVersion;
        }

        private void languageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is not ToolStripItem toolStripItem)
            {
                return;
            }

            var targetCulture = toolStripItem.Tag?.ToString();
            if (string.IsNullOrWhiteSpace(targetCulture))
            {
                targetCulture = "en-US";
            }

            if (!UtilityMethods.TryParseCultureInfo(targetCulture, out CultureInfo? newCultureInfo))
            {
                return;
            }

            if (newCultureInfo.Equals(CultureInfo.CurrentUICulture))
            {
                return;
            }

            if (MessageBox.Show(this,
                Resources.Strings.LanguageChangeConfirmationMessage,
                Resources.Strings.LanguageChangeConfirmationTitle,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            AppSettings.UserSelectedCulture = newCultureInfo;
            UtilityMethods.RestartApplication();
        }

        private void aboutToolStripMenuItem_Click(object _, EventArgs e)
        {
            MenuBarClickEvent.FireAndForget(MenuBarClickEvent.ActionId.AboutBox);
            using var aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void MainForm_DragDrop(object? _, DragEventArgs e)
        {
            var files = e.Data?.GetData(DataFormats.FileDrop) as string[];
            if (files?.Length > 0)
            {
                const int WARNING_DRAG_DROP_FILE_COUNT = 10;
                if (files.Length > WARNING_DRAG_DROP_FILE_COUNT && MessageBox.Show(
                    Strings.DragDropTooManyFilesWarningMessageFormat.Format(files.Length), Strings.DragDropTooManyFilesWarningTitleFormat.Format(files.Length),
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                {
                    return;
                }

                MenuBarClickEvent.FireAndForget(MenuBarClickEvent.ActionId.DragDrop);
                tabControl1.OpenFilesOrFolders(files);
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.W)
            {
                tabControl1.CloseTab(tabControl1.SelectedIndex);
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode >= Keys.D1 && e.KeyCode <= Keys.D9)
            {
                int tabIndex = e.KeyCode - Keys.D1;
                if (tabIndex < tabControl1.TabCount)
                {
                    tabControl1.SelectedIndex = tabIndex;
                    e.Handled = true;
                }
            }
            else if (e.Control && e.KeyCode == Keys.M)
            {
                //This is a weird shortcut that starts some kind of recording feature in FCTB, so just disable it
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.Control && (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right))
            {
                var hasTabToTheLeft = tabControl1.SelectedIndex > 0;
                var hasTabToTheRight = tabControl1.SelectedIndex < tabControl1.TabCount - 1;
                if (e.KeyCode == Keys.Left && hasTabToTheLeft)
                {
                    tabControl1.SelectedIndex--;
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Right && hasTabToTheRight)
                {
                    tabControl1.SelectedIndex++;
                    e.Handled = true;
                }
            }
            else if (e.Control && e.KeyCode == Keys.T)
            {
                tabControl1.NewQuery();
                e.Handled = true;
            }
        }

        //Source: https://stackoverflow.com/a/25648710/1458738
        //Enabled DoubleBufferring for all child controls of the form
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED       
                return handleParam;
            }
        }

        //Every way of closing the app ends up here (X button, Alt+F4, File > Exit, restarts, Windows shutdown)
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.Cancel)
                return;

            tabControl1.SaveSessionCacheAndStopSession();
        }

        private async Task ExportResultsAsync(FileType defaultFileType, string? filePathWithExtension = null)
        {
            string? filePath = null;
            LoadingIcon? loadingIcon = null;
            FileType? rerunType = null;
            var editor = tabControl1.SelectedTab?.Controls[0] as QueryEditorControl;
            var dataTable = editor?.GetDataSource();

            if (dataTable is null || dataTable.Columns.Count == 0)
                return;

            try
            {
                this.exportFileDialog.Title = Resources.Strings.RecordsToBeExportedTitleFormat.Format(dataTable.Rows.Count);
                this.exportFileDialog.Filter = "CSV file (*.csv)|*.csv|JSON file (*.json)|*.json|Excel '93 file (*.xls)|*.xls|Excel '07 file (*.xlsx)|*.xlsx|Parquet file (*.parquet)|*.parquet";
                this.exportFileDialog.FilterIndex = (int)defaultFileType + 1;
                if (filePathWithExtension is not null || this.exportFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = filePathWithExtension ?? this.exportFileDialog.FileName;
                    CleanupFile(filePath); //Delete any existing file (user already confirmed any overwrite)

                    var fileExtension = Path.GetExtension(filePath);
                    FileType? selectedFileType = UtilityMethods.ExtensionToFileType(fileExtension);
                    if (selectedFileType is null)
                        throw new ArgumentOutOfRangeException(fileExtension);

                    var stopWatch = Stopwatch.StartNew();
                    loadingIcon = this.ShowLoadingIcon(Resources.Strings.ExportingDataLabelText, dataTable.DefaultView.Count * dataTable.Columns.Count);

                    await ExportResultsImpl(dataTable, selectedFileType.Value, filePath, loadingIcon.CancellationToken,
                        loadingIcon, tabControl1.SelectedTab?.Text, editor?.LastExecutedQuery);

                    stopWatch.Stop();
                    loadingIcon.Hide();

                    if (loadingIcon.CancellationToken.IsCancellationRequested)
                    {
                        CleanupFile(filePath);
                        MessageBox.Show(Resources.Strings.ExportCancelledMessage, Resources.Strings.ExportCancelledTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        long fileSizeInBytes = new FileInfo(filePath).Length;
                        var fileSizeInMegaBytes = (float)Math.Round((fileSizeInBytes / 1024.0) / 1024.0, 2);

                        FileExportEvent.FireAndForget(selectedFileType.Value, fileSizeInMegaBytes, dataTable.Rows.Count, dataTable.Columns.Count, stopWatch.ElapsedMilliseconds);

                        MessageBox.Show(this,
                            Resources.Strings.ExportSuccessfulMessageFormat.Format(fileSizeInMegaBytes),
                            Resources.Strings.ExportSuccessfulTitle,
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex) when (ex is IOException or DuckDBException)
            {
                //DuckDBException: parquet export re-runs the query, which can fail (e.g. source file moved or isn't a SELECT)
                CleanupFile(filePath);
                MessageBox.Show(ex.Message, Resources.Errors.ExportFailedErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (XlsCellLengthException ex)
            {
                CleanupFile(filePath);

                if (MessageBox.Show(this,
                    Resources.Strings.SwitchFromXlsToXlsxMessageFormat.Format(ex.MaxLength, FileType.XLS.GetExtension(), FileType.XLSX.GetExtension()),
                    Resources.Strings.SwitchFromXlsToXlsxMessageTitle,
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    rerunType = FileType.XLSX;
                }
            }
            catch (Exception)
            {
                CleanupFile(filePath);
                throw;
            }
            finally
            {
                loadingIcon?.Dispose();
            }

            if (rerunType is not null)
            {
                await ExportResultsAsync(default, filePath is not null ? Path.ChangeExtension(filePath, rerunType.Value.ToString()) : filePath);
            }

            static void CleanupFile(string? filePath)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(filePath))
                        File.Delete(filePath);
                }
                catch (Exception) { /*Swallow*/ }
            }
        }

        private LoadingIcon ShowLoadingIcon(string message, long loadingBarMax = 0)
        {
            var loadingIcon = new LoadingIcon(message, loadingBarMax);
            loadingIcon.OnShow += (object? sender, EventArgs e) =>
            {
                this.tabControl1.Enabled = false;
                this.menuStrip1.Enabled = false;
            };
            loadingIcon.OnHide += (object? sender, EventArgs e) =>
            {
                this.tabControl1.Enabled = true;
                this.menuStrip1.Enabled = true;
            };

            loadingIcon.Show(this);
            return loadingIcon;
        }

        private static Task ExportResultsImpl(
           DataTable dataTable,
           FileType fileType,
           string filePath,
           CancellationToken cancellationToken,
           IProgress<int> progress,
           string? sourceTabName,
           string? sourceQuery)
        {
            if (fileType == FileType.CSV)
            {
                return WriteDataToCSVFile(dataTable, filePath, cancellationToken, progress);
            }
            else if (fileType == FileType.XLS)
            {
                const int MAX_XLS_COLUMN_COUNT = 256; //.xls format has a hard limit on 256 columns
                if (dataTable.Columns.Count > MAX_XLS_COLUMN_COUNT)
                {
                    MessageBox.Show(
                        Resources.Errors.TooManyColumnsErrorMessageFormat.Format(
                            FileType.XLS.ToString(), MAX_XLS_COLUMN_COUNT, dataTable.Columns.Count),
                        Resources.Errors.TooManyColumnsErrorTitle,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return Task.CompletedTask;
                }

                return WriteDataToExcel93File(dataTable, filePath, cancellationToken, progress);
            }
            else if (fileType == FileType.XLSX)
            {
                const int MAX_XLSX_COLUMN_COUNT = 16384; //.xlsx format has a hard limit on 16384 columns
                if (dataTable.Columns.Count > MAX_XLSX_COLUMN_COUNT)
                {
                    MessageBox.Show(
                        Resources.Errors.TooManyColumnsErrorMessageFormat.Format(
                            FileType.XLSX.ToString(), MAX_XLSX_COLUMN_COUNT, dataTable.Columns.Count),
                        Resources.Errors.TooManyColumnsErrorTitle,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return Task.CompletedTask;
                }

                return WriteDataToExcel2007File(dataTable, filePath, "Sheet1", cancellationToken, progress);
            }
            else if (fileType == FileType.JSON)
            {
                return WriteDataToJSONFile(dataTable, filePath, cancellationToken, progress);
            }
            else if (fileType == FileType.PARQUET)
            {
                if (string.IsNullOrWhiteSpace(sourceQuery))
                {
                    MessageBox.Show(
                        Resources.Errors.ParquetExportQueryUnavailableErrorMessage,
                        Resources.Errors.ExportFailedErrorTitle,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return Task.CompletedTask;
                }

                return WriteDataToParquetFile(dataTable, sourceQuery, filePath, cancellationToken, progress);
            }
            else
            {
                throw new NotImplementedException(fileType.ToString());
            }
        }

        private static async Task WriteDataToCSVFile(DataTable dataTable, string path, CancellationToken cancellationToken, IProgress<int> progress)
            => await Task.Run(() =>
            {
                using var writer = new StreamWriter(path, false, Encoding.UTF8);

                var rowBuilder = new StringBuilder();
                bool isFirst = true;
                foreach (DataColumn column in dataTable.Columns)
                {
                    if (!isFirst)
                    {
                        rowBuilder.Append(',');
                    }
                    else
                    {
                        isFirst = false;
                    }

                    rowBuilder.Append(
                                column.ColumnName
                                    .Replace("\r", string.Empty)
                                    .Replace("\n", string.Empty)
                                    .Replace(",", string.Empty));
                }
                writer.WriteLine(rowBuilder.ToString());

                string dateFormat = Env.DateTimeFormat;
                string dateOnlyFormat = Env.DateOnlyFormat;
                foreach (DataRowView row in dataTable.DefaultView)
                {
                    rowBuilder.Clear();

                    isFirst = true;
                    foreach (object? value in row.Row.ItemArray)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }

                        if (!isFirst)
                        {
                            rowBuilder.Append(',');
                        }
                        else
                        {
                            isFirst = false;
                        }

                        if (value is DateTime dt)
                        {
                            rowBuilder.Append(UtilityMethods.CleanCSVValue(dt.ToString(dateFormat)));
                        }
                        else if (value is DateOnly dateOnly)
                        {
                            rowBuilder.Append(UtilityMethods.CleanCSVValue(dateOnly.ToString(dateOnlyFormat)));
                        }
                        else
                        {
                            var stringValue = value!.ToString()!; //we never have `null` only `DBNull.Value`
                            rowBuilder.Append(UtilityMethods.CleanCSVValue(stringValue));
                        }

                        progress.Report(1);
                    }

                    writer.WriteLine(rowBuilder.ToString());
                }
            }, cancellationToken);

        private static Task WriteDataToExcel93File(DataTable dataTable, string path, CancellationToken cancellationToken, IProgress<int> progress)
            => Task.Run(() =>
            {
                string dateFormat = Env.DateTimeFormat;
                string dateOnlyFormat = Env.DateOnlyFormat;
                using var fs = new FileStream(path, FileMode.OpenOrCreate);
                var excelWriter = new ExcelWriter(fs);
                excelWriter.BeginWrite();

                //Write headers
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    excelWriter.WriteCell(0, i, dataTable.Columns[i].ColumnName);
                }

                //Write data
                for (int i = 0; i < dataTable.DefaultView.Count; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        var value = dataTable.DefaultView[i][j];
                        if (value == DBNull.Value)
                        {
                            excelWriter.WriteCell(i + 1, j); //empty cell
                        }
                        else if (IsIntCastSafe(value))
                        {
                            excelWriter.WriteCell(i + 1, j, Convert.ToInt32(value));
                        }
                        else if (IsDoubleCastSafe(value))
                        {
                            excelWriter.WriteCell(i + 1, j, Convert.ToDouble(value));
                        }
                        else if (value is DateTime dt)
                        {
                            excelWriter.WriteCell(i + 1, j, dt.ToString(dateFormat));
                        }
                        else if (value is DateOnly dateOnly)
                        {
                            excelWriter.WriteCell(i + 1, j, dateOnly.ToString(dateOnlyFormat));
                        }
                        else
                        {
                            var stringValue = value.ToString();

                            //BUG: for some reason strings longer than 255 characters appear empty.
                            const int maxSupportedCellLength = 255;
                            if (stringValue!.Length > maxSupportedCellLength)
                            {
                                throw new XlsCellLengthException(maxSupportedCellLength);
                            }

                            excelWriter.WriteCell(i + 1, j, stringValue);
                        }
                        progress.Report(1);
                    }
                }

                bool IsIntCastSafe(object value) => value.GetType() == typeof(int)
                    || value.GetType() == typeof(uint)
                    || value.GetType() == typeof(sbyte)
                    || value.GetType() == typeof(byte);

                bool IsDoubleCastSafe(object value) => value.GetType() == typeof(double)
                    || value.GetType() == typeof(decimal)
                    || value.GetType() == typeof(float)
                    || value.GetType() == typeof(long);

                excelWriter.EndWrite();
            }, cancellationToken);

        private static async Task WriteDataToExcel2007File(DataTable mainDataSource, string path, string sheetName, CancellationToken cancellationToken, IProgress<int> progress)
        {
            const int MAX_XLSX_SHEET_NAME_LENGTH = 31;

            //sanitize sheet name
            sheetName = Regex.Replace(sheetName, "[^a-zA-Z0-9 _\\-()]", string.Empty).Left(MAX_XLSX_SHEET_NAME_LENGTH);

            using var fs = new FileStream(path, FileMode.OpenOrCreate);
            await fs.SaveAsAsync(mainDataSource, printHeader: true, sheetName, ExcelType.XLSX, configuration: null, progress, cancellationToken);
        }

        private static Task WriteDataToJSONFile(DataTable dataTable, string path, CancellationToken cancellationToken, IProgress<int> progress)
            => Task.Run(() =>
            {
                using var fs = new FileStream(path, FileMode.OpenOrCreate);
                using var jsonWriter = new CustomJsonWriter(fs);

                jsonWriter.WriteStartArray();
                foreach (DataRowView row in dataTable.DefaultView)
                {
                    jsonWriter.WriteStartObject();
                    for (var i = 0; i < row.Row.ItemArray.Length; i++)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }

                        var columnName = dataTable.Columns[i].ColumnName;
                        jsonWriter.WritePropertyName(columnName);

                        object? value = row.Row.ItemArray[i];
                        jsonWriter.WriteValue(value!, false);
                        progress.Report(1);
                    }
                    jsonWriter.WriteEndObject();
                }
                jsonWriter.WriteEndArray();
            }, cancellationToken);

        private static Task WriteDataToParquetFile(DataTable dataTable, string query, string path, CancellationToken cancellationToken, IProgress<int> progress)
            => Task.Run(() =>
            {
                //Re-run the source query straight into a parquet file so DuckDB preserves the exact column types (structs, lists, maps, etc.)
                //Trailing semicolons aren't allowed inside COPY (...) and the newline guards against a trailing line comment
                var sourceQuery = query.Trim().TrimEnd(';').TrimEnd();
                var copyQuery = $"SELECT * FROM ({sourceQuery}\n) {GetOrderByClause(dataTable)}";

                using var connection = new DuckDBConnection("Data Source=:memory:");
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = $"COPY ({copyQuery}) TO '{path.Replace("'", "''")}' (FORMAT parquet, COMPRESSION zstd);";

                using var cancellationRegistration = cancellationToken.Register(command.Cancel);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex) when (cancellationToken.IsCancellationRequested && ex is OperationCanceledException or DuckDBException)
                {
                    return; //The caller cleans up the partially written file
                }

                progress.Report(dataTable.DefaultView.Count * dataTable.Columns.Count);

                //Carry over any sorting the user applied in the grid. Columns are referenced by position since
                //DataTable may have renamed duplicate column names (e.g. `SELECT a, a`)
                static string GetOrderByClause(DataTable dataTable)
                {
                    var sort = dataTable.DefaultView.Sort;
                    if (string.IsNullOrWhiteSpace(sort))
                        return string.Empty;

                    foreach (DataColumn column in dataTable.Columns)
                    {
                        foreach (var columnToken in new[] { $"[{column.ColumnName}]", $"[{column.ColumnName.Replace("]", "\\]")}]", column.ColumnName })
                        {
                            //DataView sorts DBNull first when ascending, so match that
                            if (sort.Equals($"{columnToken} ASC", StringComparison.Ordinal) || sort.Equals(columnToken, StringComparison.Ordinal))
                                return $"ORDER BY {column.Ordinal + 1} ASC NULLS FIRST";
                            if (sort.Equals($"{columnToken} DESC", StringComparison.Ordinal))
                                return $"ORDER BY {column.Ordinal + 1} DESC NULLS LAST";
                        }
                    }

                    throw new NotSupportedException($"Unable to translate grid sort '{sort}' to SQL");
                }
            }, cancellationToken);

        private void QueryExecutedEventHandler(object? sender, QueryEditorControl.QueryExecutedEventArgs args)
        {
            ToggleSaveResultsFileMenuItem();

            var dataTable = tabControl1.SelectedTab?.Controls[0] is QueryEditorControl editor ? editor.GetDataSource() : null;
            if (dataTable is null)
                return;

            ExecuteQueryEvent.FireAndForget(
                null, //TODO: track Open File record count info if possible
                dataTable.Rows.Count,
                dataTable.Columns.Count,
                (long)args.QueryDuration.TotalMilliseconds,
                (long)args.ConversionDuration.TotalMilliseconds,
                (long)args.RenderDuration.TotalMilliseconds
                );
        }

        private void QueryExecutionStartedEventHandler(object? sender, EventArgs args)
        {
            saveResultsAsToolStripMenuItem.Enabled = false;
        }

        private void ToggleSaveResultsFileMenuItem()
        {
            //Check if we have data to export and enable/disable the "Save results as" menu item accordingly
            saveResultsAsToolStripMenuItem.Enabled =
                tabControl1.SelectedTab?.Controls.Count > 0
                && tabControl1.SelectedTab?.Controls[0] is QueryEditorControl editor
                && editor.GetDataSource().Columns.Count > 0;
        }

        private void ToggleSaveQueryFileMenuItem(bool? enable = null)
        {
            saveQueryToolStripMenuItem.Enabled = (tabControl1.SelectedTab as CustomTabPage)?.IsDirty == true;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToggleSaveResultsFileMenuItem();
            ToggleSaveQueryFileMenuItem();
        }

        private void MainForm_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (tabControl1.TabCount == 0)
                return; //shouldn't ever happen but let's be safe

            //The tab control can't capture mouse click events in the empty space to the right of the last tab,
            //so we handle double clicks on the form itself to allow users to open files by double-clicking in that empty space
            var tabControlHotbarRectangle = tabControl1.GetTabRect(tabControl1.TabCount - 1);

            var x = tabControl1.Location.X + tabControlHotbarRectangle.Width;
            var y = tabControl1.Location.Y;
            var width = tabControl1.ClientRectangle.Width - x;
            var height = tabControlHotbarRectangle.Height;

            if (width <= 1)
                return; //case where we run out of empty space

            var emptyHotbarAreaRectangle = new Rectangle(x, y, width, height);
            var wasEmptyHotbarDoubleClicked = emptyHotbarAreaRectangle.Contains(e.Location);
            if (wasEmptyHotbarDoubleClicked)
            {
                tabControl1.NewQuery();
            }
        }

        private void openQueryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openQueryDialog.ShowDialog() == DialogResult.OK)
            {
                //We might have switched to an already open tab but lets record the analytics regardless for simplicity
                MenuBarClickEvent.FireAndForget(MenuBarClickEvent.ActionId.QueryOpen);
                tabControl1.OpenFilesOrFolders(openQueryDialog.FileName);
            }
        }

        private void saveQueryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedTab = tabControl1.SelectedTab as CustomTabPage;
            if (selectedTab is null)
                return;

            if (selectedTab.IsDirty)
            {
                var trimmedTabTitle = selectedTab.Text.Trim(' ', '*');
                saveQueryDialog.FileName = trimmedTabTitle.EndsWith(".sql") ? trimmedTabTitle : (trimmedTabTitle + ".sql");

                var shouldSave = selectedTab.IsFileBackedTab || saveQueryDialog.ShowDialog() == DialogResult.OK;
                if (shouldSave)
                {
                    var filePath = selectedTab.IsFileBackedTab ? selectedTab.FilePath! : saveQueryDialog.FileName;

                    MenuBarClickEvent.FireAndForget(MenuBarClickEvent.ActionId.QuerySave);
                    File.WriteAllText(filePath, selectedTab.CurrentText);
                    selectedTab.MarkSaved(filePath); //raises TabDirtyStateChanged, which updates the save menu item
                    Invalidate();
                }
            }
        }

        private void TabControl1_TabDirtyStateChanged(object? sender, EventArgs e)
        {
            ToggleSaveQueryFileMenuItem();
        }
    }
}