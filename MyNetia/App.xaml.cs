using System;
using System.Collections.Generic;
using System.Windows;

namespace MyNetia
{
    public partial class App : Application
    {
        //Only 1 window open at a time
        private List<string> openWindows = new List<string>();

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
                throw new Exception("Impossible to add 2 times the same window inside openWindows");
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
    }
}
