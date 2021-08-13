using MyNetia.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MyNetia.View
{
    public partial class LoginWindow : Window, INotifyPropertyChanged
    {
        private string _userName = UserSettings.userName;
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

        public LoginWindow()
        {
            DataContext = this;
            InitializeComponent();
            tBoxUserName.Focus();
            tBoxUserName.Select(tBoxUserName.Text.Length, 0);
        }

        /// <summary>
        /// Try to login
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void valid_Click(object sender, RoutedEventArgs e)
        {
            login();
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

        /// <summary>
        /// Try to login when Enter is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                login();
        }

        /// <summary>
        /// Check values and show if an error occurs
        /// </summary>
        /// <returns></returns>
        private bool checkValues()
        {
            bool isValid = true;
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

        /// <summary>
        /// Verify infos and open research window if infos are corrects
        /// </summary>
        private void login()
        {
            if (checkValues())
            {
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
                    InfoWindow info = new InfoWindow("Connection failed, verify your infos")
                    {
                        Owner = this
                    };
                    info.ShowDialog();
                }
            }
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
