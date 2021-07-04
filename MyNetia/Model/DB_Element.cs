using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyNetia.Model
{
    [Serializable]
    public class DB_Element
    {
        public string title;
        public string subtitle;
        private ObservableCollection<Chapter> _chapters = new ObservableCollection<Chapter>();
        public ObservableCollection<Chapter> chapters => _chapters;
        public DateTime lastUpdate;

        public DB_Element(string title)
        {
            this.title = title;
            subtitle = "";
            _chapters.Add(new Chapter());
            lastUpdate = DateTime.Now;
        }

        public DB_Element(string title, string subtitle, ObservableCollection<Chapter> listChap)
        {
            this.title = title;
            this.subtitle = subtitle;
            _chapters = listChap;
            lastUpdate = DateTime.Now;
        }

        public void deleteChapter(string title)
        {
            _chapters.RemoveAt(getChapterID(title));
        }

        public List<string> getChaptersTitles()
        {
            List<string> titles = new List<string>();
            foreach(Chapter ch in chapters)
                titles.Add(ch.chapTitle);
            return titles;
        }

        private int getChapterID(string title)
        {
            for (int i = 0; i < chapters.Count; i++)
            {
                if (chapters[i].chapTitle.Equals(title))
                    return i;
            }
            return -1;
        }
    }
}
