using DuckSQL.Controls;
using FastColoredTextBoxNS;
using System.Windows.Forms;
using System.Xml.Linq;

namespace DuckSQL
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            mainSplitContainer = new SplitContainer();
            mainTableLayoutPanel = new TableLayoutPanel();
            queryRichTextBox = new FastColoredTextBox();
            queryEditorContextMenuStrip = new ContextMenuStrip(components);
            copyTextMenuItem = new ToolStripMenuItem();
            pasteTextMenuItem = new ToolStripMenuItem();
            executeQueryButton = new Button();
            querySyntaxDocsButton = new Button();
            resultsGridView = new ParquetGridView();
            statusStrip = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            showingCountLabel = new ToolStripStatusLabel();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            toolStripStatusLabel3 = new ToolStripStatusLabel();
            zoomPercentageDropDown = new ToolStripDropDownButton();
            percent100 = new ToolStripMenuItem();
            percent110 = new ToolStripMenuItem();
            percent125 = new ToolStripMenuItem();
            percent140 = new ToolStripMenuItem();
            percent150 = new ToolStripMenuItem();
            queryExecutionStatusLabel = new ToolStripStatusLabel();
            timeElapsedLabel = new ToolStripStatusLabel();
            closeButton = new Button();
            executeQueryKeyboardShortcutToolTip = new ToolTip(components);
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            fileOpenToolStripMenuItem = new ToolStripMenuItem();
            openFolderToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            saveResultsAsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            dateFormatToolStripMenuItem = new ToolStripMenuItem();
            defaultToolStripMenuItem = new ToolStripMenuItem();
            iSO8601ToolStripMenuItem = new ToolStripMenuItem();
            customToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            userGuideToolStripMenuItem = new ToolStripMenuItem();
            shareUsageDataToolStripMenuItem = new ToolStripMenuItem();
            languageToolStripMenuItem1 = new ToolStripMenuItem();
            englishToolStripMenuItem = new ToolStripMenuItem();
            türkçeToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)mainSplitContainer).BeginInit();
            mainSplitContainer.Panel1.SuspendLayout();
            mainSplitContainer.Panel2.SuspendLayout();
            mainSplitContainer.SuspendLayout();
            mainTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)queryRichTextBox).BeginInit();
            queryEditorContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)resultsGridView).BeginInit();
            statusStrip.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // mainSplitContainer
            // 
            mainSplitContainer.BackColor = SystemColors.Control;
            mainSplitContainer.BorderStyle = BorderStyle.FixedSingle;
            resources.ApplyResources(mainSplitContainer, "mainSplitContainer");
            mainSplitContainer.Name = "mainSplitContainer";
            // 
            // mainSplitContainer.Panel1
            // 
            mainSplitContainer.Panel1.Controls.Add(mainTableLayoutPanel);
            // 
            // mainSplitContainer.Panel2
            // 
            mainSplitContainer.Panel2.BackColor = SystemColors.Control;
            mainSplitContainer.Panel2.Controls.Add(resultsGridView);
            // 
            // mainTableLayoutPanel
            // 
            mainTableLayoutPanel.Controls.Add(queryRichTextBox, 0, 0);
            mainTableLayoutPanel.Controls.Add(executeQueryButton, 0, 1);
            mainTableLayoutPanel.Controls.Add(querySyntaxDocsButton, 1, 1);
            resources.ApplyResources(mainTableLayoutPanel, "mainTableLayoutPanel");
            mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            // 
            // queryRichTextBox
            // 
            resources.ApplyResources(queryRichTextBox, "queryRichTextBox");
            queryRichTextBox.AutoCompleteBracketsList = new char[]
    {
    '(',
    ')',
    '{',
    '}',
    '[',
    ']',
    '"',
    '"',
    '\'',
    '\''
    };
            queryRichTextBox.AutoIndentCharsPatterns = "";
            queryRichTextBox.BackBrush = null;
            queryRichTextBox.CharHeight = 16;
            queryRichTextBox.CharWidth = 9;
            mainTableLayoutPanel.SetColumnSpan(queryRichTextBox, 2);
            queryRichTextBox.CommentPrefix = "--";
            queryRichTextBox.ContextMenuStrip = queryEditorContextMenuStrip;
            queryRichTextBox.DisabledColor = Color.FromArgb(100, 180, 180, 180);
            queryRichTextBox.Hotkeys = resources.GetString("queryRichTextBox.Hotkeys");
            queryRichTextBox.IsReplaceMode = false;
            queryRichTextBox.Language = Language.SQL;
            queryRichTextBox.LeftBracket = '(';
            queryRichTextBox.Name = "queryRichTextBox";
            queryRichTextBox.Paddings = new Padding(0);
            queryRichTextBox.RightBracket = ')';
            queryRichTextBox.SelectionColor = Color.FromArgb(60, 0, 0, 255);
            queryRichTextBox.ServiceColors = null;
            queryRichTextBox.Zoom = 100;
            queryRichTextBox.TextChanged += queryRichTextBox_TextChanged;
            queryRichTextBox.KeyDown += queryRichTextBox_KeyDown;
            // 
            // queryEditorContextMenuStrip
            // 
            queryEditorContextMenuStrip.Items.AddRange(new ToolStripItem[] { copyTextMenuItem, pasteTextMenuItem });
            queryEditorContextMenuStrip.Name = "queryEditorContextMenuStrip";
            resources.ApplyResources(queryEditorContextMenuStrip, "queryEditorContextMenuStrip");
            // 
            // copyTextMenuItem
            // 
            resources.ApplyResources(copyTextMenuItem, "copyTextMenuItem");
            copyTextMenuItem.Name = "copyTextMenuItem";
            copyTextMenuItem.Click += copyTextMenuItem_Click;
            // 
            // pasteTextMenuItem
            // 
            pasteTextMenuItem.Image = Properties.Resources.paste;
            pasteTextMenuItem.Name = "pasteTextMenuItem";
            resources.ApplyResources(pasteTextMenuItem, "pasteTextMenuItem");
            pasteTextMenuItem.Click += pasteTextMenuItem_Click;
            // 
            // executeQueryButton
            // 
            resources.ApplyResources(executeQueryButton, "executeQueryButton");
            executeQueryButton.Image = Properties.Resources.exclamation;
            executeQueryButton.Name = "executeQueryButton";
            executeQueryButton.UseVisualStyleBackColor = true;
            executeQueryButton.Click += executeQueryButton_Click;
            // 
            // querySyntaxDocsButton
            // 
            resources.ApplyResources(querySyntaxDocsButton, "querySyntaxDocsButton");
            querySyntaxDocsButton.Image = Properties.Resources.external_link;
            querySyntaxDocsButton.Name = "querySyntaxDocsButton";
            querySyntaxDocsButton.UseVisualStyleBackColor = true;
            querySyntaxDocsButton.Click += querySyntaxDocsButton_Click;
            // 
            // resultsGridView
            // 
            resultsGridView.AllowUserToAddRows = false;
            resultsGridView.AllowUserToDeleteRows = false;
            resultsGridView.AllowUserToOrderColumns = true;
            resultsGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            resultsGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            resultsGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resultsGridView.ColumnNameEscapeFormat = "[{0}]";
            resultsGridView.CopyAsWhereIcon = null;
            resultsGridView.CopyToClipboardIcon = null;
            resultsGridView.DateValueEscapeFormat = "#{0}#";
            resources.ApplyResources(resultsGridView, "resultsGridView");
            resultsGridView.EnableHeadersVisualStyles = false;
            resultsGridView.Name = "resultsGridView";
            resultsGridView.ReadOnly = true;
            resultsGridView.ShowCellToolTips = false;
            resultsGridView.ShowCopyAsWhereContextMenuItem = false;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, showingCountLabel, toolStripStatusLabel2, toolStripStatusLabel3, zoomPercentageDropDown, queryExecutionStatusLabel, timeElapsedLabel });
            resources.ApplyResources(statusStrip, "statusStrip");
            statusStrip.Name = "statusStrip";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            resources.ApplyResources(toolStripStatusLabel1, "toolStripStatusLabel1");
            // 
            // showingCountLabel
            // 
            resources.ApplyResources(showingCountLabel, "showingCountLabel");
            showingCountLabel.Name = "showingCountLabel";
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            resources.ApplyResources(toolStripStatusLabel2, "toolStripStatusLabel2");
            // 
            // toolStripStatusLabel3
            // 
            toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            resources.ApplyResources(toolStripStatusLabel3, "toolStripStatusLabel3");
            toolStripStatusLabel3.Spring = true;
            // 
            // zoomPercentageDropDown
            // 
            zoomPercentageDropDown.DisplayStyle = ToolStripItemDisplayStyle.Text;
            zoomPercentageDropDown.DropDownItems.AddRange(new ToolStripItem[] { percent100, percent110, percent125, percent140, percent150 });
            resources.ApplyResources(zoomPercentageDropDown, "zoomPercentageDropDown");
            zoomPercentageDropDown.Name = "zoomPercentageDropDown";
            // 
            // percent100
            // 
            percent100.Checked = true;
            percent100.CheckState = CheckState.Checked;
            percent100.Name = "percent100";
            resources.ApplyResources(percent100, "percent100");
            percent100.Tag = "100";
            percent100.Click += zoomPercentage_Click;
            // 
            // percent110
            // 
            percent110.Name = "percent110";
            resources.ApplyResources(percent110, "percent110");
            percent110.Tag = "110";
            percent110.Click += zoomPercentage_Click;
            // 
            // percent125
            // 
            percent125.Name = "percent125";
            resources.ApplyResources(percent125, "percent125");
            percent125.Tag = "125";
            percent125.Click += zoomPercentage_Click;
            // 
            // percent140
            // 
            percent140.Name = "percent140";
            resources.ApplyResources(percent140, "percent140");
            percent140.Tag = "140";
            percent140.Click += zoomPercentage_Click;
            // 
            // percent150
            // 
            percent150.Name = "percent150";
            resources.ApplyResources(percent150, "percent150");
            percent150.Tag = "150";
            percent150.Click += zoomPercentage_Click;
            // 
            // queryExecutionStatusLabel
            // 
            queryExecutionStatusLabel.Name = "queryExecutionStatusLabel";
            resources.ApplyResources(queryExecutionStatusLabel, "queryExecutionStatusLabel");
            // 
            // timeElapsedLabel
            // 
            resources.ApplyResources(timeElapsedLabel, "timeElapsedLabel");
            timeElapsedLabel.Name = "timeElapsedLabel";
            // 
            // closeButton
            // 
            resources.ApplyResources(closeButton, "closeButton");
            closeButton.Name = "closeButton";
            closeButton.UseVisualStyleBackColor = true;
            closeButton.Click += closeButton_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, helpToolStripMenuItem });
            resources.ApplyResources(menuStrip1, "menuStrip1");
            menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, fileOpenToolStripMenuItem, openFolderToolStripMenuItem, toolStripSeparator2, saveResultsAsToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Image = Properties.Resources.blank_page;
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            resources.ApplyResources(newToolStripMenuItem, "newToolStripMenuItem");
            newToolStripMenuItem.Click += newToolStripMenuItem_Click;
            // 
            // fileOpenToolStripMenuItem
            // 
            fileOpenToolStripMenuItem.Image = Properties.Resources.open_folder;
            fileOpenToolStripMenuItem.Name = "fileOpenToolStripMenuItem";
            resources.ApplyResources(fileOpenToolStripMenuItem, "fileOpenToolStripMenuItem");
            fileOpenToolStripMenuItem.Click += fileOpenToolStripMenuItem_Click;
            // 
            // openFolderToolStripMenuItem
            // 
            openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            resources.ApplyResources(openFolderToolStripMenuItem, "openFolderToolStripMenuItem");
            openFolderToolStripMenuItem.Click += openFolderToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
            // 
            // saveResultsAsToolStripMenuItem
            // 
            resources.ApplyResources(saveResultsAsToolStripMenuItem, "saveResultsAsToolStripMenuItem");
            saveResultsAsToolStripMenuItem.Image = Properties.Resources.save_btn;
            saveResultsAsToolStripMenuItem.Name = "saveResultsAsToolStripMenuItem";
            saveResultsAsToolStripMenuItem.Click += saveResultsAsToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(exitToolStripMenuItem, "exitToolStripMenuItem");
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { dateFormatToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            resources.ApplyResources(editToolStripMenuItem, "editToolStripMenuItem");
            // 
            // dateFormatToolStripMenuItem
            // 
            dateFormatToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { defaultToolStripMenuItem, iSO8601ToolStripMenuItem, customToolStripMenuItem });
            dateFormatToolStripMenuItem.Image = Properties.Resources.calendar;
            dateFormatToolStripMenuItem.Name = "dateFormatToolStripMenuItem";
            resources.ApplyResources(dateFormatToolStripMenuItem, "dateFormatToolStripMenuItem");
            // 
            // defaultToolStripMenuItem
            // 
            defaultToolStripMenuItem.Checked = true;
            defaultToolStripMenuItem.CheckState = CheckState.Checked;
            defaultToolStripMenuItem.Name = "defaultToolStripMenuItem";
            resources.ApplyResources(defaultToolStripMenuItem, "defaultToolStripMenuItem");
            defaultToolStripMenuItem.Tag = "0";
            defaultToolStripMenuItem.Click += dateFormatToolStripMenuItem_Click;
            // 
            // iSO8601ToolStripMenuItem
            // 
            iSO8601ToolStripMenuItem.Name = "iSO8601ToolStripMenuItem";
            resources.ApplyResources(iSO8601ToolStripMenuItem, "iSO8601ToolStripMenuItem");
            iSO8601ToolStripMenuItem.Tag = "1";
            iSO8601ToolStripMenuItem.Click += dateFormatToolStripMenuItem_Click;
            // 
            // customToolStripMenuItem
            // 
            customToolStripMenuItem.Name = "customToolStripMenuItem";
            resources.ApplyResources(customToolStripMenuItem, "customToolStripMenuItem");
            customToolStripMenuItem.Tag = "2";
            customToolStripMenuItem.Click += dateFormatToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { userGuideToolStripMenuItem, shareUsageDataToolStripMenuItem, languageToolStripMenuItem1, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // userGuideToolStripMenuItem
            // 
            userGuideToolStripMenuItem.Image = Properties.Resources.external_link;
            userGuideToolStripMenuItem.Name = "userGuideToolStripMenuItem";
            resources.ApplyResources(userGuideToolStripMenuItem, "userGuideToolStripMenuItem");
            userGuideToolStripMenuItem.Click += userGuideToolStripMenuItem_Click;
            // 
            // shareUsageDataToolStripMenuItem
            // 
            shareUsageDataToolStripMenuItem.Name = "shareUsageDataToolStripMenuItem";
            resources.ApplyResources(shareUsageDataToolStripMenuItem, "shareUsageDataToolStripMenuItem");
            shareUsageDataToolStripMenuItem.Click += shareUsageDataToolStripMenuItem_Click;
            // 
            // languageToolStripMenuItem1
            // 
            languageToolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { englishToolStripMenuItem, türkçeToolStripMenuItem });
            languageToolStripMenuItem1.Image = Properties.Resources.localization;
            languageToolStripMenuItem1.Name = "languageToolStripMenuItem1";
            resources.ApplyResources(languageToolStripMenuItem1, "languageToolStripMenuItem1");
            // 
            // englishToolStripMenuItem
            // 
            englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            resources.ApplyResources(englishToolStripMenuItem, "englishToolStripMenuItem");
            englishToolStripMenuItem.Tag = "en-US";
            englishToolStripMenuItem.Click += languageToolStripMenuItem_Click;
            // 
            // türkçeToolStripMenuItem
            // 
            türkçeToolStripMenuItem.Name = "türkçeToolStripMenuItem";
            resources.ApplyResources(türkçeToolStripMenuItem, "türkçeToolStripMenuItem");
            türkçeToolStripMenuItem.Tag = "tr-TR";
            türkçeToolStripMenuItem.Click += languageToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(aboutToolStripMenuItem, "aboutToolStripMenuItem");
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            AcceptButton = executeQueryButton;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = closeButton;
            Controls.Add(closeButton);
            Controls.Add(mainSplitContainer);
            Controls.Add(menuStrip1);
            Controls.Add(statusStrip);
            Icon = Properties.Resources.ducksql_logo_transparent_icon;
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Load += QueryEditor_Load;
            KeyUp += QueryEditor_KeyUp;
            mainSplitContainer.Panel1.ResumeLayout(false);
            mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)mainSplitContainer).EndInit();
            mainSplitContainer.ResumeLayout(false);
            mainTableLayoutPanel.ResumeLayout(false);
            mainTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)queryRichTextBox).EndInit();
            queryEditorContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)resultsGridView).EndInit();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainTableLayoutPanel;
        private FastColoredTextBox queryRichTextBox;
        private System.Windows.Forms.Button executeQueryButton;
        private Controls.ParquetGridView resultsGridView;
        private SplitContainer mainSplitContainer;
        private Button querySyntaxDocsButton;
        private Button closeButton;
        private ToolTip executeQueryKeyboardShortcutToolTip;
        private ContextMenuStrip queryEditorContextMenuStrip;
        private ToolStripMenuItem copyTextMenuItem;
        private ToolStripMenuItem pasteTextMenuItem;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel showingCountLabel;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private ToolStripStatusLabel toolStripStatusLabel3;
        private ToolStripDropDownButton zoomPercentageDropDown;
        private ToolStripMenuItem percent100;
        private ToolStripMenuItem percent110;
        private ToolStripMenuItem percent125;
        private ToolStripMenuItem percent140;
        private ToolStripMenuItem percent150;
        private ToolStripStatusLabel queryExecutionStatusLabel;
        private ToolStripStatusLabel timeElapsedLabel;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem fileOpenToolStripMenuItem;
        private ToolStripMenuItem openFolderToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem saveResultsAsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem dateFormatToolStripMenuItem;
        private ToolStripMenuItem defaultToolStripMenuItem;
        private ToolStripMenuItem iSO8601ToolStripMenuItem;
        private ToolStripMenuItem customToolStripMenuItem;
        private ToolStripMenuItem userGuideToolStripMenuItem;
        private ToolStripMenuItem languageToolStripMenuItem1;
        private ToolStripMenuItem englishToolStripMenuItem;
        private ToolStripMenuItem türkçeToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem shareUsageDataToolStripMenuItem;
    }
}
