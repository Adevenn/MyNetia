using MyNetia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace MyNetia
{
    public partial class App : Application
    {
        public DB_Manager dbManager = new DB_Manager();
        //Only 1 window open at a time
        private readonly List<string> openWindows = new List<string>();

        public bool isOpenWindow(string title)
        {
            for (int i = 0; i < openWindows.Count; i++)
            {
                if (openWindows[i].Equals(title))
                    return true;
            }
            return false;
        }

        public void addWindow(string title)
        {
            if (!isOpenWindow(title))
                openWindows.Add(title);
            else
                throw new Exception("Impossible to open 2 times the same window");
        }

        public void deleteWindow(string title)
        {
            for(int i = 0; i < openWindows.Count; i++)
            {
                if (openWindows[i].Equals(title))
                {
                    openWindows.RemoveAt(i);
                    return;
                }
            }
            throw new Exception("Tried to delete an item that doesn't exist");
        }

        public List<string> commandsList()
        {
            Commands commands = new Commands();
            List<string> list = new List<string>();
            foreach (FieldInfo field in getConstants(commands.GetType()))
                list.Add((string)field.GetRawConstantValue());
            return list;
        }

        private List<FieldInfo> getConstants(Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public |
                                                    BindingFlags.Static |
                                                    BindingFlags.FlattenHierarchy);
            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
        }
    }
}
