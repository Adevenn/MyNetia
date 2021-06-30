using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MyNetia.Model
{
    public static class AppResources
    {
        public static DB_Manager dbManager = new DB_Manager();
        private static Commands commands = new Commands();

        public static List<string> commandsList()
        {
            List<string> list = new List<string>();
            foreach (var field in getConstants(commands.GetType()))
                list.Add((string)field.GetRawConstantValue());
            return list;
        }
        private static List<FieldInfo> getConstants(Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
        }
        public static byte[] defaultImage()
        {
            return File.ReadAllBytes(Path.GetFullPath(@".\AppResources\Images\DefaultImage.png"));
        }
        public static string jsonFile()
        {
            return File.ReadAllText(Path.GetFullPath(@".\AppResources\SaveDB.json"));
        }
    }
}
