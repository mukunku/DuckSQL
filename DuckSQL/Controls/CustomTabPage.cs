using System.Diagnostics.CodeAnalysis;

namespace DuckSQL.Controls
{
    internal class CustomTabPage : TabPage
    {
        [AllowNull]
        public override string Text
        {
            get => base.Text;
            set => SetTabText(value ?? string.Empty);
        }

        public CustomTabPage()
        {
            this.Font = new Font("Segoe UI", 9);
        }

        public bool HasNewQueryTitle => Text.StartsWith(Resources.Strings.NewQueryTabTitle);

        public bool IsFileBackedTab => !HasNewQueryTitle;

        public bool IsDirty => Text.Trim().EndsWith('*') == true;

        public QueryEditorControl GetQueryEditor()
            => this.Controls[0] is QueryEditorControl editor
            ? editor : throw new InvalidOperationException($"No query editor found");

        public bool HasQueryEditor()
            => this.Controls.Count > 0 && this.Controls[0] is QueryEditorControl;

        private void SetTabText(string title)
        {
            base.Text = title;

            //add padding for the close button
            base.Text += new string(' ', 6); //magic number
        }
    }
}
