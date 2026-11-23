using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace DuckSQL.Controls
{
    internal class CustomTabPage : TabPage
    {
        public enum TabKind { Scratch, SqlFile }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabKind Kind { get; private set; }

        /// <summary>Path of the .sql file backing this tab. Only set for <see cref="TabKind.SqlFile"/> tabs.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? FilePath { get; private set; }

        /// <summary>The "N" in "New query N". Only meaningful for <see cref="TabKind.Scratch"/> tabs.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ScratchNumber { get; private set; }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDirty { get; private set; }

        public event EventHandler? DirtyStateChanged;
        public event EventHandler? QueryExecutionStarted;
        public event EventHandler<QueryEditorControl.QueryExecutedEventArgs>? QueryExecuted;

        //Text to load into the query editor once it's lazily created
        private string? _initialQueryText;

        [AllowNull]
        public override string Text
        {
            get => base.Text;
            set => SetTabText(value ?? string.Empty);
        }

        private CustomTabPage(TabKind kind, string? filePath, int scratchNumber, string? initialQueryText, bool isDirty)
        {
            this.Font = new Font("Segoe UI", 9);
            Kind = kind;
            FilePath = filePath;
            ScratchNumber = scratchNumber;
            _initialQueryText = initialQueryText;
            IsDirty = isDirty;
            UpdateTitle();
        }

        public static CustomTabPage ForScratch(int scratchNumber, string? queryText = null)
            => new(TabKind.Scratch, null, scratchNumber, queryText, !string.IsNullOrWhiteSpace(queryText));

        /// <param name="unsavedText">Unsaved edits to show instead of the file's contents</param>
        public static CustomTabPage ForSqlFile(string filePath, string? unsavedText = null)
            => new(TabKind.SqlFile, filePath, 0, unsavedText, unsavedText is not null);

        public bool IsFileBackedTab => Kind == TabKind.SqlFile;

        /// <summary>The editor's text, or the text it will be created with if it hasn't been created yet.</summary>
        public string CurrentText => HasQueryEditor() ? GetQueryEditor().QueryText : _initialQueryText ?? string.Empty;

        public QueryEditorControl GetQueryEditor()
            => this.Controls[0] is QueryEditorControl editor
            ? editor : throw new InvalidOperationException($"No query editor found");

        public bool HasQueryEditor()
            => this.Controls.Count > 0 && this.Controls[0] is QueryEditorControl;

        public QueryEditorControl GetOrCreateQueryEditor()
        {
            if (HasQueryEditor())
                return GetQueryEditor();

            var editor = IsFileBackedTab && _initialQueryText is null && File.Exists(FilePath)
                ? new QueryEditorControl(FilePath)
                : new QueryEditorControl { QueryText = _initialQueryText ?? string.Empty };
            editor.Dock = DockStyle.Fill;
            _initialQueryText = null;

            //File backed tabs become dirty on any change. Scratch tabs are dirty as long as they have text.
            editor.QueryTextChanged += (s, e) => SetDirty(IsFileBackedTab || !string.IsNullOrWhiteSpace(editor.QueryText));

            editor.QueryExecuted += (s, e) => QueryExecuted?.Invoke(s, e);
            editor.QueryExecutionStarted += (s, e) => QueryExecutionStarted?.Invoke(s, e);
            editor.DragDrop += (s, e) => OnDragDrop(e);

            Controls.Add(editor);
            return editor;
        }

        public void SetDirty(bool isDirty)
        {
            if (IsDirty == isDirty)
                return;

            IsDirty = isDirty;
            UpdateTitle();
            DirtyStateChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Marks the tab as saved to <paramref name="filePath"/>, turning scratch tabs into file backed ones.</summary>
        public void MarkSaved(string filePath)
        {
            Kind = TabKind.SqlFile;
            FilePath = filePath;
            UpdateTitle(); //Update the title here too just in case
            SetDirty(false);
        }

        private void UpdateTitle()
        {
            var title = Kind == TabKind.SqlFile
                ? Path.GetFileName(FilePath)
                : ScratchNumber > 1 ? $"{Resources.Strings.NewQueryTabTitle} {ScratchNumber}" : Resources.Strings.NewQueryTabTitle;

            Text = IsDirty ? $"{title} *" : title;
            ToolTipText = FilePath ?? string.Empty;
        }

        private void SetTabText(string title)
        {
            base.Text = title;

            //add padding for the close button
            base.Text += new string(' ', 6); //magic number
        }
    }
}
