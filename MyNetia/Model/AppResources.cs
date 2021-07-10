using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyNetia.Model
{
    public static class AppResources
    {
        public static DB_Manager dbManager = new DB_Manager();

        public static List<string> commandsList()
        {
            Commands commands = new Commands();
            List<string> list = new List<string>();
            foreach (FieldInfo field in getConstants(commands.GetType()))
                list.Add((string)field.GetRawConstantValue());
            return list;
        }

        private static List<FieldInfo> getConstants(Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public |
                                                    BindingFlags.Static |
                                                    BindingFlags.FlattenHierarchy);
            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
        }
    }
}
