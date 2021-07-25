using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace MyNetia.Model
{
    [Serializable]
    public class DB_Manager
    {
        private List<Element> _db = new List<Element>();
        public List<Element> db => _db;

        private List<string> _elemTitles;
        public List<string> elemTitles => _elemTitles;

        private readonly string server = "localhost";
        private readonly string port = "5432";
        private readonly string database = "MyNetia";
        private readonly string userID = "Adeven";
        private readonly string password = "1963258740";
        private NpgsqlConnection connection;

        public void setup()
        {
            string connString = "Server = localhost; Port = 5432; Database = MyNetia; Username = Adeven; Password = 1963258740";
            connection = new NpgsqlConnection(connString);
            getTitles();
        }

        #region DB modifs OFFLINE
        public void addElementOffline(string title)
        {
            if (!isElementExist(title))
            {
                DirectoryManager.createDirectory(title);
                _db.Add(new Element(title));
                sortDBOffline();
            }
            else
                throw new Exception("Impossible to add 2 times the same element title");
        }

        public void updateElementOffline(string oldTitle, string title, string subtitle, List<Chapter> chapList)
        {
            if(oldTitle != title)
                DirectoryManager.renameDirectory(oldTitle, title);
            int id = getElementID(oldTitle);
            _db[id] = new Element(title, subtitle, chapList);
            sortDBOffline();
        }

        public void deleteElementOffline(string title)
        {
            DirectoryManager.deleteDirectory(title);
            int id = getElementID(title);
            _db.RemoveAt(id);
            sortDBOffline();
        }

        private void sortDBOffline()
        {
            for (int i = 1; i < db.Count; i++)
            {
                if (_db[i].title.CompareTo(_db[i - 1].title) == -1)
                {
                    Element elem = _db[i];
                    _db[i] = _db[i - 1];
                    _db[i - 1] = elem;
                    i = 1;
                }
            }
            saveJson();
        }
        #endregion


        #region DB modifs
        public async void addElement(string title, string subtitle, List<string> chapTitles, List<List<string>> texts, List<List<byte[]>> images)
        {
            /* Integrity constraint
             * Element title MUST be unique
             * Chapter title MUST be unique inside a element
             */
            try
            {
                //Connect to DB
                connection.Open();

                //INSERT NEW ELEMENT
                NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO elements (title, subtitle, lastupdate) VALUES (@p, @p2, @p3)", connection);
                cmd.Parameters.AddWithValue("p", title);
                cmd.Parameters.AddWithValue("p2", subtitle);
                cmd.Parameters.AddWithValue("p3", DateTime.Now);
                await cmd.ExecuteNonQueryAsync();

                //INSERT CHAPTERS
                for(int i = 0; i < chapTitles.Count; i++)
                {
                    cmd = new NpgsqlCommand("INSERT INTO chapters (title, idelem) VALUES (@p, @p2)", connection);
                    cmd.Parameters.AddWithValue("p", chapTitles[i]);
                    cmd.Parameters.AddWithValue("p2", title);
                    await cmd.ExecuteNonQueryAsync();

                    //INSERT TEXTS
                    foreach (string s in texts[i])
                    {
                        cmd = new NpgsqlCommand("INSERT INTO texts (string, idchap, idelem) VALUES (@p, @p2, @p3)", connection);
                        cmd.Parameters.AddWithValue("p", s);
                        cmd.Parameters.AddWithValue("p2", chapTitles[i]);
                        cmd.Parameters.AddWithValue("p3", title);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    //INSERT IMAGES
                    foreach (byte[] img in images[i])
                    {
                        cmd = new NpgsqlCommand("INSERT INTO texts (string, idchap, idelem) VALUES (@p, @p2, @p3)", connection);
                        cmd.Parameters.AddWithValue("p", img);
                        cmd.Parameters.AddWithValue("p2", chapTitles[i]);
                        cmd.Parameters.AddWithValue("p3", title);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                //DISCONNECT FROM DB
                cmd.Dispose();
                connection.Close();
            }
            catch (Exception e) { throw new Exception(e.Message); }
            getTitles();
        }

        public async void updateElement()
        {
            throw new NotImplementedException();
        }

        public async void deleteElement(string title)
        {
            try
            {
                //Connect to DB
                connection.Open();

                //DELETE ELEMENT
                NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM elements WHERE title=(@p)", connection);
                cmd.Parameters.AddWithValue("p", title);
                await cmd.ExecuteNonQueryAsync();

                //DISCONNECT FROM DB
                cmd.Dispose();
                connection.Close();
            }
            catch (Exception e) { throw new Exception(e.Message); }
            getTitles();
        }
        #endregion

        #region Access to DB
        public void getTitles()
        {
            //Connect to DB
            connection.Open();

            //SELECT TITLES
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT DISTINCT title FROM elements", connection);
            var reader = cmd.ExecuteReader();

            //STORE TITLES
            _elemTitles = new List<string>();
            while (reader.Read())
            {
                //Store the value of the first column
                _elemTitles.Add(reader.GetString(0));
            }

            //DISCONNECT FROM DB
            cmd.Dispose();
            connection.Close();
        }

        public List<string> getTitlesOffline()
        {
            _elemTitles = new List<string>();
            foreach (Element elem in db)
                _elemTitles.Add(elem.title);
            return _elemTitles;
        }

        public Element getElement(string title)
        {
            throw new NotImplementedException();
        }

        public Element getElementOffline(string title)
        {
            for (int i = 0; i < db.Count; i++)
            {
                if (_elemTitles[i] == title)
                    return db[i];
            }
            return new Element(title);
        }

        public bool isElementExist(string title)
        {
            for (int i = 0; i < elemTitles.Count; i++)
            {
                if (_elemTitles[i] == title)
                    return true;
            }
            return false;
        }

        public List<string> matchingResearch(string txt)
        {
            List<string> matchList = new List<string>();
            foreach (string s in elemTitles)
            {
                if (s.Contains(txt))
                    matchList.Add(s);
            }
            return matchList;
        }

        private int getElementID(string title)
        {
            for (int i = 0; i < db.Count; i++)
            {
                if (_elemTitles[i] == title)
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
            catch (JsonSerializationException) { _db = new List<Element>(); }
        }

        public void saveJson()
        {
            string jsonContent = JsonConvert.SerializeObject(new { db }, Formatting.None);
            FileManager.saveJson(jsonContent);
        }

        public void copyJsonToDesktop()
        {
            string path = DirectoryManager.DESKTOP;
            FileManager.copyJson(path);
        }

        public void loadJson(string path)
        {
            string jsonContent = FileManager.readTxtFile(path);
            FileManager.saveJson(jsonContent);
            readJson();
        }
        #endregion
    }
}
