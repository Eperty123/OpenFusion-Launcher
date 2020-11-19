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
            webBrowser.Load(Global.COMMUNITY_LINK);

        }

        private void optionsBtn_Click(object sender, EventArgs e)
        {
            ShowOptionsForm();
        }

        private void startGameBtn_Click(object sender, EventArgs e)
        {
            if (Global.GAME_SETTING.SettingsPath == null)
            {
                MessageBox.Show("No game version specified. Default will be used. You can specify one in the options menu.", "No game version specified",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (string.IsNullOrEmpty(Global.LAUNCHER_SETTING.GamePath))
            {
                MessageBox.Show("Client path is not yet set. You must set it first!", "Client path not set", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowOptionsForm();
            }
            else
            {
                if (File.Exists(Global.LAUNCHER_SETTING.GameExecutablePath))
                {
                    if (!Global.GAME_SETTING.MakeAssetLink()) MessageBox.Show($"Failed to link the assetInfo.php to {Global.GAME_SETTING.Version}.", "assetInfo.php linking failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (!Global.GAME_SETTING.MakeLink()) MessageBox.Show("Failed to link the game files! Make sure you've set your game files folder. Default game version or existing will be used.", "Linking failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Process.Start(Global.LAUNCHER_SETTING.GameExecutablePath);
                }
                else MessageBox.Show("Client file not found.", "No client file found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ShowOptionsForm()
        {
            Global.OptionsForm = new Options();
            Global.OptionsForm.ShowDialog();
        }
    }
}
