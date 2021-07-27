using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyNetia.Model
{
    public class Chapter : INotifyPropertyChanged
    {
        private string _title;
        public string title
        {
            get => _title;
            set
            {
                if(_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<string> texts;
        public ObservableCollection<byte[]> images;

        public Chapter(string title)
        {
            this.title = "Chapter " + title;
            this.texts = new ObservableCollection<string>();
            this.texts.Add("");
            this.images = new ObservableCollection<byte[]>();
            this.images.Add(new byte[0]);
        }

        public Chapter(string title, ObservableCollection<string> texts, ObservableCollection<byte[]> images)
        {
            this.title = title;
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
