using Newtonsoft.Json;
using System.IO;

namespace OpenFusion_Launcher.Definition
{
    public abstract class Setting<T>
    {
        #region Public Variables
        public string SettingsPath { get; set; }
        #endregion

        #region Protected, Private Variables
        protected string SettingsContent { get; set; }
        protected T DeserializedContent { get; set; }
        #endregion

        #region Constructors
        public Setting()
        {
            Initialize();
        }
        #endregion

        #region Overridable Methods

        /// <summary>
        /// Load file from the specified path.
        /// </summary>
        /// <param name="settingsFile">The path for the settings file.</param>
        public virtual void LoadFile(string settingsFile)
        {
            if (File.Exists(settingsFile))
            {
                SettingsPath = settingsFile;
                SettingsContent = File.ReadAllText(settingsFile);
            }
        }

        /// <summary>
        /// Initialization.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Save settings to file.
        /// </summary>
        public virtual void Save()
        {
            if (!string.IsNullOrEmpty(SettingsPath))
                File.WriteAllText(SettingsPath, Serialize());
        }

        /// <summary>
        /// Save settings to the specified path.
        /// </summary>
        /// <param name="path">The path to save the settings file.</param>
        public virtual void Save(string path)
        {
            SettingsPath = path;
            File.WriteAllText(path, Serialize());
        }
        #endregion

        #region Public Methods

        #endregion

        #region Protected, Private Methods
        /// <summary>
        /// Return the serialized version of this class.
        /// </summary>
        /// <returns></returns>
        protected string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Deserialize the loaded content to the <see cref="T"/> type.
        /// </summary>
        /// <returns></returns>
        protected T Deserialize()
        {
            return JsonConvert.DeserializeObject<T>(SettingsContent);
        }

        /// <summary>
        /// Is the settings file valid?
        /// </summary>
        /// <returns></returns>
        protected bool IsValid()
        {
            return !string.IsNullOrEmpty(SettingsContent) && !string.IsNullOrEmpty(SettingsPath);
        }
        #endregion
    }
}
