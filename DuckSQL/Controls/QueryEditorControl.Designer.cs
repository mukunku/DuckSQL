using FastColoredTextBoxNS;

namespace DuckSQL.Controls
{
    partial class QueryEditorControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryEditorControl));
            mainSplitContainer = new GripSplitContainer();
            mainTableLayoutPanel = new TableLayoutPanel();
            queryRichTextBox = new FastColoredTextBox();
            queryEditorContextMenuStrip = new ContextMenuStrip(components);
            copyTextMenuItem = new ToolStripMenuItem();
            pasteTextMenuItem = new ToolStripMenuItem();
            executeQueryButton = new Button();
            cancelQueryButton = new Button();
            resultsGridView = new ParquetGridView();
            statusStrip = new FormSizingStatusStrip();
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
            executeQueryButtonTooltip = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)mainSplitContainer).BeginInit();
            mainSplitContainer.Panel1.SuspendLayout();
            mainSplitContainer.Panel2.SuspendLayout();
            mainSplitContainer.SuspendLayout();
            mainTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)queryRichTextBox).BeginInit();
            queryEditorContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)resultsGridView).BeginInit();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // mainSplitContainer
            // 
            mainSplitContainer.BackColor = SystemColors.Control;
            mainSplitContainer.BorderStyle = BorderStyle.FixedSingle;
            mainSplitContainer.Dock = DockStyle.Fill;
            mainSplitContainer.GripBackColor = Color.FromArgb(240, 240, 240);
            mainSplitContainer.GripBorderColor = Color.Gray;
            mainSplitContainer.GripDotColor = Color.DimGray;
            mainSplitContainer.Location = new Point(0, 0);
            mainSplitContainer.Name = "mainSplitContainer";
            mainSplitContainer.Orientation = Orientation.Horizontal;
            // 
            // mainSplitContainer.Panel1
            // 
            mainSplitContainer.Panel1.Controls.Add(mainTableLayoutPanel);
            // 
            // mainSplitContainer.Panel2
            // 
            mainSplitContainer.Panel2.BackColor = SystemColors.Control;
            mainSplitContainer.Panel2.Controls.Add(resultsGridView);
            mainSplitContainer.Panel2.Controls.Add(statusStrip);
            mainSplitContainer.Size = new Size(800, 600);
            mainSplitContainer.SplitterColor = Color.Gray;
            mainSplitContainer.SplitterDistance = 200;
            mainSplitContainer.SplitterWidth = 12;
            mainSplitContainer.TabIndex = 0;
            // 
            // mainTableLayoutPanel
            // 
            mainTableLayoutPanel.ColumnCount = 2;
            mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            mainTableLayoutPanel.Controls.Add(queryRichTextBox, 0, 0);
            mainTableLayoutPanel.Controls.Add(executeQueryButton, 0, 1);
            mainTableLayoutPanel.Controls.Add(cancelQueryButton, 1, 1);
            mainTableLayoutPanel.Dock = DockStyle.Fill;
            mainTableLayoutPanel.Location = new Point(0, 0);
            mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            mainTableLayoutPanel.RowCount = 2;
            mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            mainTableLayoutPanel.Size = new Size(798, 198);
            mainTableLayoutPanel.TabIndex = 0;
            // 
            // queryRichTextBox
            // 
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
            queryRichTextBox.AutoScrollMinSize = new Size(29, 16);
            queryRichTextBox.BackBrush = null;
            queryRichTextBox.CharHeight = 16;
            queryRichTextBox.CharWidth = 9;
            mainTableLayoutPanel.SetColumnSpan(queryRichTextBox, 2);
            queryRichTextBox.CommentPrefix = "--";
            queryRichTextBox.ContextMenuStrip = queryEditorContextMenuStrip;
            queryRichTextBox.DisabledColor = Color.FromArgb(100, 180, 180, 180);
            queryRichTextBox.Dock = DockStyle.Fill;
            queryRichTextBox.Font = new Font("Courier New", 11.25F);
            queryRichTextBox.Hotkeys = resources.GetString("queryRichTextBox.Hotkeys");
            queryRichTextBox.IsReplaceMode = false;
            queryRichTextBox.Language = Language.SQL;
            queryRichTextBox.LeftBracket = '(';
            queryRichTextBox.Location = new Point(3, 3);
            queryRichTextBox.Name = "queryRichTextBox";
            queryRichTextBox.Paddings = new Padding(0);
            queryRichTextBox.RightBracket = ')';
            queryRichTextBox.SelectionColor = Color.FromArgb(60, 0, 0, 255);
            queryRichTextBox.ServiceColors = null;
            queryRichTextBox.Size = new Size(792, 157);
            queryRichTextBox.TabIndex = 0;
            queryRichTextBox.Zoom = 100;
            queryRichTextBox.TextChanged += queryRichTextBox_TextChanged;
            queryRichTextBox.KeyDown += queryRichTextBox_KeyDown;
            // 
            // queryEditorContextMenuStrip
            // 
            queryEditorContextMenuStrip.Items.AddRange(new ToolStripItem[] { copyTextMenuItem, pasteTextMenuItem });
            queryEditorContextMenuStrip.Name = "queryEditorContextMenuStrip";
            queryEditorContextMenuStrip.Size = new Size(103, 48);
            // 
            // copyTextMenuItem
            // 
            copyTextMenuItem.Name = "copyTextMenuItem";
            copyTextMenuItem.Size = new Size(102, 22);
            copyTextMenuItem.Text = "Copy";
            copyTextMenuItem.Click += copyTextMenuItem_Click;
            // 
            // pasteTextMenuItem
            // 
            pasteTextMenuItem.Image = Properties.Resources.paste;
            pasteTextMenuItem.Name = "pasteTextMenuItem";
            pasteTextMenuItem.Size = new Size(102, 22);
            pasteTextMenuItem.Text = "Paste";
            pasteTextMenuItem.Click += pasteTextMenuItem_Click;
            // 
            // executeQueryButton
            // 
            executeQueryButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            executeQueryButton.Image = Properties.Resources.exclamation;
            executeQueryButton.Location = new Point(3, 166);
            executeQueryButton.Name = "executeQueryButton";
            executeQueryButton.Size = new Size(672, 29);
            executeQueryButton.TabIndex = 1;
            executeQueryButton.Text = "Execute Query";
            executeQueryButton.TextAlign = ContentAlignment.MiddleRight;
            executeQueryButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            executeQueryButtonTooltip.SetToolTip(executeQueryButton, "Shortcuts: F5, SHIFT + Enter");
            executeQueryButton.UseVisualStyleBackColor = true;
            executeQueryButton.Click += executeQueryButton_Click;
            // 
            // cancelQueryButton
            // 
            cancelQueryButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cancelQueryButton.Enabled = false;
            cancelQueryButton.Location = new Point(681, 166);
            cancelQueryButton.Name = "cancelQueryButton";
            cancelQueryButton.Size = new Size(114, 29);
            cancelQueryButton.TabIndex = 2;
            cancelQueryButton.Text = "Cancel";
            executeQueryButtonTooltip.SetToolTip(cancelQueryButton, "Shortcut: Escape");
            cancelQueryButton.UseVisualStyleBackColor = true;
            cancelQueryButton.Click += cancelQueryButton_Click;
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
            resultsGridView.Dock = DockStyle.Fill;
            resultsGridView.EnableHeadersVisualStyles = false;
            resultsGridView.Location = new Point(0, 0);
            resultsGridView.Name = "resultsGridView";
            resultsGridView.ReadOnly = true;
            resultsGridView.RowHeadersWidth = 24;
            resultsGridView.ShowCellToolTips = false;
            resultsGridView.ShowCopyAsWhereContextMenuItem = false;
            resultsGridView.Size = new Size(798, 364);
            resultsGridView.TabIndex = 0;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, showingCountLabel, toolStripStatusLabel2, toolStripStatusLabel3, zoomPercentageDropDown, queryExecutionStatusLabel, timeElapsedLabel });
            statusStrip.Location = new Point(0, 364);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(798, 22);
            statusStrip.TabIndex = 2;
            statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(53, 17);
            toolStripStatusLabel1.Text = "Showing";
            // 
            // showingCountLabel
            // 
            showingCountLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            showingCountLabel.Name = "showingCountLabel";
            showingCountLabel.Size = new Size(14, 17);
            showingCountLabel.Text = "0";
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(44, 17);
            toolStripStatusLabel2.Text = "Results";
            // 
            // toolStripStatusLabel3
            // 
            toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            toolStripStatusLabel3.Size = new Size(446, 17);
            toolStripStatusLabel3.Spring = true;
            // 
            // zoomPercentageDropDown
            // 
            zoomPercentageDropDown.DisplayStyle = ToolStripItemDisplayStyle.Text;
            zoomPercentageDropDown.DropDownItems.AddRange(new ToolStripItem[] { percent100, percent110, percent125, percent140, percent150 });
            zoomPercentageDropDown.Image = (Image)resources.GetObject("zoomPercentageDropDown.Image");
            zoomPercentageDropDown.ImageTransparentColor = Color.Magenta;
            zoomPercentageDropDown.Name = "zoomPercentageDropDown";
            zoomPercentageDropDown.Size = new Size(121, 20);
            zoomPercentageDropDown.Text = "Query Zoom: 100%";
            // 
            // percent100
            // 
            percent100.Checked = true;
            percent100.CheckState = CheckState.Checked;
            percent100.Name = "percent100";
            percent100.Size = new Size(102, 22);
            percent100.Tag = "100";
            percent100.Text = "100%";
            percent100.Click += zoomPercentage_Click;
            // 
            // percent110
            // 
            percent110.Name = "percent110";
            percent110.Size = new Size(102, 22);
            percent110.Tag = "110";
            percent110.Text = "110%";
            percent110.Click += zoomPercentage_Click;
            // 
            // percent125
            // 
            percent125.Name = "percent125";
            percent125.Size = new Size(102, 22);
            percent125.Tag = "125";
            percent125.Text = "125%";
            percent125.Click += zoomPercentage_Click;
            // 
            // percent140
            // 
            percent140.Name = "percent140";
            percent140.Size = new Size(102, 22);
            percent140.Tag = "140";
            percent140.Text = "140%";
            percent140.Click += zoomPercentage_Click;
            // 
            // percent150
            // 
            percent150.Name = "percent150";
            percent150.Size = new Size(102, 22);
            percent150.Tag = "150";
            percent150.Text = "150%";
            percent150.Click += zoomPercentage_Click;
            // 
            // queryExecutionStatusLabel
            // 
            queryExecutionStatusLabel.Name = "queryExecutionStatusLabel";
            queryExecutionStatusLabel.Size = new Size(67, 17);
            queryExecutionStatusLabel.Text = "Finished in:";
            // 
            // timeElapsedLabel
            // 
            timeElapsedLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            timeElapsedLabel.Name = "timeElapsedLabel";
            timeElapsedLabel.Size = new Size(38, 17);
            timeElapsedLabel.Text = "00:00";
            // 
            // executeQueryButtonTooltip
            // 
            executeQueryButtonTooltip.AutoPopDelay = 5000;
            executeQueryButtonTooltip.InitialDelay = 350;
            executeQueryButtonTooltip.ReshowDelay = 100;
            // 
            // QueryEditorControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(mainSplitContainer);
            Name = "QueryEditorControl";
            Size = new Size(800, 600);
            Load += QueryEditorControl_Load;
            Resize += QueryEditorControl_Resize;
            mainSplitContainer.Panel1.ResumeLayout(false);
            mainSplitContainer.Panel2.ResumeLayout(false);
            mainSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)mainSplitContainer).EndInit();
            mainSplitContainer.ResumeLayout(false);
            mainTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)queryRichTextBox).EndInit();
            queryEditorContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)resultsGridView).EndInit();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GripSplitContainer mainSplitContainer;
        private TableLayoutPanel mainTableLayoutPanel;
        private FastColoredTextBox queryRichTextBox;
        private Button executeQueryButton;
        private ParquetGridView resultsGridView;
        private ContextMenuStrip queryEditorContextMenuStrip;
        private ToolStripMenuItem copyTextMenuItem;
        private ToolStripMenuItem pasteTextMenuItem;
        private ToolTip executeQueryButtonTooltip;
        private FormSizingStatusStrip statusStrip;
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
        private Button cancelQueryButton;
    }
}