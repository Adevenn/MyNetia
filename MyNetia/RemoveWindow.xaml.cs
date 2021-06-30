using MyNetia.Model;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyNetia
{
    public partial class RemoveWindow : Window
    {
        List<string> matchingResearch = null;
        public RemoveWindow()
        {
            InitializeComponent();
            helpResearchBar();
        }

        #region EVENTS
        private void selectionTxtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (AppResources.dbManager.isExist(selectionTxtBox.Text))
                {
                    MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to remove this element ?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        DirectoryManager.deleteDirectory(Path.GetFullPath(@".\AppResources\Images\" + selectionTxtBox.Text));
                        AppResources.dbManager.remove(selectionTxtBox.Text);
                        helpResearchBar();
                    }
                }
            }
            else if (e.Key == Key.Tab && matchingResearch.Count > 0)
            {
                selectionTxtBox.Text = matchingResearch[0];
                //Set Keyboard focus at the end
                selectionTxtBox.CaretIndex = selectionTxtBox.Text.Length;
            }
        }
        private void onTextChanged(object sender, TextChangedEventArgs e)
        {
            helpResearchBar();
        }
        #endregion

        #region OTHERS METHODS
        private void helpResearchBar()
        {
            matchingResearch = new List<string>();
            txtBlock.Text = null;
            foreach (string txt in AppResources.dbManager.getNames())
            {
                if (txt.Contains(selectionTxtBox.Text))
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
            ResearchWindow.isRemoveWindowOpen = false;
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
