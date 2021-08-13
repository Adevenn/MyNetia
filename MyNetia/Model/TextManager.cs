using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyNetia.Model
{
    public class TextManager : INotifyPropertyChanged
    {
        private string _text = "";
        public string text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _type;
        public int type
        {
            get => _type;
            set
            {
                if(_type != value)
                {
                    _type = value;
                    OnPropertyChanged();
                }
            }
        }

        public TextManager(TypesTxt t, string value = null)
        {
            type = (int)t;
            if (type != 0)
                text = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
