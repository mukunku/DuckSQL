using System.Diagnostics;
using System.Reflection;
using DuckSQL.Analytics;
using DuckSQL.Helpers;

namespace DuckSQL
{
    public partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            this.Text = this.Text.Format(AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = this.labelVersion.Text.Format(Env.AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.textBoxDescription.Text = AssemblyDescription.Replace($"Privacy policy:", $"{Resources.Strings.PrivacyPolicyLabelText}:"); //HACK: to translate privacy policy text
            this.newVersionLabel.Image = null;

            if (!AmplitudeEvent.HasApiKey)
            {
                this.textBoxDescription.Text = $"No Amplitude API Key!{Environment.NewLine}{Environment.NewLine}"
                    + this.textBoxDescription.Text;
            }
        }

        private void AboutBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private async void AboutBox_Load(object sender, EventArgs e)
        {
            try
            {
                this.newVersionLabel.Visible = false;
                var latestRelease = await Env.FetchLatestRelease();
                this.newVersionLabel.Text = this.newVersionLabel.Text.Format(latestRelease.Version);
                this.newVersionLabel.Visible = true;

                if (latestRelease.Version > Env.AssemblyVersion)
                {
                    this.newVersionLabel.Tag = latestRelease.Url;
                    this.newVersionLabel.Image = Properties.Resources.external_link;
                }
                else if (latestRelease.Version == Env.AssemblyVersion)
                {
                    this.newVersionLabel.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionEvent.FireAndForget(ex);
                this.newVersionLabel.Visible = false;
            }
        }

        //public override void SetTheme(Theme theme)
        //{
        //    if (DesignMode)
        //    {
        //        return;
        //    }

        //    base.SetTheme(theme);
        //    this.okButton.BackColor = Color.White;
        //    this.okButton.ForeColor = Color.Black;
        //}

        private void newVersionLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.newVersionLabel.Tag is Uri url)
                Process.Start(new ProcessStartInfo(url.ToString()) { UseShellExecute = true });
        }

        #region Assembly Attribute Accessors

        private string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != string.Empty)
                    {
                        return titleAttribute.Title;
                    }
                }
                return "ParquetViewer.exe";
            }
        }

        private string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        private string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        private string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        private string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion
    }
}