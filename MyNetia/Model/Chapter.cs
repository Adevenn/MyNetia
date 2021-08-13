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
        private ObservableCollection<TextManager> _texts;
        public ObservableCollection<TextManager> texts
        {
            get => _texts;
            set
            {
                if(_texts != value)
                {
                    _texts = value;
                    OnPropertyChanged();
                }
            }
        }
        private ObservableCollection<ImageManager> _images;
        public ObservableCollection<ImageManager> images
        {
            get => _images;
            set
            {
                if(_images != value)
                {
                    _images = value;
                    OnPropertyChanged();
                }
            }
        }

        public Chapter(string title)
        {
            this.title = title;
            this.texts = new ObservableCollection<TextManager>();
            this.texts.Add(new TextManager(TypesTxt.none));
            this.images = new ObservableCollection<ImageManager>();
            this.images.Add(new ImageManager());
        }

        public Chapter(string title, ObservableCollection<TextManager> texts, ObservableCollection<ImageManager> images)
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
