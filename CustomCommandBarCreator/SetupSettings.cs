using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace CustomCommandBarCreator
{
    public partial class SetupSettings : Form
    {

        public Settings Settings { get; protected set; }
        private FileInfo slnFile;

        public SetupSettings(FileInfo slnFile)
        {

            Settings = new Settings();
            this.slnFile = slnFile;
            InitializeComponent();

            LoadSettings();
            ApplySettings();
#if (Donate || Debug)
            panel_url.Visible = true;
#endif
            validate();

        }

        private void SaveSettings()
        {
#if (Donate || Debug)
            Properties.Settings1.Default.OpenSite = Settings.OpenSite;
            Properties.Settings1.Default.URL = Settings.URL;
#endif
            Properties.Settings.Default.SetupFolder = Settings.SetupFolder;
            Properties.Settings.Default.LogoPath = Settings.LogoPath;
            Properties.Settings.Default.ReadmePath = Settings.ReadmePath;
            Properties.Settings.Default.ReadmePath = Settings.ReadmePath;
            Properties.Settings.Default.IconPath = Settings.IconPath;
            Properties.Settings.Default.Author = Settings.Author;
            Properties.Settings.Default.Email = Settings.Email;
            Properties.Settings.Default.Save();



        }
        private void SaveSettings(string xmlPath)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));

                using (StreamWriter sw = new StreamWriter(xmlPath))
                {
                    try
                    {
                        xmlSerializer.Serialize(sw, this.Settings);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void LoadSettings()
        {
#if (Donate || Debug)
            Settings.OpenSite = Properties.Settings1.Default.OpenSite;
            Settings.URL = Properties.Settings1.Default.URL;
#endif
            Settings.SetupFolder = Properties.Settings.Default.SetupFolder;
            Settings.LogoPath = Properties.Settings.Default.LogoPath;
            Settings.ReadmePath = Properties.Settings.Default.ReadmePath;
            Settings.IconPath = Properties.Settings.Default.IconPath;
            if (!string.IsNullOrEmpty(Settings.IconPath) && File.Exists(Settings.IconPath))
                this.Icon = new Icon(Settings.IconPath);
            else
                Settings.IconPath = "";
            Settings.Author = Properties.Settings.Default.Author;
            Settings.Email = Properties.Settings.Default.Email;
            LoadConfigurations();
        }
        private void LoadSettings(string xmlPath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
            using (StreamReader sr = new StreamReader(xmlPath))
            {
                Settings ss = (Settings)xmlSerializer.Deserialize(sr);
#if (Donate || Debug)
                Settings.OpenSite = ss.OpenSite;
                Settings.URL = ss.URL;
#endif
                Settings.SetupFolder = ss.SetupFolder;
                Settings.LogoPath = ss.LogoPath;
                Settings.ReadmePath = ss.ReadmePath;
                Settings.IconPath = ss.IconPath;
                if (!string.IsNullOrEmpty(Settings.IconPath) && File.Exists(Settings.IconPath))
                    this.Icon = new Icon(Settings.IconPath);
                else
                    Settings.IconPath = "";
                Settings.Author = ss.Author;
                Settings.Email = ss.Email;
                LoadConfigurations();
            }
        }
        private void LoadConfigurations()
        {
            txt_configurations.Text = "";

            var solutionConfigurations = new List<object>();
            //for (int j = 1; j <= solutionConfigurations.Count; j++)
            //{
            //    var sol = solutionConfigurations.Item(j);
            //    if (sol.Name.Contains("Release"))
            //    {
            //        txt_configurations.Text += sol.Name.Replace(" Release", "");
            //        txt_configurations.Text += Environment.NewLine;
            //    }
            //}
        }
        private void ApplySettings()
        {
#if (Donate || Debug)
            cb_openURL.Checked = Settings.OpenSite;
            txt_url.Text = Settings.URL;
#endif
            txt_setupFolder.Text = Settings.SetupFolder;
            try
            {
                pictureBox1.Image = new Bitmap(Settings.LogoPath);
            }
            catch { }
            lba_readmePath.Text = Settings.ReadmePath;
            lba_icon.Text = Settings.IconPath;
            txt_Author.Text = Settings.Author;
            txt_email.Text = Settings.Email;
        }

        private void btn_searchFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string p = fd.SelectedPath;
                    using (File.Create(p + "\\t.t")) { }
                    File.Delete(p + "\\t.t");
                    txt_setupFolder.Text = p;
                    Settings.SetupFolder = p;
                }
                catch
                {
                    MessageBox.Show("Invalid folder, please select another");
                }
                finally
                {
                    validate();
                }
            }
        }

        private void btn_logo_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "JPG Image (*.jpg)|*.jpg|BMP Image (*.bmp)|*.bmp|PNG Image (*.png)|*.png";
            of.Multiselect = false;
            if (of.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Bitmap bitmap = new Bitmap(of.FileName);

                    if (bitmap.Width > 160 || bitmap.Height > 160)
                    {
                        MessageBox.Show("Select a image with correct size");
                        return;
                    }
                    else
                    {
                        Settings.LogoPath = of.FileName;
                        pictureBox1.Image = bitmap;
                    }

                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
                finally
                {
                    validate();
                }
            }
        }
        private void btn_readme_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Text Files (*.txt)|*.txt|Rich Text Files (*.rtf)|*.rtf";
            of.Multiselect = false;
            of.InitialDirectory = GetSolutionDirectory();
            if (of.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    lba_readmePath.Text = of.FileName;
                    Settings.ReadmePath = of.FileName;

                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }
        private string solutionDirectory = string.Empty;
        private string GetSolutionDirectory()
        {
            if (string.IsNullOrEmpty(solutionDirectory))
            {
                try
                {

                    solutionDirectory = this.slnFile.DirectoryName;

                }
                catch { }
            }
            return solutionDirectory;
        }
        private void btn_icon_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "icon files (*.ico)|*.ico";
            of.Multiselect = false;
            if (of.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    lba_icon.Text = of.FileName;
                    Settings.IconPath = of.FileName;
                    this.Icon = new Icon(Settings.IconPath);

                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }

        private void lba_readmePath_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start((sender as LinkLabel).Text);
        }

        private void cb_openURL_Click(object sender, EventArgs e)
        {
            Settings.OpenSite = cb_openURL.Checked;
        }

        private void txt_Author_TextChanged(object sender, EventArgs e)
        {
            Settings.Author = txt_Author.Text;
            validate();
        }
        private void txt_email_TextChanged(object sender, EventArgs e)
        {
            Settings.Email = txt_email.Text;
            validate();
        }
        private void txt_url_TextChanged(object sender, EventArgs e)
        {
            Settings.URL = txt_url.Text;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }
        private void validate()
        {
            bool valid = false;
            if (!string.IsNullOrEmpty(txt_Author.Text) && !string.IsNullOrEmpty(txt_setupFolder.Text) && pictureBox1.Image != null)
                valid = true;
            else
                valid = false;
            btn_ok.Enabled = valid;
        }

        private void btn_configurations_Click(object sender, EventArgs e)
        {
            //ConfigurationForm.ConfigurationForm cf = new ConfigurationForm.ConfigurationForm(dte);
            //cf.ShowDialog();
            LoadConfigurations();
        }

        private void btn_load_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Packer Configuration File (*.pcf)|*.pcf";
            of.Multiselect = false;
            if (of.ShowDialog() == DialogResult.OK)
            {
                LoadSettings(of.SafeFileName);
                ApplySettings();
                validate();
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            SaveFileDialog of = new SaveFileDialog();
            of.Filter = "Packer Configuration File (*.pcf)|*.pcf";
            of.InitialDirectory = GetSolutionDirectory();
            of.AddExtension = true;

            if (of.ShowDialog() == DialogResult.OK)
            {
                SaveSettings(of.FileName);
            }
        }
    }
}
