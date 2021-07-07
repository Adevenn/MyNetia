using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyNetia.Model
{
    [Serializable]
    public class Chapter : INotifyPropertyChanged
    {
        private string _chapTitle;
        public string chapTitle
        {
            get => _chapTitle;
            set
            {
                if(_chapTitle != value)
                {
                    _chapTitle = value;
                    OnPropertyChanged();
                }
            }
        }
        public List<string> texts;
        public List<string> images;

        public Chapter()
        {
            chapTitle = "New chapter";
            texts = new List<string>
            {
                ""
            };
            images = new List<string>
            {
                ""
            };
        }

        public Chapter(string chapTitle, List<string> texts, List<string> images)
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
