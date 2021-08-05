using System;
using System.IO;
using System.Text.RegularExpressions;

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

        /// <summary>
        /// Return true if the name is valid, return false if the name is not valid
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool isValidName(string name)
        {
            Regex containsABadCharacter = new Regex("[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]");
            if (containsABadCharacter.IsMatch(name) || string.IsNullOrWhiteSpace(name)) { return false; };
            return true;
        }
    }
}
