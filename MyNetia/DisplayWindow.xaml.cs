using MyNetia.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MyNetia
{
    public partial class DisplayWindow : Window
    {
        private App currentApp = (App)Application.Current;
        private readonly InfoBinding binding = new InfoBinding();
        public DisplayWindow(string title)
        {
            DataContext = binding;
            InitializeComponent();
            Title = "MyNetia-" + title;
            setValues(title);
        }

        #region EVENTS
        private void listChapters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            setChapterValues((Chapter)listChapters.SelectedItem);
        }

        private void setValues(string title)
        {
            DB_Element elem = AppResources.dbManager.getElement(title);
            elemTitle.Text = elem.title;
            elemSubtitle.Text = elem.subtitle;
            binding.chapters = new ObservableCollection<Chapter>(elem.chapters);
            listChapters.SelectedIndex = 0;
            elemLastUpdate.Text = "Last update : " + elem.lastUpdate.Month.ToString() + "/" + elem.lastUpdate.Day.ToString() + "/" + elem.lastUpdate.Year.ToString();
        }

        private void setChapterValues(Chapter ch)
        {
            scrollViewer.ScrollToTop();
            binding.chapTitle = ch.chapTitle;
            setUI(ch.texts, ch.images, spContent);
        }
        #endregion

        #region UI CREATOR
        private void setUI(List<string> txt, List<string> img, StackPanel sp)
        {
            sp.Children.Clear();
            int idTxt = 0;
            int idImg = 0;
            while (true)
            {
                StackPanel spHoriz = new StackPanel
                {
                    Style = (Style)Resources["spHCustom"]
                };
                if (idTxt < txt.Count)
                {
                    if (!string.IsNullOrWhiteSpace(txt[idTxt]))
                        spHoriz.Children.Add(setTxtBlock(txt[idTxt]));
                    idTxt++;
                }
                if (idImg < img.Count)
                {
                    if (!string.IsNullOrWhiteSpace(img[idImg]))
                    {
                        string path = elemTitle.Text + @"\" + img[idImg];
                        if (FileManager.isImageExist(path))
                            spHoriz.Children.Add(image(FileManager.readByteFile(path)));
                        else
                            spHoriz.Children.Add(image(FileManager.defaultImage()));
                    }
                    idImg++;
                }
                sp.Children.Add(spHoriz);
                if (idTxt >= txt.Count && idImg >= img.Count)
                    break;
            }
        }

        private TextBlock setTxtBlock(string text) => new TextBlock
        {
            Text = text,
            Style = (Style)Resources["txtBlock"]
        };

        private Image image(byte[] imageFile) => new Image
        {
            Source = loadImage(imageFile),
            Style = (Style)Resources["image"]
        };
        #endregion

        #region OTHERS METHODS
        private BitmapImage loadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return null;
            BitmapImage image = new BitmapImage();
            using (System.IO.MemoryStream mem = new System.IO.MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
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

        private void Window_Closing(object sender, CancelEventArgs e)
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

        private class InfoBinding : INotifyPropertyChanged
        {
            private string _chapTitle;
            public string chapTitle
            {
                get { return _chapTitle; }
                set
                {
                    if (_chapTitle != value)
                    {
                        _chapTitle = value;
                        OnPropertyChanged();
                    }
                }
            }

            private ObservableCollection<Chapter> _chapters;
            public ObservableCollection<Chapter> chapters
            {
                get { return _chapters; }
                set
                {
                    if (_chapters != value)
                    {
                        _chapters = value;
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
