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
        public void add(string name, string port, List<string> theoryTxt, List<string> hackingTxt, List<string> theoryImg, List<string> hackingImg)
        {
            _db.Add(new DB_Element(name, port, theoryTxt, hackingTxt, theoryImg, hackingImg, DateTime.Now));
            sort();
            saveAsJson(Path.GetFullPath(@".\AppResources"));
        }
        public void remove(string name)
        {
            for (int i = 0; i < db.Count; i++)
            {
                if (db[i].name == name)
                {
                    _db.RemoveAt(i);
                    break;
                }
            }
            sort();
            saveAsJson(Path.GetFullPath(@".\AppResources"));
        }
        public void update(int id, DB_Element element)
        {
            _db[id] = element;
            saveAsJson(Path.GetFullPath(@".\AppResources"));
        }
        public void sort()
        {
            for (int i = 1; i < db.Count; i++)
            {
                if (_db[i].name.CompareTo(_db[i - 1].name) == -1)
                {
                    DB_Element element = _db[i];
                    _db[i] = _db[i - 1];
                    _db[i - 1] = element;
                    i = 0;
                }
            }
        }

        #endregion

        #region Access to DB
        public DB_Element getElement(string name)
        {
            for (int i = 0; i < db.Count; i++)
            {
                if (name == db[i].name)
                {
                    return db[i];
                }
            }
            return null;
        }
        public bool isExist(string name)
        {
            for (int i = 0; i < db.Count; i++)
            {
                if (db[i].name == name)
                    return true;
            }
            return false;
        }
        public int getId(string name)
        {
            for (int i = 0; i < db.Count; i++)
            {
                if (db[i].name == name)
                    return i;
            }
            return -1;
        }
        public List<string> getNames()
        {
            List<string> names = new List<string>();
            foreach (DB_Element element in db)
                names.Add(element.name);
            return names;
        }
        #endregion

        #region Json Manager
        public void readJson()
        {
            string jsonContent = AppResources.jsonFile();
            if (!string.IsNullOrWhiteSpace(jsonContent))
                _db = JsonConvert.DeserializeObject<DB_Manager>(jsonContent).db;
        }
        public void saveAsJson(string path)
        {
            string jsonString = JsonConvert.SerializeObject(new { db }, Formatting.None);
            File.WriteAllText(path + @"\SaveDB.json", jsonString);
        }
        #endregion
    }
}
