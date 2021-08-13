using MyNetia.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyNetia
{
    public partial class ResearchWindow : Window, INotifyPropertyChanged
    {
        private readonly App currentApp = (App)Application.Current;
        private string _selection = "";
        public string selection
        {
            get => _selection;
            set
            {
                if(_selection != value)
                {
                    _selection = value;
                    OnPropertyChanged();
                }
            }
        }
        private ObservableCollection<string> _matchingResearch;
        public ObservableCollection<string> matchingResearch
        {
            get => _matchingResearch;
            set
            {
                if (value != _matchingResearch)
                {
                    _matchingResearch = value;
                    OnPropertyChanged();
                }
            }
        }

        public ResearchWindow()
        {
            DB_Manager.getTitles();
            DataContext = this;
            InitializeComponent();
            txtBox.Focus();
            txtBox.Select(0, 0);
            matchingResearchUpdate();
        }

        #region EVENTS

        /// <summary>
        /// Update match list when the selection text change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void onTextChanged(object sender, TextChangedEventArgs args)
        {
            matchingResearchUpdate();
        }

        /// <summary>
        /// Apply the command when Enter is pressed, Auto-complete with the first item of match list when Tab is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                DB_Manager.getTitles();
                matchingResearchUpdate();
                if (DB_Manager.checkTitleAvailablity(selection))
                {
                    if (!currentApp.isOpenWindow(selection))
                    {
                        DisplayWindow displayWindow = new DisplayWindow(selection);
                        currentApp.addWindow(displayWindow.Title);
                        displayWindow.Show();
                    }
                }
                if (selection.StartsWith("-") || selection.Equals("Help") || selection.Equals("help") || string.IsNullOrWhiteSpace(selection))
                {
                    switch (selection)
                    {
                        case Commands.admin:
                            AdminWindow adminWindow = new AdminWindow();
                            if (!currentApp.isOpenWindow(adminWindow.Title))
                            {
                                currentApp.addWindow(adminWindow.Title);
                                adminWindow.Show();
                            }
                            break;
                        case Commands.help:
                        default:
                            HelpWindow helpWindow = new HelpWindow();
                            if (!currentApp.isOpenWindow(helpWindow.Title))
                            {
                                currentApp.addWindow(helpWindow.Title);
                                helpWindow.Show();
                            }
                            break;
                    }
                }
            }
        }
        #endregion

        #region OTHERS METHODS

        /// <summary>
        /// Update the match list with the selection
        /// </summary>
        private void matchingResearchUpdate()
        {
            matchingResearch = new ObservableCollection<string>(DB_Manager.matchingResearch(selection));
        }
        #endregion

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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
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
