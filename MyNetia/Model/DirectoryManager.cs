using System;
using System.IO;

namespace MyNetia.Model
{
    public static class DirectoryManager
    {
        public static string DESKTOP = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public static string APPDATA = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>
        /// Create MyNetia directory in AppData if it doesn't exist
        /// </summary>
        public static void isConfigDirectoryExist()
        {
            if (!Directory.Exists(APPDATA + "/MyNetia"))
                createDirectory(APPDATA + "/MyNetia");
        }

        /// <summary>
        /// Create a new directory to the path specified
        /// </summary>
        /// <param name="path"></param>
        private static void createDirectory(string path)
        {
            try { Directory.CreateDirectory(path); }
            catch (Exception e) { throw new Exception(e.Message); }
        }
    }
}
