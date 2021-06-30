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
        private List<string> matchingResearch = null;
        //Only 1 window open at a time
        public static bool isUpdateWindowOpen = false;
        public static bool isHelpWindowOpen = false;
        public static bool isAddWindowOpen = false;
        public static bool isRemoveWindowOpen = false;
        public ResearchWindow()
        {
            InitializeComponent();
            if (File.Exists(Path.GetFullPath(@".\AppResources\SaveDB.json")))
                AppResources.dbManager.readJson();
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
                if (AppResources.dbManager.isExist(txtBox.Text))
                {
                    DB_Element element = AppResources.dbManager.getElement(txtBox.Text);
                    DisplayWindow displayWindow = new DisplayWindow(element.name);
                    displayWindow.Show();
                }
                if (txtBox.Text.StartsWith("-") || txtBox.Text.Equals("Help") || txtBox.Text.Equals("help") || string.IsNullOrWhiteSpace(txtBox.Text))
                {
                    switch (txtBox.Text)
                    {
                        case Commands.add:
                            if (!isAddWindowOpen)
                            {
                                AddWindow addWindow = new AddWindow();
                                addWindow.Show();
                                isAddWindowOpen = true;
                            }
                            break;
                        case Commands.remove:
                            if (!isRemoveWindowOpen)
                            {
                                RemoveWindow removeWindow = new RemoveWindow();
                                removeWindow.Show();
                                isRemoveWindowOpen = true;
                            }
                            break;
                        case Commands.saveAsJson:
                            AppResources.dbManager.saveAsJson(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                            break;
                        case Commands.update:
                            if (!isUpdateWindowOpen)
                            {
                                UpdateWindow updateWindow = new UpdateWindow();
                                updateWindow.Show();
                                isUpdateWindowOpen = true;
                            }
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
            foreach (string txt in AppResources.dbManager.getNames())
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
        private void titleBar_MouseEnter(object sender, MouseEventArgs e)
        {
            titleBar.Opacity = 1;
        }
        private void titleBar_MouseLeave(object sender, MouseEventArgs e)
        {
            titleBar.Opacity = 0;
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
