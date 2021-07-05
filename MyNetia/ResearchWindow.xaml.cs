using MyNetia.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyNetia
{
    public partial class ResearchWindow : Window
    {
        private List<string> matchingResearch = null;
        //Only 1 window open at a time
        public static bool isHelpWindowOpen = false;
        public static bool isAdminWindowOpen = false;

        public ResearchWindow()
        {
            InitializeComponent();
            //if (File.Exists(Path.GetFullPath(@".\AppResources\SaveDB.json")))
            //    AppResources.dbManager.readJson();
            ObservableCollection<string> texts = new ObservableCollection<string>
            {
                "TEST",
                "TEST2",
                "TEST3",
                "TEST4",
                "TEST5"
            };
            ObservableCollection<string> images = new ObservableCollection<string>
            {
                "TEST",
                "TEST",
                "TEST",
                "TEST"
            };
            ObservableCollection<Chapter> chapList = new ObservableCollection<Chapter>
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
                    displayWindow.Show();
                }
                if (txtBox.Text.StartsWith("-") || txtBox.Text.Equals("Help") || txtBox.Text.Equals("help") || string.IsNullOrWhiteSpace(txtBox.Text))
                {
                    switch (txtBox.Text)
                    {
                        case Commands.admin: 
                            if (!isAdminWindowOpen)
                            {
                                AdminWindow adminWindow = new AdminWindow();
                                adminWindow.Show();
                                isAdminWindowOpen = true;
                            }
                            break;
                        case Commands.saveAsJson:
                            AppResources.dbManager.saveJson(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                            break;
                        case Commands.help:
                        default:
                            if (!isHelpWindowOpen)
                            {
                                HelpWindow helpWindow = new HelpWindow();
                                helpWindow.Show();
                                isHelpWindowOpen = true;
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
