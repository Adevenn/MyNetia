using MyNetia.Model;
using MyNetia.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace MyNetia
{
    public partial class App : Application
    {
        //1 window type open at a time
        private readonly List<string> openWindows = new List<string>();

        /// <summary>
        /// Select the right window to load on application starts
        /// </summary>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (UserSettings.userName != "")
            {
                LoginWindow login = new LoginWindow();
                login.Show();
            }
            else
            {
                SetupWindow setup = new SetupWindow();
                setup.Show();
            }
        }

        #region OpenWindow Manager
        /// <summary>
        /// Check is a window is open
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public bool isOpenWindow(string title)
        {
            for (int i = 0; i < openWindows.Count; i++)
            {
                if (openWindows[i].Equals(title))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Add a window to the openWindows list
        /// </summary>
        /// <param name="title"></param>
        public void addWindow(string title)
        {
            if (!isOpenWindow(title))
                openWindows.Add(title);
            else
                throw new Exception("Impossible to open 2 times the same window");
        }

        /// <summary>
        /// Delete a window from the openWindows list
        /// </summary>
        /// <param name="title"></param>
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
        #endregion

        #region Commands
        /// <summary>
        /// List every commands inside the Commands class
        /// </summary>
        /// <returns></returns>
        public List<string> commandsList()
        {
            Commands commands = new Commands();
            List<string> list = new List<string>();
            foreach (FieldInfo field in getConstants(commands.GetType()))
                list.Add((string)field.GetRawConstantValue());
            return list;
        }

        /// <summary>
        /// Get all constants from a class Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<FieldInfo> getConstants(Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public |
                                                    BindingFlags.Static |
                                                    BindingFlags.FlattenHierarchy);
            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
        }
        #endregion

    }
}
