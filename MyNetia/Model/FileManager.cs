using System.IO;

namespace MyNetia.Model
{
    public static class FileManager
    {
        private static readonly string jsonPath = Path.GetFullPath(@".\AppResources\SaveDB.json");
        private static readonly string defaultImgPath = Path.GetFullPath(@".\AppResources\Images\DefaultImage.png");
        private static readonly string imagesPath = Path.GetFullPath(@".\AppResources\Images\");

        private static void createTxtFile(string path, string content)
        {
            try { File.WriteAllText(path, content); }
            catch (IOException e) { throw new IOException("Text file creation failed:\n\n" + e.Message); }
        }

        public static byte[] readByteFile(string path)
        {
            try { return File.ReadAllBytes(path); }
            catch (IOException e) { throw new IOException("Read byte file failed:\n\n" + e.Message); }
        }

        private static string readTxtFile(string path)
        {
            try { return File.ReadAllText(path); }
            catch (IOException e) { throw new IOException("Read text file failed:\n\n" + e.Message); }
        }

        private static void copyFile(string sourceFile, string destFile)
        {
            try { File.Copy(sourceFile, destFile, true); }
            catch (IOException e) { throw new IOException("File copy failed\n\n" + e.Message); }
        }

        public static bool isImageExist(string path)
        {
            return File.Exists(imagesPath + path);
        }

        public static void saveJson(string content)
        {
            createTxtFile(jsonPath, content);
        }

        public static void saveJsonToDesktop(string path)
        {
            if (!File.Exists(path))
                createTxtFile(path, "");
            copyFile(jsonPath, path);
        }

        public static string loadJson()
        {
            return readTxtFile(jsonPath);
        }

        public static byte[] defaultImage()
        {
            return readByteFile(defaultImgPath);
        }
    }
}
