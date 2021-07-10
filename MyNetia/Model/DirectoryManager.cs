using System.IO;

namespace MyNetia.Model
{
    public static class DirectoryManager
    {
        public static void createDirectory(string name)
        {
            string path = Path.GetFullPath(@".\AppResources\Images\" + name);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static void deleteDirectory(string name)
        {
            string path = Path.GetFullPath(@".\AppResources\Images\" + name);
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }

        public static void renameDirectory(string oldName, string name)
        {
            string path = Path.GetFullPath(@".\AppResources\Images\");
            if (Directory.Exists(path + oldName))
                Directory.Move(path + oldName, path + name);
        }
    }
}
