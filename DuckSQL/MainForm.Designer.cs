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
            queryEditorContextMenuStrip = new ContextMenuStrip(components);
            copyTextMenuItem = new ToolStripMenuItem();
            pasteTextMenuItem = new ToolStripMenuItem();
            executeQueryKeyboardShortcutToolTip = new ToolTip(components);
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newQueryToolStripMenuItem = new ToolStripMenuItem();
            openQueryToolStripMenuItem = new ToolStripMenuItem();
            saveQueryToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            fileOpenToolStripMenuItem = new ToolStripMenuItem();
            openFolderToolStripMenuItem = new ToolStripMenuItem();
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
            tabPage1 = new TabPage();
            queryEditorControl1 = new QueryEditorControl();
            tabControl1 = new CustomTabControl();
            tabControlImageList = new ImageList(components);
            openFileDialog = new OpenFileDialog();
            openFolderDialog = new FolderBrowserDialog();
            exportFileDialog = new SaveFileDialog();
            openQueryDialog = new OpenFileDialog();
            saveQueryDialog = new SaveFileDialog();
            queryEditorContextMenuStrip.SuspendLayout();
            menuStrip1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabControl1.SuspendLayout();
            SuspendLayout();
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
            // 
            // pasteTextMenuItem
            // 
            pasteTextMenuItem.Image = Properties.Resources.paste;
            pasteTextMenuItem.Name = "pasteTextMenuItem";
            resources.ApplyResources(pasteTextMenuItem, "pasteTextMenuItem");
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = SystemColors.Control;
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, helpToolStripMenuItem });
            resources.ApplyResources(menuStrip1, "menuStrip1");
            menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newQueryToolStripMenuItem, openQueryToolStripMenuItem, saveQueryToolStripMenuItem, toolStripSeparator2, fileOpenToolStripMenuItem, openFolderToolStripMenuItem, saveResultsAsToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // newQueryToolStripMenuItem
            // 
            newQueryToolStripMenuItem.Image = Properties.Resources.blank_page;
            newQueryToolStripMenuItem.Name = "newQueryToolStripMenuItem";
            resources.ApplyResources(newQueryToolStripMenuItem, "newQueryToolStripMenuItem");
            newQueryToolStripMenuItem.Click += newToolStripMenuItem_Click;
            // 
            // openQueryToolStripMenuItem
            // 
            openQueryToolStripMenuItem.Image = Properties.Resources.open_file;
            openQueryToolStripMenuItem.Name = "openQueryToolStripMenuItem";
            resources.ApplyResources(openQueryToolStripMenuItem, "openQueryToolStripMenuItem");
            openQueryToolStripMenuItem.Click += openQueryToolStripMenuItem_Click;
            // 
            // saveQueryToolStripMenuItem
            // 
            resources.ApplyResources(saveQueryToolStripMenuItem, "saveQueryToolStripMenuItem");
            saveQueryToolStripMenuItem.Image = Properties.Resources.save_btn;
            saveQueryToolStripMenuItem.Name = "saveQueryToolStripMenuItem";
            saveQueryToolStripMenuItem.Click += saveQueryToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
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
            // saveResultsAsToolStripMenuItem
            // 
            resources.ApplyResources(saveResultsAsToolStripMenuItem, "saveResultsAsToolStripMenuItem");
            saveResultsAsToolStripMenuItem.Image = Properties.Resources.file_export;
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
            // tabPage1
            // 
            tabPage1.Controls.Add(queryEditorControl1);
            resources.ApplyResources(tabPage1, "tabPage1");
            tabPage1.Name = "tabPage1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // queryEditorControl1
            // 
            queryEditorControl1.AllowDrop = true;
            resources.ApplyResources(queryEditorControl1, "queryEditorControl1");
            queryEditorControl1.Name = "queryEditorControl1";
            queryEditorControl1.QueryExecuted = null;
            queryEditorControl1.QueryExecutionStarted = null;
            queryEditorControl1.QueryText = "";
            queryEditorControl1.QueryTextChanged = null;
            // 
            // tabControl1
            // 
            tabControl1.AllowDrop = true;
            tabControl1.Controls.Add(tabPage1);
            resources.ApplyResources(tabControl1, "tabControl1");
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.ImageList = tabControlImageList;
            tabControl1.Multiline = true;
            tabControl1.Name = "tabControl1";
            tabControl1.QueryExecuted = null;
            tabControl1.QueryExecutionStarted = null;
            tabControl1.SelectedIndex = 0;
            tabControl1.TabDirtyStateChanged = null;
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            // 
            // tabControlImageList
            // 
            tabControlImageList.ColorDepth = ColorDepth.Depth32Bit;
            tabControlImageList.ImageStream = (ImageListStreamer)resources.GetObject("tabControlImageList.ImageStream");
            tabControlImageList.TransparentColor = Color.Transparent;
            tabControlImageList.Images.SetKeyName(0, "close.png");
            // 
            // openFileDialog
            // 
            resources.ApplyResources(openFileDialog, "openFileDialog");
            // 
            // openFolderDialog
            // 
            resources.ApplyResources(openFolderDialog, "openFolderDialog");
            openFolderDialog.ShowNewFolderButton = false;
            // 
            // openQueryDialog
            // 
            resources.ApplyResources(openQueryDialog, "openQueryDialog");
            // 
            // saveQueryDialog
            // 
            resources.ApplyResources(saveQueryDialog, "saveQueryDialog");
            // 
            // MainForm
            // 
            AllowDrop = true;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabControl1);
            Controls.Add(menuStrip1);
            Icon = Properties.Resources.ducksql_logo_transparent_icon;
            KeyPreview = true;
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Load += MainForm_Load;
            DragDrop += MainForm_DragDrop;
            DragEnter += MainForm_DragEnter;
            KeyDown += MainForm_KeyDown;
            MouseDoubleClick += MainForm_MouseDoubleClick;
            queryEditorContextMenuStrip.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            tabPage1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ToolTip executeQueryKeyboardShortcutToolTip;
        private ContextMenuStrip queryEditorContextMenuStrip;
        private ToolStripMenuItem copyTextMenuItem;
        private ToolStripMenuItem pasteTextMenuItem;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem fileOpenToolStripMenuItem;
        private ToolStripMenuItem openFolderToolStripMenuItem;
        private ToolStripMenuItem newQueryToolStripMenuItem;
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
        private TabPage tabPage1;
        private CustomTabControl tabControl1;
        private QueryEditorControl queryEditorControl1;
        private OpenFileDialog openFileDialog;
        private FolderBrowserDialog openFolderDialog;
        private ImageList tabControlImageList;
        private SaveFileDialog exportFileDialog;
        private ToolStripMenuItem saveQueryToolStripMenuItem;
        private ToolStripMenuItem openQueryToolStripMenuItem;
        private OpenFileDialog openQueryDialog;
        private SaveFileDialog saveQueryDialog;
    }
}
