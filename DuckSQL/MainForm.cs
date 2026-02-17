using DuckSQL.Controls;
using DuckSQL.Helpers;
using System.Diagnostics;
using System.Globalization;

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
            tabControl1.CreateNewTab();
            tabControl1.TriggerSelectedIndexChanged();

            this.RefreshDateFormatMenuItemSelection();
            shareUsageDataToolStripMenuItem.Checked = AppSettings.AnalyticsDataGatheringConsent;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tabIndex = tabControl1.FindEmptyQueryEditorTabIndex();
            if (tabIndex is not null)
            {
                tabControl1.SelectedIndex = tabIndex.Value;
                return;
            }

            tabIndex = tabControl1.CreateNewTab();
            tabControl1.SelectedIndex = tabIndex.Value;
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
            var tabIndex = tabControl1.CreateNewTab(fileOrFolderPath);
            tabControl1.SelectedIndex = tabIndex;

            if (tabControl1.TabCount > 1 && tabControl1.IsEmptyQueryEditor(tabControl1.TabCount - 2))
            {
                //If the last tab was an empty one, get rid of it
                tabControl1.TabPages.RemoveAt(tabControl1.TabCount - 2);
            }

            tabControl1.SelectedTab?.Text = tabControl1.SelectedTab?.Text + " *"; //mark the tab as dirty
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
                e.Effect = DragDropEffects.Copy;
        }
    }
}