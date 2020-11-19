using System.IO;
using System.Windows.Forms;

namespace OpenFusion_Launcher.Definition
{
    public class LauncherSetting : Setting<LauncherSetting>
    {
        #region Public Variables
        public string LastUsedGameSettingPath { get; set; }
        public string GameExecutablePath { get; set; }
        #endregion

        #region Constructors
        public LauncherSetting()
        {
            Initialize();
        }
        #endregion

        #region Overridable Methods
        public override void LoadFile(string settingsFile)
        {
            base.LoadFile(settingsFile);
            LoadSetting();
        }

        public override void Initialize()
        {
            SettingsPath = Global.LAUNCHER_SETTING_PATH;
        }

        public override void Save()
        {
            LastUsedGameSettingPath = Global.GAME_SETTING.SettingsPath;

            if (!string.IsNullOrEmpty(SettingsPath))
                File.WriteAllText(SettingsPath, Serialize());
            else
            {
                var sfd = new SaveFileDialog();
                sfd.Title = "Save launcher setting file";
                sfd.Filter = "Setting file|*.json";
                sfd.FileName = "Launcher";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SettingsPath = sfd.FileName;
                    File.WriteAllText(SettingsPath, Serialize());
                }
            }
        }

        public override void Save(string path)
        {
            LastUsedGameSettingPath = Global.GAME_SETTING.SettingsPath;
            base.Save(path);
        }
        #endregion

        #region Public Methods

        #endregion

        #region Protected, Private Methods
        /// <summary>
        /// Load the launcher setting.
        /// </summary>
        protected void LoadSetting()
        {
            if (IsValid())
            {
                var loaded = Deserialize();
                LastUsedGameSettingPath = loaded.LastUsedGameSettingPath;
                GameExecutablePath = loaded.GameExecutablePath;
                Global.GAME_SETTING.LoadFile(LastUsedGameSettingPath);
            }
        }
        #endregion
    }
}
