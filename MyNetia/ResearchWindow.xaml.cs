using MyNetia.Model;
using System.Collections.Generic;
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
            InitializeComponent();
            currentApp.dbManager.readJson();

            //TESTS
            List<string> texts = new List<string>
            {
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean id gravida purus. Mauris tempor consequat neque, eu facilisis ante blandit at. Quisque faucibus et est mollis bibendum. Nullam sagittis viverra justo, ac consequat velit dapibus eget. Nullam non mauris risus. Morbi sed nisl nec elit luctus porttitor. Fusce purus lorem, dictum sit amet luctus nec, tincidunt in sem. Aenean non odio urna. Suspendisse elementum finibus tellus nec dapibus. Maecenas commodo quam eu nisi bibendum convallis eget vitae massa. Cras porta tortor eu purus volutpat ultrices. Sed eget enim vel nunc egestas mollis congue eget orci. Vivamus mollis sem a eleifend laoreet. Etiam id dictum velit. Pellentesque congue libero justo, in sodales velit mattis quis.",
                "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Curabitur egestas leo vel lectus pulvinar, et mattis urna varius. Praesent cursus posuere mauris, id vulputate est pharetra eu. Maecenas pharetra nisl ac imperdiet rhoncus. Vestibulum magna lacus, elementum vel aliquet nec, rutrum mollis quam. Etiam lacinia tempor pellentesque. Sed ut volutpat erat. In ligula nibh, auctor ut hendrerit eget, scelerisque sed lectus. Donec erat lacus, tincidunt sit amet mi ac, convallis ornare nibh. Fusce lobortis erat massa, sit amet suscipit ligula luctus at. Suspendisse sollicitudin placerat risus ac tincidunt. Sed ut magna orci. In hac habitasse platea dictumst. Maecenas lectus nisl, porttitor et tellus sit amet, vestibulum faucibus ante.",
                "TEST3",
                "TEST4",
                "TEST5"
            };
            List<string> images = new List<string>
            {
                "image1.png",
                "image2.jpg",
                "image3.png",
                "TEST"
            };
            List<Chapter> chapList = new List<Chapter>
            {
                new Chapter("TEST CHAPTER", texts, images),
                new Chapter("TEST CHAPTERS", texts, images)
            };
            currentApp.dbManager.addElement("A", "port 123", chapList);
            currentApp.dbManager.addElement("B", "port 123", chapList);
            currentApp.dbManager.addElement("C", "port 123", chapList);
            currentApp.dbManager.addElement("DAAAAAAAAA", "port 123", chapList);
            currentApp.dbManager.addElement("EAAAAAAAAA", "port 123", chapList);
            currentApp.dbManager.addElement("FAAAAAAAAA", "port 123", chapList);
            currentApp.dbManager.addElement("GAAAAAAAAA", "port 123", chapList);
            currentApp.dbManager.addElement("HAAAAAAAAA", "port 123", chapList);
            currentApp.dbManager.addElement("IAAAAAAAAA", "port 123", chapList);
            currentApp.dbManager.addElement("JAAAAAAAAA", "port 123", chapList);
            //END TESTS

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
