using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text.RegularExpressions;

namespace MyNetia.Model
{
    public static class DB_Manager
    {
        public static List<string> elemTitles { get; private set; }
        private static NpgsqlConnection connection;

        /// <summary>
        /// Return true if connection succeed, false if fails
        /// </summary>
        /// <returns></returns>
        public static bool testConnection()
        {
            string connString = $"Server = {UserSettings.serverIP}; Port = {UserSettings.port}; Database = {UserSettings.database}; Username = {UserSettings.userName}; Password = {UserSettings.password}";
            connection = new NpgsqlConnection(connString);
            try
            {
                connection.Open();
                connection.Close();
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Add a new element in the database
        /// </summary>
        /// <param name="title"></param>
        /// <param name="subtitle"></param>
        /// <param name="chapTitles"></param>
        /// <param name="texts"></param>
        /// <param name="images"></param>
        public static void addElement(Element elem)
        {
            /* Integrity constraint
             * Element title MUST be unique
             * Chapter title MUST be unique inside an element
             */
            try
            {
                lock (connection)
                {
                    //Connect to DB
                    connection.Open();

                    //INSERT NEW ELEMENT
                    NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO elements (title, subtitle, lastupdate) VALUES (@p, @p2, @p3)", connection);
                    cmd.Parameters.AddWithValue("p", elem.title);
                    cmd.Parameters.AddWithValue("p2", elem.subtitle);
                    cmd.Parameters.AddWithValue("p3", DateTime.Now);
                    cmd.ExecuteNonQuery();

                    //INSERT CHAPTERS
                    ObservableCollection<string> chapTitles = elem.getChapTitles();
                    List<ObservableCollection<TextManager>> texts = elem.getAllTexts();
                    List<ObservableCollection<ImageManager>> images = elem.getAllImg();
                    for (int i = 0; i < chapTitles.Count; i++)
                    {
                        cmd = new NpgsqlCommand("INSERT INTO chapters (title, idelem) VALUES (@p, @p2)", connection);
                        cmd.Parameters.AddWithValue("p", chapTitles[i]);
                        cmd.Parameters.AddWithValue("p2", elem.title);
                        cmd.ExecuteNonQuery();

                        //INSERT TEXTS
                        foreach (TextManager t in texts[i])
                        {
                            cmd = new NpgsqlCommand("INSERT INTO texts (idchap, idelem, type, txt) VALUES (@p, @p2, @p3, @p4)", connection);
                            cmd.Parameters.AddWithValue("p", chapTitles[i]);
                            cmd.Parameters.AddWithValue("p2", elem.title);
                            cmd.Parameters.AddWithValue("p3", t.type);
                            cmd.Parameters.AddWithValue("p4", t.text);
                            cmd.ExecuteNonQuery();
                        }

                        //INSERT IMAGES
                        foreach (ImageManager img in images[i])
                        {
                            cmd = new NpgsqlCommand("INSERT INTO images (idchap, idelem, filename, image, type) VALUES (@p, @p2, @p3, @p4, @p5)", connection);
                            cmd.Parameters.AddWithValue("p", chapTitles[i]);
                            cmd.Parameters.AddWithValue("p2", elem.title);
                            cmd.Parameters.AddWithValue("p3", img.fileName);
                            cmd.Parameters.AddWithValue("p4", img.datas);
                            cmd.Parameters.AddWithValue("p5", img.type);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    //DISCONNECT FROM DB
                    cmd.Dispose();
                    connection.Close();
                }
            }
            catch (NpgsqlException e) { throw new NpgsqlException(e.Message); }
        }

        /// <summary>
        /// Update an element in the database
        /// </summary>
        /// <param name="oldTitle"></param>
        /// <param name="title"></param>
        /// <param name="subtitle"></param>
        /// <param name="chapTitles"></param>
        /// <param name="texts"></param>
        /// <param name="images"></param>
        public static void updateElement(string oldTitle, Element elem)
        {
            deleteElement(oldTitle);
            addElement(elem);
        }

        /// <summary>
        /// Delete an element in the database
        /// </summary>
        /// <param name="title"></param>
        public static void deleteElement(string title)
        {
            try
            {
                lock (connection)
                {
                    //CONNECT TO DB
                    connection.Open();

                    //DELETE ELEMENT
                    NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM elements WHERE title=(@p)", connection);
                    cmd.Parameters.AddWithValue("p", title);
                    cmd.ExecuteNonQuery();

                    //DISCONNECT FROM DB
                    cmd.Dispose();
                    connection.Close();
                }
            }
            catch (NpgsqlException e) { throw new NpgsqlException(e.Message); }
        }

        /// <summary>
        /// Select every element title in the database and store them in elemTitles
        /// </summary>
        public static void getTitles()
        {
            lock (connection)
            {
                //Connect to DB
                connection.Open();

                //SELECT TITLES
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT title FROM elements ORDER BY title", connection);
                NpgsqlDataReader reader = cmd.ExecuteReader();

                //STORE TITLES
                elemTitles = new List<string>();
                while (reader.Read())
                    elemTitles.Add(reader.GetString(0));

                //DISCONNECT FROM DB
                cmd.Dispose();
                connection.Close();
            }
        }

        /// <summary>
        /// Select the element if it exists, else return a new element
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static Element getElement(string title)
        {
            if (checkTitleAvailablity(title))
            {
                string subtitle;
                DateTime lastUpdate;
                ObservableCollection<Chapter> chapters = new ObservableCollection<Chapter>();
                ObservableCollection<string> chapTitles;
                ObservableCollection<TextManager> txtList;
                ObservableCollection<ImageManager> imgList;

                //SELECT ELEMENT && CHAPTERS TITLE
                object[] values = getValues(title);
                subtitle = (string)values[0];
                lastUpdate = (DateTime)values[1];
                chapTitles = (ObservableCollection<string>)values[2];

                //STORE CHAPTERS DATAS
                foreach (string ch in chapTitles)
                {
                    txtList = getTextsFromChapter(title, ch);
                    imgList = getImagesFromChapter(title, ch);
                    chapters.Add(new Chapter(ch, txtList, imgList));
                }
                return new Element(title, subtitle, chapters, lastUpdate);
            }
            return new Element(title);
        }

        /// <summary>
        /// Get subtitle and last update from idElem in database
        /// </summary>
        /// <param name="idElem"></param>
        /// <returns></returns>
        private static object[] getValues(string idElem)
        {
            object[] values = new object[3];
            try
            {
                connection.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT e.subtitle, e.lastupdate, c.title FROM elements as e, chapters as c WHERE e.title = c.idelem AND e.title = @p", connection);
                cmd.Parameters.AddWithValue("p", idElem);
                DataTable dt = new DataTable();
                NpgsqlDataAdapter nda = new NpgsqlDataAdapter(cmd);
                nda.Fill(dt);
                connection.Close();
                values[0] = (string)dt.Rows[0]["subtitle"];
                values[1] = (DateTime)dt.Rows[0]["lastupdate"];
                ObservableCollection<string> ch = new ObservableCollection<string>();
                foreach (DataRow row in dt.Rows)
                    ch.Add((string)row["title"]);
                values[2] = ch;
                return values;
            }
            catch (NpgsqlException e) { throw new NpgsqlException(e.Message); }
        }

        /// <summary>
        /// Get all texts from a chapter
        /// </summary>
        /// <param name="idElem"></param>
        /// <param name="idChap"></param>
        /// <returns></returns>
        private static ObservableCollection<TextManager> getTextsFromChapter(string idElem, string idChap)
        {
            ObservableCollection<TextManager> txtList = new ObservableCollection<TextManager>();
            try
            {
                connection.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT type, txt FROM texts WHERE idelem = @p AND idchap = @p2", connection);
                cmd.Parameters.AddWithValue("p", idElem);
                cmd.Parameters.AddWithValue("p2", idChap);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int type = reader.GetInt32(0);
                    if (type == 0)
                        txtList.Add(new TextManager(TypesTxt.none));
                    else
                        txtList.Add(new TextManager((TypesTxt)type, reader.GetString(1)));
                }
                connection.Close();
                return txtList;
            }
            catch (NpgsqlException e) { throw new NpgsqlException(e.Message); }
        }

        /// <summary>
        /// Get all images from a chapter
        /// </summary>
        /// <param name="idElem"></param>
        /// <param name="idChap"></param>
        /// <returns></returns>
        private static ObservableCollection<ImageManager> getImagesFromChapter(string idElem, string idChap)
        {
            try
            {
                ObservableCollection<ImageManager> imgList = new ObservableCollection<ImageManager>();
                connection.Open();
                imgList = new ObservableCollection<ImageManager>();
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT filename, image, type FROM images WHERE idelem = @p AND idchap = @p2", connection);
                cmd.Parameters.AddWithValue("p", idElem);
                cmd.Parameters.AddWithValue("p2", idChap);
                DataTable dt = new DataTable();
                NpgsqlDataAdapter nda = new NpgsqlDataAdapter(cmd);
                nda.Fill(dt);
                connection.Close();
                foreach (DataRow row in dt.Rows)
                    imgList.Add(new ImageManager((string)row["filename"], (byte[])row["image"], (TypesImage)row["type"]));
                return imgList;
            }
            catch (NpgsqlException e) { throw new NpgsqlException(e.Message); }
        }

        /// <summary>
        /// Return true if the element exits, else return false
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static bool checkTitleAvailablity(string title)
        {
            for (int i = 0; i < elemTitles.Count; i++)
            {
                if (elemTitles[i] == title)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Return a list of string matching with the parameter
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static ObservableCollection<string> matchingResearch(string txt)
        {
            ObservableCollection<string> matchList = new ObservableCollection<string>();
            foreach (string s in elemTitles)
            {
                if (s.Contains(txt))
                    matchList.Add(s);
            }
            return matchList;
        }

        /// <summary>
        /// Return true if the name is valid, return false if the name is not valid
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool isValidName(string name)
        {
            Regex validCharacters = new Regex(@"[a-zA-Z0-9_-]+$");
            return validCharacters.IsMatch(name) && !string.IsNullOrWhiteSpace(name);
        }
    }
}
