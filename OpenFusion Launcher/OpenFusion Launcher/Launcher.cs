using OpenFusion_Launcher.Definition;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.Threading;

namespace OpenFusion_Launcher
{
    public partial class Launcher : Form
    {
        ChromiumWebBrowser webBrowser;
        bool stopMinimize;

        private delegate void FormWindowStateDelegate(Form form, FormWindowState formWindowState);

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
                    HandleGameProcess();
                }
                else MessageBox.Show("Client file not found.", "No client file found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void HandleGameProcess()
        {
            Global.GAME_PROCESS = Process.Start(Global.LAUNCHER_SETTING.GameExecutablePath);
            if (Global.GAME_PROCESS != null) GameProcessWatcher();
        }

        private void ShowOptionsForm()
        {
            Global.OptionsForm = new Options();
            Global.OptionsForm.ShowDialog();
        }

        private void SetFormWindowState(Form form, FormWindowState formWindowState)
        {
            if (form.InvokeRequired)
            {
                var d = new FormWindowStateDelegate(SetFormWindowState);
                form.Invoke(d, new object[] { form, formWindowState });
            }
            else
            {
                form.WindowState = formWindowState;
            }
        }

        private void GameProcessWatcher()
        {
            if (Global.GAME_WATCHER_THREAD != null && Global.GAME_WATCHER_THREAD.IsAlive)
                Global.GAME_WATCHER_THREAD.Abort();

            Global.GAME_WATCHER_THREAD = new Thread(() =>
            {
                while (IsGameProcessActive(Global.GAME_PROCESS))
                {
                    switch (WindowState)
                    {
                        case FormWindowState.Normal:
                            if (!stopMinimize) SetFormWindowState(this, FormWindowState.Minimized);
                            break;

                        case FormWindowState.Minimized:
                            stopMinimize = true;
                            break;
                    }

                    // Sleep to not hog the cpu since we're in a while loop.
                    Thread.Sleep(1500);

                }
                // Process killed or not found, go back to normal.
                if (WindowState != FormWindowState.Normal)
                    SetFormWindowState(this, FormWindowState.Normal);
            });
            Global.GAME_WATCHER_THREAD.IsBackground = true;
            Global.GAME_WATCHER_THREAD.Start();
        }

        private bool IsGameProcessActive(Process gameProcess)
        {
            if (gameProcess != null)
            {
                var processes = Process.GetProcessesByName(gameProcess.ProcessName);
                for (int i = 0; i < processes.Length; i++)
                {
                    var process = processes[i];
                    if (process.Id.Equals(gameProcess.Id))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
