using System;
using System.Collections.Generic;

namespace MyNetia.Model
{
    [Serializable]
    public class DB_Element
    {
        public string title;
        public string subtitle;
       
        private readonly List<Chapter> _chapters = new List<Chapter>();
        public List<Chapter> chapters => _chapters;
        public DateTime lastUpdate;
        public DB_Element(string title)
        {
            this.title = title;
            subtitle = "";
            _chapters.Add(new Chapter());
            lastUpdate = DateTime.Now;
        }
        public DB_Element(string title, string subtitle, List<Chapter> listChap)
        {
            this.title = title;
            this.subtitle = subtitle;
            _chapters = listChap;
            lastUpdate = DateTime.Now;
        }

        public void deleteChapter(string title)
        {
            _chapters.RemoveAt(getChapterID(title));
            GC.Collect();
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
