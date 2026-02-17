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

            //Dynamically set label width
            using var gfx = CreateGraphics();
            var width = MeasureStringWidth(gfx, Font, title, false);
            //width += 24; //Add some buffer for the close button
            width = Math.Clamp(width, 20, 120);
            Width = width + 200;
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
    }
}
