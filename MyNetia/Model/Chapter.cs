using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyNetia.Model
{
    [Serializable]
    public class Chapter : INotifyPropertyChanged
    {
        public int id;
        private string _chapTitle;
        public string chapTitle
        {
            get
            {
                return _chapTitle;
            }

            set
            {
                if(_chapTitle != value)
                {
                    _chapTitle = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<string> texts;
        public ObservableCollection<string> images;

        public Chapter()
        {
            chapTitle = "New chapter";
            texts = new ObservableCollection<string>
            {
                "NEW TEXT CONTENT"
            };
            images = new ObservableCollection<string>
            {
                "NEW IMAGE CONTENT"
            };
        }

        public Chapter(string chapTitle, ObservableCollection<string> texts, ObservableCollection<string> images)
        {
            this.chapTitle = chapTitle;
            this.texts = texts;
            this.images = images;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
