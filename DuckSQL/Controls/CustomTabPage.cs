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

        private void SetTabText(string title)
        {
            base.Text = title;

            //add padding for the close button
            base.Text += new string(' ', 6); //magic number
        }
    }
}
