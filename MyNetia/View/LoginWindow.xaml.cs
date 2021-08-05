using MyNetia.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MyNetia.View
{
    public partial class LoginWindow : Window, INotifyPropertyChanged
    {
        private string _login = UserSettings.userName;
        public string login
        {
            get => _login;
            set
            {
                if (_login != value)
                {
                    _login = value;
                    OnPropertyChanged();
                }
            }
        }

        public LoginWindow()
        {
            DataContext = this;
            InitializeComponent();
        }

        /// <summary>
        /// Verify infos and open research window if infos are corrects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void valid_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                UserSettings.userName = login;
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
        /// Load the setup window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadSetup_Click(object sender, RoutedEventArgs e)
        {
            SetupWindow setup = new SetupWindow();
            setup.Show();
            Close();
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
