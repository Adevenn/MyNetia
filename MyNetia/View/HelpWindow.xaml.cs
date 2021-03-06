using MyNetia.Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyNetia
{
    public partial class HelpWindow : Window
    {
        private readonly App currentApp = (App)Application.Current;
        public HelpWindow()
        {
            InitializeComponent();
            showCommands();
            showElements();
        }

        #region UI CREATOR

        /// <summary>
        /// Show elements on screen
        /// </summary>
        private void showElements()
        {
            List<string> list = DB_Manager.elemTitles;
            for (int i = 0; i < list.Count; i++)
            {
                StackPanel s = setSP();
                for (int j = i; j < i + 3; j++)
                {
                    if (j < list.Count)
                        s.Children.Add(setTextBlock(list[j]));
                    else
                        break;
                }
                stackProtocols.Children.Add(s);
                i += 2;
            }
        }

        /// <summary>
        /// Show commands on screen
        /// </summary>
        private void showCommands()
        {
            List<string> commands = currentApp.commandsList();
            foreach (string txt in commands)
                stackCommands.Children.Add(setTextBlock(txt));
        }

        /// <summary>
        /// Create a custom Label
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private TextBlock setTextBlock(string text) => new TextBlock
        {
            Text = text
        };

        /// <summary>
        /// Create a custom StackPanel
        /// </summary>
        /// <returns></returns>
        private StackPanel setSP() => new StackPanel
        {
            Style = (Style)Application.Current.TryFindResource("spH")
        };
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
            currentApp.deleteWindow(Title);
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
