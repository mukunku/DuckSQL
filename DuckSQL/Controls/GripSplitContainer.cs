using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace DuckSQL.Controls
{
    internal class GripSplitContainer : SplitContainer
    {
        private const int GRIP_WIDTH = 28;
        private const int GRIP_HEIGHT = 9;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color SplitterColor { get; set; } = Color.Gray;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color GripBackColor { get; set; } = Color.FromArgb(240, 240, 240);
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color GripBorderColor { get; set; } = Color.Gray;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color GripDotColor { get; set; } = Color.DimGray;

        public GripSplitContainer(int splitterWidth = 14)
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);

            SplitterWidth = splitterWidth;

            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var splitter = SplitterRectangle;

            using var splitterPen = new Pen(SplitterColor);
            using var gripBrush = new SolidBrush(GripBackColor);
            using var gripBorder = new Pen(GripBorderColor);
            using var dotBrush = new SolidBrush(GripDotColor);

            if (Orientation == Orientation.Horizontal)
            {
                // Horizontal splitter (top/bottom panels)
                int y = splitter.Top + splitter.Height / 2;

                e.Graphics.DrawLine(
                    splitterPen,
                    splitter.Left,
                    y,
                    splitter.Right,
                    y);

                const int gripWidth = GRIP_WIDTH;
                const int gripHeight = GRIP_HEIGHT;

                var grip = new Rectangle(
                    splitter.Left + (splitter.Width - gripWidth) / 2,
                    splitter.Top + (splitter.Height - gripHeight) / 2,
                    gripWidth,
                    gripHeight);

                DrawGrip(e.Graphics, grip, true, gripBrush, gripBorder, dotBrush);
            }
            else
            {
                // Vertical splitter (left/right panels)
                int x = splitter.Left + splitter.Width / 2;

                e.Graphics.DrawLine(
                    splitterPen,
                    x,
                    splitter.Top,
                    x,
                    splitter.Bottom);

                //Reverse height and width for vertical
                const int gripWidth = GRIP_HEIGHT;
                const int gripHeight = GRIP_WIDTH;

                var grip = new Rectangle(
                    splitter.Left + (splitter.Width - gripWidth) / 2,
                    splitter.Top + (splitter.Height - gripHeight) / 2,
                    gripWidth,
                    gripHeight);

                DrawGrip(e.Graphics, grip, false, gripBrush, gripBorder, dotBrush);
            }
        }

        private static void DrawGrip(
            Graphics g,
            Rectangle rect,
            bool horizontal,
            Brush backBrush,
            Pen borderPen,
            Brush dotBrush)
        {
            g.FillRectangle(backBrush, rect);
            g.DrawRectangle(borderPen, rect);

            const int dot = 2;
            const int spacingX = 5;
            const int spacingY = 3;

            if (horizontal)
            {
                int startX = rect.Left + 6;
                int startY = rect.Top + 2;

                for (int row = 0; row < 2; row++)
                {
                    for (int col = 0; col < 4; col++)
                    {
                        g.FillEllipse(
                            dotBrush,
                            startX + col * spacingX,
                            startY + row * spacingY,
                            dot,
                            dot);
                    }
                }
            }
            else
            {
                int startX = rect.Left + 2;
                int startY = rect.Top + 6;

                for (int col = 0; col < 2; col++)
                {
                    for (int row = 0; row < 4; row++)
                    {
                        g.FillEllipse(
                            dotBrush,
                            startX + col * spacingY,
                            startY + row * spacingX,
                            dot,
                            dot);
                    }
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
    }
}
