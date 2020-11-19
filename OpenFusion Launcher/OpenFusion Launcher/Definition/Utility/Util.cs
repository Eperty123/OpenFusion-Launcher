using System.IO;

namespace OpenFusion_Launcher.Definition.Utility
{
    public static class Util
    {
        public static string GetAbsolutePath(this string path)
        {
            return Path.GetFullPath(path);
        }
    }
}
