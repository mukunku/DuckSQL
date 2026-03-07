using System.ComponentModel;

namespace DuckSQL
{
    public class LoadingIcon : Form, IDisposable, IProgress<int>
    {
        private const int LoadingPanelWidth = 400;
        private const int LoadingPanelHeight = 120;

        private readonly ProgressBar progressBar;
        private readonly Label labelText;
        private readonly Button cancelButton;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly long totalSteps;
        private long stepsSoFar = 0;

        private Form? parentForm => this.Owner;

        public CancellationToken CancellationToken => cancellationTokenSource.Token;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]

        public EventHandler? OnShow { get; set; } = null;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]

        public EventHandler? OnHide { get; set; } = null;

        public LoadingIcon(string text, long totalSteps)
        {
            this.cancellationTokenSource = new CancellationTokenSource();
            this.totalSteps = totalSteps;

            // Form properties
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(LoadingPanelWidth, LoadingPanelHeight);
            this.BackColor = Color.White;

            // Label
            labelText = new Label
            {
                Text = text,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(150, 4),
                Height = 30,
                Font = new Font(this.Font.FontFamily, 10F)
            };

            // Progress bar
            progressBar = new ProgressBar
            {
                Style = ProgressBarStyle.Continuous,
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                Location = new Point(20, 46),
                Size = new Size(360, 25)
            };

            // Cancel button
            cancelButton = new Button
            {
                Text = "Cancel",
                Size = new Size(80, 25),
                Location = new Point(160, 84)
            };
            cancelButton.Click += CancelButton_Click;

            // Add controls
            this.Controls.Add(labelText);
            this.Controls.Add(progressBar);
            this.Controls.Add(cancelButton);

            // Add border
            this.Paint += LoadingIcon_Paint;

            // Handle events
            this.Shown += (s, e) => OnShow?.Invoke(this, EventArgs.Empty);
            this.FormClosed += (s, e) => OnHide?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnShown(EventArgs e)
        {
            this.Location = this.GetFormCenter();

            //Keep it centered
            this.parentForm?.SizeChanged += (object? sender, EventArgs e) =>
            {
                this.Location = this.GetFormCenter();
            };
            this.parentForm?.LocationChanged += (object? sender, EventArgs e) =>
            {
                this.Location = this.GetFormCenter();
            };

            base.OnShown(e);
        }

        private Point GetFormCenter()
            => this.parentForm is null ? new(0,0) : new(
                this.parentForm.Location.X + (this.parentForm.Width / 2) - (LoadingPanelWidth / 2), 
                this.parentForm.Location.Y + (this.parentForm.Height / 2) - (LoadingPanelHeight / 2));

        public void Report(int value)
        {
            stepsSoFar += value;
            int percentage = Math.Clamp((int)(stepsSoFar * 100 / (double)totalSteps), 0, 100);
            if (percentage != progressBar.Value)
            {
                progressBar.Invoke(() =>
                {
                    if (value == progressBar.Maximum)
                    {
                        //In order to show the progress bar as full, we need to make the value more than the maximum
                        progressBar.Maximum = value + 1;
                        progressBar.Value = value + 1;
                        progressBar.Maximum = value;
                    }
                    else
                    {
                        //HACK: the progress bar's animations cause it to visually lag.
                        //Setting the value this way fixes that (https://stackoverflow.com/a/61583361/1458738)
                        progressBar.Value = percentage;
                        progressBar.Value = percentage - 1;
                        progressBar.Value = percentage;
                    }

                    progressBar.Update();
                });
            }
        }

        private void LoadingIcon_Paint(object? sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
                Color.DarkGray, 1, ButtonBorderStyle.Solid,
                Color.DarkGray, 1, ButtonBorderStyle.Solid,
                Color.DarkGray, 1, ButtonBorderStyle.Solid,
                Color.DarkGray, 1, ButtonBorderStyle.Solid);
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
            cancelButton.Enabled = false;
            cancelButton.Text = "Cancelling...";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                OnHide?.Invoke(this, EventArgs.Empty);
                cancellationTokenSource?.Dispose();
                progressBar?.Dispose();
                labelText?.Dispose();
                cancelButton?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}