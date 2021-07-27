using System.Windows;
using System.Windows.Input;

namespace MyNetia
{
    public partial class ConfirmationWindow : Window
    {
        public ConfirmationWindow(string text)
        {
            InitializeComponent();
            label.Content = text;
        }

        #region EVENTS
        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region TITLE BAR

        #region EVENTS
        private void minBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void titleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
        #endregion

        #endregion

    }
}
