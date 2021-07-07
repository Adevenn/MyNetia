using MyNetia.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyNetia
{
    public partial class ResearchWindow : Window
    {
        private App currentApp = (App)Application.Current;
        private List<string> matchingResearch = null;

        public ResearchWindow()
        {
            InitializeComponent();
            if (File.Exists(Path.GetFullPath(@".\AppResources\SaveDB.json")))
                AppResources.dbManager.readJson();

            List<string> texts = new List<string>
            {
                "TEST",
                "TEST2",
                "TEST3",
                "TEST4",
                "TEST5"
            };
            List<string> images = new List<string>
            {
                "TEST",
                "TEST",
                "TEST",
                "TEST"
            };
            List<Chapter> chapList = new List<Chapter>
            {
                new Chapter("TEST CHAPTER", texts, images),
                new Chapter("TEST CHAPTERS", texts, images)
            };
            AppResources.dbManager.addElement("A", "port 123", chapList);
            AppResources.dbManager.addElement("B", "port 123", chapList);
            AppResources.dbManager.addElement("C", "port 123", chapList);
            AppResources.dbManager.addElement("DAAAAAAAAA", "port 123", chapList);
            AppResources.dbManager.addElement("EAAAAAAAAA", "port 123", chapList);
            AppResources.dbManager.addElement("FAAAAAAAAA", "port 123", chapList);
            AppResources.dbManager.addElement("GAAAAAAAAA", "port 123", chapList);
            AppResources.dbManager.addElement("HAAAAAAAAA", "port 123", chapList);
            AppResources.dbManager.addElement("IAAAAAAAAA", "port 123", chapList);
            AppResources.dbManager.addElement("JAAAAAAAAA", "port 123", chapList);

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
                if (AppResources.dbManager.isElementExist(txtBox.Text))
                {
                    DB_Element elem = AppResources.dbManager.getElement(txtBox.Text);
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
                            AppResources.dbManager.saveJson(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
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
            else if (e.Key.Equals(Key.Tab) && matchingResearch.Count > 0)
            {
                txtBox.Text = matchingResearch[0];
                //Set Keyboard focus at the end
                txtBox.CaretIndex = txtBox.Text.Length;
            }
        }
        #endregion

        #region OTHERS METHODS
        private void helpResearchBar()
        {
            matchingResearch = new List<string>();
            txtBlock.Text = null;
            foreach (string txt in AppResources.dbManager.getTitles())
            {
                if (txt.Contains(txtBox.Text))
                {
                    txtBlock.Text += txt + "\n";
                    matchingResearch.Add(txt);
                }
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

    }
}
