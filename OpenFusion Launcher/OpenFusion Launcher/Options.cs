using OpenFusion_Launcher.Definition;
using System.Windows.Forms;

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
            var fbd = new FolderBrowserDialog();
            fbd.Description = "Browse for the folder containing assetbundles.";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                Global.GAME_SETTING.GameFilesPath = fbd.SelectedPath;
                gameFolderTxtBox.Text = fbd.SelectedPath;
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
    }
}
