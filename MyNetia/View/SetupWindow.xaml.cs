using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MyNetia.View
{
    public partial class SetupWindow : Window, INotifyPropertyChanged
    {
        private int _themes;
        public int themes
        {
            get => _themes;
            set
            {
                if (_themes != value)
                {
                    _themes = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _server;
        public string server
        {
            get => _server;
            set
            {
                if (_server != value)
                {
                    _server = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _port;
        public string port
        {
            get => _port;
            set
            {
                if (_port != value)
                {
                    _port = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _database;
        public string database
        {
            get => _database;
            set
            {
                if (_database != value)
                {
                    _database = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _userName;
        public string userName
        {
            get => _userName;
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _password;
        public string password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }

        public SetupWindow()
        {
            DataContext = this;
            InitializeComponent();
        }

        /// <summary>
        /// Save userdatas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void valid_Click(object sender, RoutedEventArgs e)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
