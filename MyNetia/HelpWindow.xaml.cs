using MyNetia.Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyNetia
{
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
            loadCommands();
            loadProtocols();
        }

        #region UI CREATOR
        private void loadProtocols()
        {
            for (int i = 0; i < AppResources.dbManager.db.Count; i++)
            {
                StackPanel s = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };
                for (int j = i; j < i + 4; j++)
                {
                    if (j < AppResources.dbManager.db.Count)
                        s.Children.Add(setLabel(AppResources.dbManager.db[j].title));
                    else
                        break;
                }
                stackProtocols.Children.Add(s);
                i += 3;
            }
        }
        private void loadCommands()
        {
            List<string> commands = AppResources.commandsList();
            StackPanel sp = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Style = (Style)Application.Current.TryFindResource("sp")
            };
            foreach (string txt in commands)
                stackCommands.Children.Add(setLabel(txt));
        }
        private Label setLabel(string content) => new Label
        {
            Content = content,
            Style = (Style)Resources["label"]
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
            ResearchWindow.isHelpWindowOpen = false;
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
