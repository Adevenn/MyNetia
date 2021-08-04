using System;
using System.IO;

namespace MyNetia.Model
{
    public static class FileManager
    {
        private static readonly string defaultImgPath = Path.GetFullPath(@".\AppResources\Images\DefaultImage.png");
        public static readonly string CONFIG_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "MyNetia.ini";

        /// <summary>
        /// Read all bytes from a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] readByteFile(string path)
        {
            try { return File.ReadAllBytes(path); }
            catch (IOException e) { throw new IOException("Read byte[] file failed:\n\n" + e.Message); }
        }

        /// <summary>
        /// Return the file name from a path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string getFileName(string path)
        {
            return Path.GetFileName(path);
        }

        /// <summary>
        /// Return the default image
        /// </summary>
        /// <returns></returns>
        public static byte[] defaultImage()
        {
            return readByteFile(defaultImgPath);
        }

        /// <summary>
        /// Check if the file exist and create it if not
        /// </summary>
        public static string loadConfig()
        {
            if (!File.Exists(CONFIG_PATH))
            {

            }

            return CONFIG_PATH;
        }
    }
}
