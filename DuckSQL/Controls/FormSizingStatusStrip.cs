using System.Runtime.InteropServices;

namespace DuckSQL.Controls
{
    /// <summary>
    /// StatusStrip whose sizing grip resizes the top-level form even when the strip is nested
    /// inside other controls (e.g. a tab page). The stock StatusStrip only does this when the
    /// grip sits within ~2px of the form's client-area corner.
    /// </summary>
    internal class FormSizingStatusStrip : StatusStrip
    {
        private const int WM_NCHITTEST = 0x0084;
        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int WM_NCLBUTTONDBLCLK = 0x00A3;
        private const int HTBOTTOMRIGHT = 17;

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCHITTEST when IsOverResizableGrip(m.LParam):
                    // Gives us the diagonal resize cursor and routes the click to WM_NCLBUTTONDOWN below
                    m.Result = HTBOTTOMRIGHT;
                    return;

                case WM_NCLBUTTONDOWN when m.WParam == HTBOTTOMRIGHT && FindForm() is Form form:
                    // Hand the drag off to the form so it resizes instead of this child control
                    ReleaseCapture();
                    SendMessage(form.Handle, WM_NCLBUTTONDOWN, HTBOTTOMRIGHT, m.LParam);
                    return;

                case WM_NCLBUTTONDBLCLK when m.WParam == HTBOTTOMRIGHT:
                    return; // swallow so it doesn't do anything odd to the child
            }

            base.WndProc(ref m);
        }

        private bool IsOverResizableGrip(IntPtr lParam)
        {
            if (!SizingGrip || FindForm() is not Form form
                || form.WindowState != FormWindowState.Normal
                || form.FormBorderStyle is not (FormBorderStyle.Sizable or FormBorderStyle.SizableToolWindow))
                return false;

            // lParam holds signed 16-bit screen coordinates (can be negative on multi-monitor setups)
            long lp = lParam.ToInt64();
            var screenPoint = new Point((short)(lp & 0xFFFF), (short)((lp >> 16) & 0xFFFF));
            return SizeGripBounds.Contains(PointToClient(screenPoint));
        }
    }
}
