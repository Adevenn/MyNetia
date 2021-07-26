using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace MyNetia.Model
{
    public static class DB_Manager
    {
        public static List<string> elemTitles;
        private static NpgsqlConnection connection;

        public static void setup()
        {
            string connString = "Server = localhost; Port = 5432; Database = MyNetia; Username = Adeven; Password = 1963258740";
            connection = new NpgsqlConnection(connString);
            getTitles();
        }

        #region Data Modifs
        public static async void addElement(string title, string subtitle, List<string> chapTitles, List<List<string>> texts, List<List<byte[]>> images)
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

        public static void updateElement(string oldTitle, string title, string subtitle, List<string> chapTitles, List<List<string>> texts, List<List<byte[]>> images)
        {
            deleteElement(oldTitle);
            addElement(title, subtitle, chapTitles, texts ,images);
        }

        public static async void deleteElement(string title)
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

        #region Data Access
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
            {
                //Store the value of the first column
                elemTitles.Add(reader.GetString(0));
            }

            //DISCONNECT FROM DB
            cmd.Dispose();
            connection.Close();
        }

        /// <summary>
        /// Return the selected element or return a new element if the title doesn't exist
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
                List<Chapter> chapters = new List<Chapter>();
                List<string> chapTitles = new List<string>();
                List<string> txtList;
                List<byte[]> imgList;

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
                        txtList = new List<string>();
                        cmd = new NpgsqlCommand("SELECT string FROM texts WHERE idelem = @p AND idchap = @p2", connection);
                        cmd.Parameters.AddWithValue("p", title);
                        cmd.Parameters.AddWithValue("p2", ch);
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                            txtList.Add(reader.GetString(1));
                        connection.Close();

                        //SELECT IMAGES
                        connection.Open();
                        imgList = new List<byte[]>();
                        cmd = new NpgsqlCommand("SELECT image FROM images WHERE idelem = @p AND idchap = @p2", connection);
                        cmd.Parameters.AddWithValue("p", title);
                        cmd.Parameters.AddWithValue("p2", ch);
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                            imgList.Add(System.Text.Encoding.ASCII.GetBytes(reader.GetString(1)));
                        connection.Close();

                        chapters.Add(new Chapter(ch, txtList, imgList));
                    }
                    return new Element(title, subtitle, chapters, lastUpdate);
                }
                catch(NpgsqlException e) { throw new NpgsqlException(e.Message); }
            }
            return new Element(title);
        }

        public static bool isElementExist(string title)
        {
            for (int i = 0; i < elemTitles.Count; i++)
            {
                if (elemTitles[i] == title)
                    return true;
            }
            return false;
        }

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
        #endregion
    }
}
