using OpenFusion_Launcher.Definition;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;

namespace OpenFusion_Launcher
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            gameVersionComboBox.Items.Clear();
            foreach (var entry in Global.BUILD_VERSIONS)
            {
                gameVersionComboBox.Items.Add(entry);
            }
            gameVersionComboBox.SelectedItem = Global.GAME_SETTING.Version;
            gameFolderTxtBox.Text = Global.GAME_SETTING.GameFilesPath;

            clientPathTxtBox.Text = Global.LAUNCHER_SETTING.GamePath;
        }

        private void saveBtn_Click(object sender, System.EventArgs e)
        {
            var gameVersion = gameVersionComboBox.SelectedItem.ToString();
            if (Global.GAME_SETTING.Version != gameVersion)
                Global.GAME_SETTING.SettingsPath = null;

            Global.GAME_SETTING.Version = gameVersion;
            Global.GAME_SETTING.Save();
            Global.LAUNCHER_SETTING.Save();
        }

        private void browseBtn_Click(object sender, System.EventArgs e)
        {
            var fbd = new CommonOpenFileDialog();
            fbd.IsFolderPicker = true;
            fbd.Title = "Browse for the folder containing assetbundles";
            if (fbd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Global.GAME_SETTING.GameFilesPath = fbd.FileName;
                gameFolderTxtBox.Text = fbd.FileName;
            }
        }

        private void gameVersionComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {

        }

        private void loadBtn_Click(object sender, System.EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Title = "Browse for game setting file";
            ofd.Filter = "Setting file|*.json";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Global.GAME_SETTING.LoadFile(ofd.FileName);
                Initialize();
            }
        }

        private void clientPathBrowseBtn_Click(object sender, System.EventArgs e)
        {
            var fbd = new CommonOpenFileDialog();
            fbd.IsFolderPicker = true;
            fbd.Title = "Browse for the folder containing the OpenFusion client";
            if (fbd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Global.LAUNCHER_SETTING.GamePath = fbd.FileName;
                Global.LAUNCHER_SETTING.GameExecutablePath = Path.Combine(fbd.FileName, "OpenFusionClient.exe");
                clientPathTxtBox.Text = fbd.FileName;
            }
        }
    }
}
