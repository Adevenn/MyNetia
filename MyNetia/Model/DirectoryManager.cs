using System.IO;

namespace MyNetia.Model
{
    public static class DirectoryManager
    {
        public static void createDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        public static void deleteDirectory(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
    }
}
