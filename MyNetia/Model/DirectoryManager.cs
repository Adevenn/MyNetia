using System;
using System.IO;
using System.Text.RegularExpressions;

namespace MyNetia.Model
{
    public static class DirectoryManager
    {
        public static string DESKTOP = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

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
