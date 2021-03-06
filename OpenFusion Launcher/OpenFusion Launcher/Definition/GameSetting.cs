﻿using CreateMaps;
using System;
using System.IO;
using System.Windows.Forms;

namespace OpenFusion_Launcher.Definition
{
    public class GameSetting : Setting<GameSetting>
    {
        #region Public Variables
        public string Name { get; set; }
        public string UnityCachePath { get; set; }
        public string GameFilesPath { get; set; }
        public string Version { get; set; }
        #endregion

        #region Constructors
        public GameSetting()
        {
            Initialize();
        }
        #endregion

        #region Overridable Methods
        public override void Initialize()
        {
            UnityCachePath = Global.UNITY_CACHE_PATH;
        }

        public override void LoadFile(string settingsFile)
        {
            base.LoadFile(settingsFile);
            HandleDeserialization();
        }

        public override void Save()
        {
            if (!string.IsNullOrEmpty(SettingsPath))
                File.WriteAllText(SettingsPath, Serialize());
            else
            {
                var sfd = new SaveFileDialog();
                sfd.Title = "Save game setting file";
                sfd.Filter = "Setting file|*.json";
                sfd.FileName = Version;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SettingsPath = sfd.FileName;
                    File.WriteAllText(SettingsPath, Serialize());
                }
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Make the link to the appropriate folder for the <see cref="GameFilesPath"/>.
        /// </summary>
        /// <returns>Return true if it made the link otherwise false.</returns>
        public bool MakeLink()
        {
            bool result = false;
            var ffCachePath = Path.Combine(Global.UNITY_CACHE_PATH, Global.FF_CACHE_FOLDER_NAME);
            var ffCachePathBak = Path.Combine(Global.UNITY_CACHE_PATH, "_" + Global.FF_CACHE_FOLDER_NAME);
            var unityPath = Global.UNITY_CACHE_PATH;

            try
            {
                if (!string.IsNullOrEmpty(GameFilesPath))
                {

                    // Create the Unity folder if not existing.
                    if (!Directory.Exists(unityPath)) Directory.CreateDirectory(unityPath);

                    // If the Fusionfall folder exists, rename it so we can create the link without problems.
                    if (!JunctionPoint.Exists(ffCachePath) && Directory.Exists(ffCachePath) && !Directory.Exists(ffCachePathBak)) Directory.Move(ffCachePath, ffCachePathBak);

                    // Delete any prior traces of any links.
                    if (JunctionPoint.Exists(ffCachePath)) JunctionPoint.Delete(ffCachePath);

                    // Create the link.
                    JunctionPoint.Create(ffCachePath, GameFilesPath, true);

                    result = true;
                }
            }
            catch (Exception)
            {
                // Revert the backup folder.
                if (Directory.Exists(ffCachePathBak) && !Directory.Exists(ffCachePath)) Directory.Move(ffCachePathBak, ffCachePath);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Make the assetInfo.php page with the specified version in <see cref="Version"/>.
        /// </summary>
        /// <returns>Return true if it made the file otherwise false.</returns>
        public bool MakeAssetLink()
        {
            bool result = false;
            try
            {
                var filesPath = Path.Combine(Global.LAUNCHER_SETTING.GamePath, "resources/app/files");
                var assetInfoPath = Path.Combine(filesPath, "assetInfo.php");
                var indexPath = Path.Combine(filesPath, "index.html");
                var indexPage = new HtmlAgilityPack.HtmlDocument();

                // Load the index page if it exists and modify the unity embed src value.
                if (File.Exists(indexPath))
                {
                    // Load the index page.
                    indexPage.Load(indexPath);

                    /* References:
                     * https://stackoverflow.com/questions/52890557/htmlagilitypack-get-element-in-class-by-class
                     * https://html-agility-pack.net/from-file
                     * https://html-agility-pack.net/knowledge-base/39274255/how-to-get-src-tag-from-image-
                     */
                    var embed_node = indexPage.DocumentNode.SelectSingleNode("//embed[@id='Unity_embed']");
                    embed_node.SetAttributeValue("src", $"{Global.GAME_CDN_LINK}/{Version}/main.unity3d");
                    indexPage.Save(indexPath);
                }

                // If the files path exists write the modified assetInfo.php.
                if (Directory.Exists(filesPath))
                {
                    File.WriteAllText(assetInfoPath, $"{Global.GAME_CDN_LINK}/{Version}/");
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        #endregion

        #region Protected, Private Methods
        /// <summary>
        /// Deserialize file and load its settings.
        /// </summary>
        protected void HandleDeserialization()
        {
            // If everything's okay continue.
            if (IsValid())
            {
                var loaded = Deserialize();
                Name = loaded.Name;
                GameFilesPath = loaded.GameFilesPath;
                UnityCachePath = loaded.UnityCachePath;
                Version = loaded.Version;
            }
        }
        #endregion
    }
}
