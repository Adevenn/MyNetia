using System;
using System.IO;

namespace MyNetia.Model
{
    public static class DirectoryManager
    {
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
            catch (DirectoryNotFoundException e) { throw new DirectoryNotFoundException("The path is not correct:\n\n" + e.Message); }
        }

        public static void renameDirectory(string oldName, string name)
        {
            string path = Path.GetFullPath(@".\AppResources\Images\");
            try { Directory.Move(path + oldName, path + name); }
            catch (ArgumentException e) { throw new ArgumentException("Invalid path (some characters are not allowed:\n\n" + e.Message); }
            catch (DirectoryNotFoundException e) { throw new DirectoryNotFoundException("The path is not correct:\n\n" + e.Message); }
        }
    }
}
