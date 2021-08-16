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
        private int _type;
        public int type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged();
                }
            }
        }


        public ImageManager()
        {
            fileName = "";
            datas = new byte[0];
            type = (int)TypesImage.none;
        }

        public ImageManager(string fileName, byte[] datas, TypesImage type)
        {
            this.fileName = fileName;
            this.datas = datas;
            this.type = (int)type;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
