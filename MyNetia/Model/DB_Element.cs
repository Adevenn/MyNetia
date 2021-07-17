using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MyNetia.Model
{
    [Serializable]
    public class DB_Element
    {
        public string title;
        public string subtitle;
        private List<Chapter> _chapters = new List<Chapter>();
        public List<Chapter> chapters
        {
            get => _chapters;
            set
            {
                if (_chapters != value)
                    _chapters = value;
            }
        }
        public DateTime lastUpdate;

        public DB_Element(string title)
        {
            this.title = title;
            subtitle = "";
            _chapters.Add(new Chapter());
            lastUpdate = DateTime.Now;
        }

        [JsonConstructor]
        public DB_Element(string title, string subtitle, List<Chapter> listChap)
        {
            this.title = title;
            this.subtitle = subtitle;
            _chapters = listChap;
            lastUpdate = DateTime.Now;
        }
    }
}
