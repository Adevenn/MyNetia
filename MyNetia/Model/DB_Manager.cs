using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MyNetia.Model
{
    [Serializable]
    public class DB_Manager
    {
        private List<DB_Element> _db = new List<DB_Element>();
        public List<DB_Element> db => _db;

        #region DB modifs
        public void addElement(string title)
        {
            if (!isElementExist(title))
            {
                DirectoryManager.createDirectory(title);
                _db.Add(new DB_Element(title));
                sortDB();
            }
            else
                throw new Exception("Impossible to add 2 times the same element title");
        }

        public void updateElement(string oldTitle, string title, string subtitle, List<Chapter> chapList)
        {
            if(oldTitle != title)
                DirectoryManager.renameDirectory(oldTitle, title);
            int id = getElementID(oldTitle);
            _db[id] = new DB_Element(title, subtitle, chapList);
            sortDB();
        }

        public void deleteElement(string title)
        {
            DirectoryManager.deleteDirectory(title);
            int id = getElementID(title);
            _db.RemoveAt(id);
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
            saveJson();
        }
        #endregion

        #region Access to DB
        public List<string> getTitles()
        {
            List<string> titles = new List<string>();
            foreach (DB_Element elem in db)
                titles.Add(elem.title);
            return titles;
        }

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
        #endregion

        #region Json Manager
        public void readJson()
        {
            try
            {
                string jsonContent = FileManager.loadJson();
                if (!string.IsNullOrWhiteSpace(jsonContent))
                    _db = JsonConvert.DeserializeObject<DB_Manager>(jsonContent).db;
            }
            catch (JsonSerializationException) { _db = new List<DB_Element>(); }
        }

        public void saveJson()
        {
            string jsonContent = JsonConvert.SerializeObject(new { db }, Formatting.None);
            FileManager.saveJson(jsonContent);
        }

        public void copyJsonToDesktop()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            FileManager.copyJson(path);
        }
        #endregion
    }
}
