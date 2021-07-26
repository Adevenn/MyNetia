using System;
using System.Collections.Generic;

namespace MyNetia.Model
{
    public class Element
    {
        public string title;
        public string subtitle;
        public List<Chapter> chapters = new List<Chapter>();
        public DateTime lastUpdate;

        public Element(string title)
        {
            this.title = title;
            this.subtitle = "";
            this.chapters.Add(new Chapter());
            this.lastUpdate = DateTime.Now;
        }

        public Element(string title, string subtitle, List<Chapter> chapters)
        {
            this.title = title;
            this.subtitle = subtitle;
            this.chapters = chapters;
            this.lastUpdate = DateTime.Now;
        }

        public Element(string title, string subtitle, List<Chapter> chapters, DateTime lastUpdate)
        {
            this.title = title;
            this.subtitle = subtitle;
            this.chapters = chapters;
            this.lastUpdate = lastUpdate;
        }
    }
}
