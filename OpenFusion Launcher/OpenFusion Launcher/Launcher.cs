using OpenFusion_Launcher.Definition;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace OpenFusion_Launcher
{
    public partial class Launcher : Form
    {
        ChromiumWebBrowser webBrowser;
        public Launcher()
        {
            InitializeComponent();
            InitializeChromium();
            Global.LAUNCHER_SETTING.LoadFile(Global.LAUNCHER_SETTING_PATH);
        }

        private void InitializeChromium()
        {
            CefSettings settings = new CefSettings();
            settings.CachePath = Path.Combine(Directory.GetCurrentDirectory(), "WebCache");
            Cef.Initialize(settings);

            webBrowser = new ChromiumWebBrowser();
            panel1.Controls.Add(webBrowser);
            webBrowser.Dock = DockStyle.Fill;
            webBrowser.Load("https://discord.gg/DYavckB");

        }

        private void optionsBtn_Click(object sender, EventArgs e)
        {
            Global.OptionsForm = new Options();
            Global.OptionsForm.ShowDialog();
        }

        private void startGameBtn_Click(object sender, EventArgs e)
        {
            if (Global.GAME_SETTING.SettingsPath == null)
            {
                MessageBox.Show("No game version specified. Default will be used. You can specify one in the options menu.", "No game version specified",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (string.IsNullOrEmpty(Global.LAUNCHER_SETTING.GameExecutablePath))
            {
                var ofd = new OpenFileDialog();
                ofd.Title = "Browse for OpenFusion client executable";
                ofd.Filter = "Client file|*.exe";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Global.LAUNCHER_SETTING.GameExecutablePath = ofd.FileName;
                    Global.LAUNCHER_SETTING.Save();
                }
            }

            if (File.Exists(Global.LAUNCHER_SETTING.GameExecutablePath))
            {
                Global.GAME_SETTING.MakeLink();
                Process.Start(Global.LAUNCHER_SETTING.GameExecutablePath);
            }
            else MessageBox.Show("Client file not found.", "No client file found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
