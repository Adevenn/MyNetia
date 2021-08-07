using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyNetia.Model
{
    public class Element : INotifyPropertyChanged
    {
        private string _title, _subtitle;
        public string title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }
        public string subtitle
        {
            get => _subtitle;
            set
            {
                if (_subtitle != value)
                {
                    _subtitle = value;
                    OnPropertyChanged();
                }
            }
        }
        private ObservableCollection<Chapter> _chapters = new ObservableCollection<Chapter>();
        public ObservableCollection<Chapter> chapters
        {
            get => _chapters;
            set
            {
                if (_chapters != value)
                {
                    _chapters = value;
                    OnPropertyChanged();
                }
            }
        }
        public DateTime lastUpdate { get; private set; }

        public Element(string title)
        {
            this.title = title;
            this.subtitle = "";
            addChapter();
            this.lastUpdate = DateTime.Now;
        }

        public Element(string title, string subtitle, ObservableCollection<Chapter> chapters, DateTime lastUpdate)
        {
            this.title = title;
            this.subtitle = subtitle;
            this.chapters = chapters;
            this.lastUpdate = lastUpdate;
        }

        /// <summary>
        /// Add a new chapter and make sure the new chapter title is unique
        /// </summary>
        public void addChapter()
        {
            int i = 0;
            string title;
            while (true)
            {
                title = "Chapter " + i;
                if (isChapTitleUnique(title))
                {
                    chapters.Add(new Chapter(title));
                    break;
                }
                i++;
            }
        }

        /// <summary>
        /// Return all chapters title of the element
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<string> getChapTitles()
        {
            ObservableCollection<string> titles = new ObservableCollection<string>();
            foreach (Chapter ch in chapters)
                titles.Add(ch.title);
            return titles;
        }

        /// <summary>
        /// Return all texts of the element
        /// </summary>
        /// <returns></returns>
        public List<ObservableCollection<TextManager>> getAllTexts()
        {
            List<ObservableCollection<TextManager>> allTextsList = new List<ObservableCollection<TextManager>>();
            foreach (Chapter ch in chapters)
                allTextsList.Add(ch.texts);
            return allTextsList;
        }

        /// <summary>
        /// Return all images of the element
        /// </summary>
        /// <returns></returns>
        public List<ObservableCollection<ImageManager>> getAllImg()
        {
            List<ObservableCollection<ImageManager>> allImagesList = new List<ObservableCollection<ImageManager>>();
            foreach (Chapter ch in chapters)
                allImagesList.Add(ch.images);
            return allImagesList;
        }

        /// <summary>
        /// Return false if 2 chapters have the same title, else return true
        /// </summary>
        /// <returns></returns>
        public bool checkChapTitles()
        {
            for (int i = 0; i < chapters.Count; i++)
                for (int j = 0; j < chapters.Count; j++)
                    if (chapters[i].title == chapters[j].title && j != i)
                        return false;
            return true;
        }

        /// <summary>
        /// Check if the title already exists as chapter title
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private bool isChapTitleUnique(string title)
        {
            foreach (Chapter ch in chapters)
            {
                if (ch.title == title)
                    return false;
            }
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
