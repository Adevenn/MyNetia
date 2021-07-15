using MyNetia.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyNetia
{
    public partial class ResearchWindow : Window
    {
        private readonly App currentApp = (App)Application.Current;
        private readonly InfoBinding binds = new InfoBinding();

        public ResearchWindow()
        {
            DataContext = binds;
            currentApp.dbManager.readJson();
            InitializeComponent();
            helpResearchBar();
        }

        #region EVENTS
        private void onTextChanged(object sender, TextChangedEventArgs args)
        {
            helpResearchBar();
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (currentApp.dbManager.isElementExist(txtBox.Text))
                {
                    DB_Element elem = currentApp.dbManager.getElement(txtBox.Text);
                    DisplayWindow displayWindow = new DisplayWindow(elem.title);
                    if (!currentApp.isOpenWindow(displayWindow.Title))
                    {
                        currentApp.addWindow(displayWindow.Title);
                        displayWindow.Show();
                    }
                }
                if (txtBox.Text.StartsWith("-") || txtBox.Text.Equals("Help") || txtBox.Text.Equals("help") || string.IsNullOrWhiteSpace(txtBox.Text))
                {
                    switch (txtBox.Text)
                    {
                        case Commands.admin:
                            AdminWindow adminWindow = new AdminWindow();
                            if (!currentApp.isOpenWindow(adminWindow.Title))
                            {
                                currentApp.addWindow(adminWindow.Title);
                                adminWindow.Show();
                            }
                            break;
                        case Commands.saveAsJson:
                            currentApp.dbManager.copyJsonToDesktop();
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
            else if (e.Key.Equals(Key.Tab) && binds.matchingResearch.Count > 0)
            {
                txtBox.Text = binds.matchingResearch[0];
                //Set Keyboard focus at the end
                txtBox.CaretIndex = txtBox.Text.Length;
            }
        }
        #endregion

        #region OTHERS METHODS
        private void helpResearchBar()
        {
            binds.matchingResearch = new ObservableCollection<string>();
            foreach (string txt in currentApp.dbManager.getTitles())
            {
                if (txt.Contains(txtBox.Text))
                    binds.matchingResearch.Add(txt);
            }
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

        private class InfoBinding : INotifyPropertyChanged
        {
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

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName] string name = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
