using System;
using System.IO;
using System.Text.RegularExpressions;

namespace MyNetia.Model
{
    public static class DirectoryManager
    {
        public static string DESKTOP = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public static void createDirectory(string name)
        {
            string path = Path.GetFullPath(@".\AppResources\Images\" + name);
            try { Directory.CreateDirectory(path); }
            catch (ArgumentException e) { throw new ArgumentException("Invalid path (some characters are not allowed:\n\n" + e.Message); }
        }

        public static void deleteDirectory(string name)
        {
            string path = Path.GetFullPath(@".\AppResources\Images\" + name);
            try { Directory.Delete(path, true); }
            catch (DirectoryNotFoundException e) { throw new DirectoryNotFoundException("Path is incorrect:\n\n" + e.Message); }
        }

        public static void renameDirectory(string oldName, string name)
        {
            string path = Path.GetFullPath(@".\AppResources\Images\");
            try { Directory.Move(path + oldName, path + name); }
            catch (ArgumentException e) { throw new ArgumentException("Invalid path (some characters are not allowed:\n\n" + e.Message); }
            catch (DirectoryNotFoundException e) { throw new DirectoryNotFoundException("Path is incorrect:\n\n" + e.Message); }
        }

        public static bool isValidName(string name)
        {
            Regex containsABadCharacter = new Regex("[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]");
            if (containsABadCharacter.IsMatch(name)) { return false; };
            return true;
        }
    }
}
