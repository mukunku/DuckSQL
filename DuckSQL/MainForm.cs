using DuckSQL.Controls;
using DuckSQL.Helpers;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

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
            CreateNewTab();
            tabControl1_SelectedIndexChanged(tabControl1, e);

            this.RefreshDateFormatMenuItemSelection();
            shareUsageDataToolStripMenuItem.Checked = AppSettings.AnalyticsDataGatheringConsent;
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.Escape)
            {
                this.Close();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Checks if the query editor is "empty". 
        /// Meaning it has our <see cref="Resources.Strings.NewQueryTabTitle"/> title, no file path, and no query text.
        /// </summary>
        /// <returns></returns>
        private bool IsEmptyQueryEditor(int tabIndex)
        {
            if (tabIndex >= tabControl1.TabCount)
                throw new ArgumentOutOfRangeException(nameof(tabIndex));

            var tab = tabControl1.TabPages[tabIndex];
            if (tab.Controls.Count == 0 || tab.Controls[0] is not QueryEditorControl queryEditor)
            {
                return true;
            }

            var hasNewQueryTitle = tab.Text.StartsWith(Resources.Strings.NewQueryTabTitle);
            var hasQueryText = !string.IsNullOrWhiteSpace(queryEditor.QueryText);
            var hasFilePath = !string.IsNullOrWhiteSpace(queryEditor.CurrentFilePath);

            return hasNewQueryTitle && !hasQueryText && !hasFilePath;
        }

        private int? FindEmptyQueryEditorTabIndex()
        {
            for (var i = 0; i < tabControl1.TabCount; i++)
            {
                if (IsEmptyQueryEditor(i))
                {
                    return i;
                }
            }
            return null;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tabIndex = FindEmptyQueryEditorTabIndex();
            if (tabIndex is not null)
            {
                tabControl1.SelectedIndex = tabIndex.Value;
                return;
            }

            tabIndex = CreateNewTab();
            tabControl1.SelectedIndex = tabIndex.Value;
        }

        private int CreateNewTab(string? queryTextOrFilePath = null)
        {
            var newTab = new TabPage();
            SetTabText(newTab, Resources.Strings.NewQueryTabTitle);
            newTab.Tag = queryTextOrFilePath;
            tabControl1.TabPages.Add(newTab);
            return tabControl1.TabCount - 1;
        }

        private async void fileOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                await OpenNewFileOrFolderAsync(openFileDialog.FileName);
            }
        }

        private async void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                await OpenNewFileOrFolderAsync(openFolderDialog.SelectedPath);
            }
        }

        private void saveResultsAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
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

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab is null)
                return;

            if (tabControl1.SelectedTab.Controls.Count == 0)
            {
                //Lazily initialize the query editor
                QueryEditorControl queryEditor;
                var tag = tabControl1.SelectedTab.Tag?.ToString();
                if (!string.IsNullOrWhiteSpace(tag))
                {
                    if (File.Exists(tag) || Directory.Exists(tag))
                    {
                        queryEditor = new QueryEditorControl(dataFilePath: tag) { Dock = DockStyle.Fill };
                    }
                    else
                    {
                        queryEditor = new QueryEditorControl() { Dock = DockStyle.Fill };
                        queryEditor.QueryText = tag;
                    }
                }
                else
                {
                    queryEditor = new QueryEditorControl { Dock = DockStyle.Fill };
                }

                AttachEventHandlers(queryEditor, tabControl1.SelectedTab);
                tabControl1.SelectedTab.Controls.Add(queryEditor);
            }
            else
            {
                var queryEditor = (QueryEditorControl)tabControl1.SelectedTab.Controls[0];
                queryEditor.RefreshControl();
            }
        }

        private static void AttachEventHandlers(QueryEditorControl queryEditor, TabPage tab)
        {
            //Handle new tab title styling
            queryEditor.QueryTextChanged += (s, args) =>
            {
                if (!tab.Text.StartsWith(Resources.Strings.NewQueryTabTitle))
                {
                    return; //This is a file backed tab. So it will be styled separately
                }

                var isEmpty = string.IsNullOrWhiteSpace(queryEditor.QueryText);
                var hasModifiedIndicator = tab.Text.EndsWith('*');

                if (hasModifiedIndicator && isEmpty)
                {
                    SetTabText(tab, tab.Text.Replace("*", string.Empty).Trim());
                }
                else if (!hasModifiedIndicator && !isEmpty)
                {
                    SetTabText(tab, tab.Text += " *");
                }
            };
        }

        private async void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            var files = e.Data?.GetData(DataFormats.FileDrop) as string[];
            if (files?.Length > 0)
            {
                await OpenNewFileOrFolderAsync(files[0]);
            }
        }

        private async Task OpenNewFileOrFolderAsync(string fileOrFolderPath)
        {
            var tabIndex = CreateNewTab(fileOrFolderPath);
            tabControl1.SelectedIndex = tabIndex;

            if (tabControl1.TabCount > 1 && IsEmptyQueryEditor(tabControl1.TabCount - 2))
            {
                //If the last tab was an empty one, get rid of it
                tabControl1.TabPages.RemoveAt(tabControl1.TabCount - 2);
            }

            SetTabText(tabControl1.SelectedTab, tabControl1.SelectedTab?.Text + " *"); //mark the tab as dirty
        }

        private static void SetTabText(TabPage? tab, string title)
        {
            tab?.Text = title;
            if (tab is not null)
            {
                //Dynamically set label width
                var width = MeasureStringWidth(tab.CreateGraphics(), tab.Font, title, false);
                width += 24; //Add some buffer for the close button
                width = Math.Clamp(width, 12, 120);
                tab.Width = width;
            }
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
                e.Effect = DragDropEffects.Copy;
        }

        private void TabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabPage = this.tabControl1.TabPages[e.Index];
            var tabRect = this.tabControl1.GetTabRect(e.Index);
            tabRect.Inflate(-2, -2);

            var closeButtonRectangle = GetTabCloseButtonRectangle(e.Index);
            if (_closeButtonHoveredTabIndex == e.Index)
            {
                if (_isMouseLeftButtonDown)
                {
                    ControlPaint.DrawButton(e.Graphics, closeButtonRectangle, ButtonState.Pushed);
                }
                else
                {
                    ControlPaint.DrawButton(e.Graphics, closeButtonRectangle, ButtonState.Normal);
                }
            }
            else
            {
                ControlPaint.DrawButton(e.Graphics, closeButtonRectangle, ButtonState.Flat);
            }

            closeButtonRectangle.Inflate(-2, -2); //Add some padding inside the button
            e.Graphics.DrawImage(Properties.Resources.close,
                closeButtonRectangle.X,
                closeButtonRectangle.Y,
                closeButtonRectangle.Width,
                closeButtonRectangle.Height);

            TextRenderer.DrawText(e.Graphics, tabPage.Text, tabPage.Font,
                tabRect, tabPage.ForeColor, TextFormatFlags.Left);
        }

        private Rectangle GetTabCloseButtonRectangle(int tabIndex)
        {
            const int iconSize = 12;
            var tabPage = tabControl1.TabPages[tabIndex];
            var tabRect = tabControl1.GetTabRect(tabIndex);
            tabRect.Inflate(-2, -2);

            return new Rectangle(
                tabRect.Right - iconSize - 2,
                tabRect.Top + (tabRect.Height - iconSize) / 2,
                iconSize, iconSize
                );
        }

        private static int MeasureStringWidth(Graphics gfx, Font font, string input, bool appendWhitespaceBuffer)
        {
            const string WHITESPACE_BUFFER = "#";
            try
            {
                var width = (int)gfx.MeasureString(input + (appendWhitespaceBuffer ? WHITESPACE_BUFFER : string.Empty), font).Width;

                if (width <= 0) //happens with really long strings sometimes
                    return int.MaxValue;
                else
                    return width;
            }
            catch (Exception)
            {
                return int.MaxValue; //Assume worst case
            }
        }

        private int? _closeButtonHoveredTabIndex = null;
        private void tabControl1_MouseMove(object sender, MouseEventArgs e)
        {
            int? hoveredTabIndex = null;
            for (var i = 0; i < tabControl1.TabCount; i++)
            {
                var closeButtonRectangle = GetTabCloseButtonRectangle(i);
                if (closeButtonRectangle.Contains(e.Location))
                {
                    hoveredTabIndex = i;
                    break;
                }
            }

            if (hoveredTabIndex != _closeButtonHoveredTabIndex)
            {
                _closeButtonHoveredTabIndex = hoveredTabIndex;
                tabControl1.Invalidate(); //Trigger redraw to show hover effect
            }
        }

        private bool _isMouseLeftButtonDown = false;
        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            _isMouseLeftButtonDown = e.Button == MouseButtons.Left;

            //Check if the close button was clicked
            for (var i = 0; i < tabControl1.TabCount; i++)
            {
                var closeButtonRectangle = GetTabCloseButtonRectangle(i);
                if (closeButtonRectangle.Contains(e.Location))
                {
                    tabControl1.Invalidate();
                    break;
                }
            }
        }

        private void tabControl1_MouseUp(object sender, MouseEventArgs e)
        {
            _isMouseLeftButtonDown = false;

            //Check if the close button was clicked
            for (var i = 0; i < tabControl1.TabCount; i++)
            {
                var closeButtonRectangle = GetTabCloseButtonRectangle(i);
                if (closeButtonRectangle.Contains(e.Location))
                {
                    CloseTab(i);
                    break;
                }
            }
        }

        private void CloseTab(int tabIndex)
        {
            //Don't close if this is already the only, empty, tab
            if (tabControl1.TabCount == 1 && IsEmptyQueryEditor(tabIndex))
            {
                tabControl1.Invalidate(); //Animate the close button
                return;
            }

                tabControl1.TabPages.RemoveAt(tabIndex);
            if (tabControl1.TabCount == 0)
            {
                CreateNewTab();
                tabControl1_SelectedIndexChanged(tabControl1, EventArgs.Empty);
            }
        }
    }
}