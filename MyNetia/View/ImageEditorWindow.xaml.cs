using Microsoft.Win32;
using MyNetia.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MyNetia.View
{
    public partial class ImageEditorWindow : Window, INotifyPropertyChanged
    {
        private string _path;
        public string path
        {
            get => _path;
            set
            {
                if (_path != value)
                {
                    _path = value;
                    OnPropertyChanged();
                }
            }
        }

        public ImageEditorWindow(string imgName, byte[] imgData)
        {
            DataContext = this;
            InitializeComponent();
            path = imgName;
        }

        /// <summary>
        /// Open an OpenFileDialog to select an image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reasearch_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png",
                InitialDirectory = DirectoryManager.DESKTOP,
                Title = "Please select an image"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                path = openFileDialog.FileName;
                DialogResult = true;
            }
        }

        /// <summary>
        /// Return an empty path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void null_Click(object sender, RoutedEventArgs e)
        {
            path = "";
            DialogResult = true;
        }

        /// <summary>
        /// Send a delete message to the admin window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void delete_Click(object sender, RoutedEventArgs e)
        {
            path = "delete";
            DialogResult = true;
        }

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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
