using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyNetia.Model
{
    public class ImageManager : INotifyPropertyChanged
    {
        private string _fileName;
        public string fileName
        {
            get => _fileName;
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    OnPropertyChanged();
                }
            }
        }
        private byte[] _datas;
        public byte[] datas
        {
            get => _datas;
            set
            {
                if (_datas != value)
                {
                    _datas = value;
                    OnPropertyChanged();
                }
            }
        }

        public ImageManager()
        {
            fileName = "TEST";
            datas = new byte[0];
        }

        public ImageManager(string fileName, byte[] datas)
        {
            this.fileName = fileName;
            this.datas = datas;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
