using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MyNetia.Model
{
    [Serializable]
    public class DB_Manager
    {
        private List<DB_Element> _db = new List<DB_Element>();
        public List<DB_Element> db => _db;

        #region DB modifications
        public void addElement(string title)
        {
            _db.Add(new DB_Element(title));
            sortDB();
        }

        public void addElement(string title, string subtitle, List<Chapter> chapList)
        {
            _db.Add(new DB_Element(title, subtitle, chapList));
            sortDB();
        }

        public void updateElement(string oldTitle, string title, string subtitle,List<Chapter> chapList)
        {
            int id = getElementID(oldTitle);
            _db[id] = new DB_Element(title, subtitle, chapList);
            sortDB();
        }

        public void deleteElement(string title)
        {
            for (int i = 0; i < db.Count; i++)
            {
                if (db[i].title == title)
                {
                    _db.RemoveAt(i);
                    break;
                }
            }
            sortDB();
        }

        private void sortDB()
        {
            for (int i = 1; i < db.Count; i++)
            {
                if (_db[i].title.CompareTo(_db[i - 1].title) == -1)
                {
                    DB_Element elem = _db[i];
                    _db[i] = _db[i - 1];
                    _db[i - 1] = elem;
                    i = 1;
                }
            }
            saveJson(Path.GetFullPath(@".\AppResources"));
        }
        #endregion

        #region Access to DB
        public DB_Element getElement(string title)
        {
            for (int i = 0; i < db.Count; i++)
            {
                if (title == db[i].title)
                    return db[i];
            }
            return new DB_Element(title);
        }

        public bool isElementExist(string title)
        {
            for (int i = 0; i < db.Count; i++)
            {
                if (db[i].title.Equals(title))
                    return true;
            }
            return false;
        }

        private int getElementID(string title)
        {
            for (int i = 0; i < db.Count; i++)
            {
                if (db[i].title.Equals(title))
                    return i;
            }
            return -1;
        }

        public List<string> getTitles()
        {
            List<string> titles = new List<string>();
            foreach (DB_Element elem in db)
                titles.Add(elem.title);
            return titles;
        }
        #endregion

        #region Json Manager
        public void readJson()
        {
            try
            {
                string jsonContent = AppResources.jsonFile();
                if (!string.IsNullOrWhiteSpace(jsonContent))
                    _db = JsonConvert.DeserializeObject<DB_Manager>(jsonContent).db;
            }
            catch (JsonSerializationException) { _db = new List<DB_Element>(); }
        }

        public void saveJson(string path)
        {
            string jsonString = JsonConvert.SerializeObject(new { db }, Formatting.None);
            File.WriteAllText(path + @"\SaveDB.json", jsonString);
        }
        #endregion
    }
}
