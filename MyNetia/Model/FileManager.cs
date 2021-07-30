using System.IO;

namespace MyNetia.Model
{
    public static class FileManager
    {
        private static readonly string defaultImgPath = Path.GetFullPath(@".\AppResources\Images\DefaultImage.png");

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
    }
}
