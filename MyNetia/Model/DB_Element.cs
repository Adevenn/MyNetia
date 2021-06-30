using System;
using System.Collections.Generic;

namespace MyNetia.Model
{
    [Serializable]
    public class DB_Element
    {
        public string name;
        public string port;
        public List<string> theoryTxt = new List<string>();
        public List<string> hackingTxt = new List<string>();
        public List<string> theoryImg = new List<string>();
        public List<string> hackingImg = new List<string>();
        public DateTime lastUpdate = new DateTime();

        public DB_Element(string name, string port, List<string> theoryTxt, List<string> hackingTxt, List<string> theoryImg, List<string> hackingImg, DateTime date)
        {
            this.name = name;
            this.port = port;
            this.theoryTxt = theoryTxt;
            this.hackingTxt = hackingTxt;
            this.theoryImg = theoryImg;
            this.hackingImg = hackingImg;
            lastUpdate = date;
        }
    }
}
