using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyNetia.Model
{
    public static class DB_Manager
    {
        public static List<string> elemTitles { get; private set; }
        private static NpgsqlConnection connection;

        /// <summary>
        /// Setup connection and app datas
        /// </summary>
        public static void setup()
        {
            string connString = "Server = localhost; Port = 5432; Database = MyNetia; Username = Adeven; Password = 1963258740";
            connection = new NpgsqlConnection(connString);
            getTitles();
        }

        /// <summary>
        /// Add a new element in the database
        /// </summary>
        /// <param name="title"></param>
        /// <param name="subtitle"></param>
        /// <param name="chapTitles"></param>
        /// <param name="texts"></param>
        /// <param name="images"></param>
        public static async void addElement(string title, string subtitle, ObservableCollection<string> chapTitles, List<ObservableCollection<string>> texts, List<ObservableCollection<byte[]>> images)
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
        public static void updateElement(string oldTitle, string title, string subtitle, ObservableCollection<string> chapTitles, List<ObservableCollection<string>> texts, List<ObservableCollection<byte[]>> images)
        {
            deleteElement(oldTitle);
            addElement(title, subtitle, chapTitles, texts ,images);
        }

        /// <summary>
        /// Delete an element in the database
        /// </summary>
        /// <param name="title"></param>
        public static async void deleteElement(string title)
        {
            try
            {
                connection.Open();

                //DELETE ELEMENT
                NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM elements WHERE title=(@p)", connection);
                cmd.Parameters.AddWithValue("p", title);
                await cmd.ExecuteNonQueryAsync();

                cmd.Dispose();
                connection.Close();
            }
            catch (NpgsqlException e) { throw new NpgsqlException(e.Message); }
        }

        /// <summary>
        /// Select every element title in the database and store them in elemTitles
        /// </summary>
        public static void getTitles()
        {
            //Connect to DB
            connection.Open();

            //SELECT TITLES
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT title FROM elements ORDER BY title", connection);
            var reader = cmd.ExecuteReader();

            //STORE TITLES
            elemTitles = new List<string>();
            while (reader.Read())
                elemTitles.Add(reader.GetString(0));

            //DISCONNECT FROM DB
            cmd.Dispose();
            connection.Close();
        }

        /// <summary>
        /// Select the element if it exists, else return a new element
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static Element getElement(string title)
        {
            if (isElementExist(title))
            {
                NpgsqlCommand cmd;
                NpgsqlDataReader reader;
                string subtitle;
                DateTime lastUpdate;
                ObservableCollection<Chapter> chapters = new ObservableCollection<Chapter>();
                ObservableCollection<string> chapTitles = new ObservableCollection<string>();
                ObservableCollection<string> txtList;
                ObservableCollection<byte[]> imgList;

                try
                {
                    //SELECT ELEMENT
                    connection.Open();
                    cmd = new NpgsqlCommand("SELECT subtitle, lastupdate FROM elements WHERE title = @p;", connection);
                    cmd.Parameters.AddWithValue("p", title);
                    reader = cmd.ExecuteReader();
                    reader.Read();
                    subtitle = reader.GetString(0);
                    lastUpdate = reader.GetDateTime(1);
                    connection.Close();
                
                    //SELECT CHAPTERS
                    connection.Open();
                    cmd = new NpgsqlCommand("SELECT title FROM chapters WHERE idelem = @p", connection);
                    cmd.Parameters.AddWithValue("p", title);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                        chapTitles.Add(reader.GetString(0));
                    connection.Close();

                    //STORE CHAPTERS DATA
                    foreach (string ch in chapTitles)
                    {
                        //SELECT TEXTS
                        connection.Open();
                        txtList = new ObservableCollection<string>();
                        cmd = new NpgsqlCommand("SELECT string FROM texts WHERE idelem = @p AND idchap = @p2", connection);
                        cmd.Parameters.AddWithValue("p", title);
                        cmd.Parameters.AddWithValue("p2", ch);
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                            txtList.Add(reader.GetString(0));
                        connection.Close();

                        //SELECT IMAGES
                        connection.Open();
                        imgList = new ObservableCollection<byte[]>();
                        cmd = new NpgsqlCommand("SELECT image FROM images WHERE idelem = @p AND idchap = @p2", connection);
                        cmd.Parameters.AddWithValue("p", title);
                        cmd.Parameters.AddWithValue("p2", ch);
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                            imgList.Add(System.Text.Encoding.ASCII.GetBytes(reader.GetString(0)));
                        connection.Close();

                        chapters.Add(new Chapter(ch, txtList, imgList));
                    }
                    return new Element(title, subtitle, chapters, lastUpdate);
                }
                catch(NpgsqlException e) { throw new NpgsqlException(e.Message); }
            }
            return new Element(title);
        }

        /// <summary>
        /// Return true if the element exits, else return false
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static bool isElementExist(string title)
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
        public static List<string> matchingResearch(string txt)
        {
            List<string> matchList = new List<string>();
            foreach (string s in elemTitles)
            {
                if (s.Contains(txt))
                    matchList.Add(s);
            }
            return matchList;
        }
    }
}
