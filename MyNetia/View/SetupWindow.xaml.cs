using MyNetia.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace MyNetia.View
{
    public partial class SetupWindow : Window, INotifyPropertyChanged
    {
        private readonly Regex ipv4Regex = new Regex(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$");
        private int _themes;
        private string _server, _port, _database, _userName;
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

        public SetupWindow()
        {
            DataContext = this;
            InitializeComponent();
            setupValues();
        }

        /// <summary>
        /// Auto-complete with the last values registered
        /// </summary>
        private void setupValues()
        {
            if (UserSettings.theme == "") themes = 0;
            else themes = int.Parse(UserSettings.theme);
            server = UserSettings.serverIP;
            port = UserSettings.port;
            database = UserSettings.database;
            userName = UserSettings.userName;
        }

        /// <summary>
        /// Save user settings and check connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void valid_Click(object sender, RoutedEventArgs e)
        {
            if (checkValues())
            {
                UserSettings.theme = themes.ToString();
                UserSettings.serverIP = server;
                UserSettings.port = port;
                UserSettings.database = database;
                UserSettings.userName = userName;
                UserSettings.password = passwordBox.Password;

                if (DB_Manager.testConnection())
                {
                    ResearchWindow research = new ResearchWindow();
                    research.Show();
                    Close();
                }
                else
                {
                    InfoWindow info = new InfoWindow("Connection failed, verify your infos");
                    info.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Check values and show if an error occurs
        /// </summary>
        /// <returns></returns>
        private bool checkValues()
        {
            bool isValid = true;
            if (!ipv4Regex.Match(server).Success)
            {
                ipError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else { ipError.Visibility = Visibility.Hidden; }
            if (string.IsNullOrWhiteSpace(port)) //Make a Regex
            {
                portError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else { portError.Visibility = Visibility.Hidden; }
            if (string.IsNullOrWhiteSpace(database))
            {
                dbError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else { dbError.Visibility = Visibility.Hidden; }
            if (string.IsNullOrWhiteSpace(userName))
            {
                userNameError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else { userNameError.Visibility = Visibility.Hidden; }
            if (string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                pswError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else { pswError.Visibility = Visibility.Hidden; }
            return isValid;
        }

        #region TITLE BAR

        #region EVENTS
        private void minBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void maxBtn_Click(object sender, RoutedEventArgs e)
        {
            AdjustWindowSize();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void titleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                if (e.ClickCount == 2)
                    AdjustWindowSize();
                else if (WindowState == WindowState.Maximized)
                    WindowState = WindowState.Normal;
                else
                    DragMove();
        }
        #endregion

        #region OTHERS METHODS
        private void AdjustWindowSize()
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }
        #endregion

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
