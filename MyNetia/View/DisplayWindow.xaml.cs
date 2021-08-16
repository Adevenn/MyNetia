using MyNetia.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MyNetia
{
    public partial class DisplayWindow : Window, INotifyPropertyChanged
    {
        private readonly App currentApp = (App)Application.Current;
        private Element _elem;
        public Element elem
        {
            get => _elem;
            set
            {
                if(_elem != value)
                {
                    _elem = value;
                    OnPropertyChanged();
                }
            }
        }

        public DisplayWindow(string title)
        {
            DataContext = this;
            InitializeComponent();
            Title = title;
            setValues(title);
        }

        #region EVENTS

        /// <summary>
        /// Set the element values and select the first chapter
        /// </summary>
        /// <param name="title"></param>
        private void setValues(string title)
        {
            elem = DB_Manager.getElement(title);
            listChapters.SelectedIndex = 0;
            elemLastUpdate.Text = "Last update : " + elem.lastUpdate.Month.ToString() + "/" + elem.lastUpdate.Day.ToString() + "/" + elem.lastUpdate.Year.ToString();
        }

        /// <summary>
        /// Set the chapters values on chapter selection changed and scroll to top
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listChapters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Chapter ch = (Chapter)listChapters.SelectedItem;
            setUI(ch);
            scrollViewer.ScrollToTop();
        }
        #endregion

        #region UI CREATOR

        /// <summary>
        /// Create text and image zones and set title
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="img"></param>
        /// <param name="sp"></param>
        private void setUI(Chapter ch)
        {
            gridContent.Children.Clear();
            gridContent.RowDefinitions.Clear();
            ObservableCollection<TextManager> texts = ch.texts;
            ObservableCollection<ImageManager> images = ch.images;
            TextBlock t;
            Image i;
            int idTxt = 0;
            int idImg = 0;
            while (true)
            {
                t = null;
                i = null;
                if (idTxt < texts.Count)
                {
                    if ((TypesTxt)texts[idTxt].type != TypesTxt.none)
                    {
                        t = setupText(texts, idTxt);
                        gridContent.Children.Add(t);
                    }
                    idTxt++;
                }
                if (idImg < images.Count)
                {
                    if ((TypesImage)images[idImg].type != TypesImage.none)
                    {
                        i = setupImage(images, idImg);
                        gridContent.Children.Add(i);
                    }
                    idImg++;
                }
                assignToRow(t, i);
                if (idTxt >= texts.Count && idImg >= images.Count)
                    break;
            }
        }

        /// <summary>
        /// Return a new TextBlock correctly setup
        /// </summary>
        /// <param name="texts"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private TextBlock setupText(ObservableCollection<TextManager> texts, int id)
        {
            return (TypesTxt)texts[id].type switch
            {
                TypesTxt.title => setTitle(texts[id].text),
                TypesTxt.subtitle => setSubtitle(texts[id].text),
                TypesTxt.subsubtitle => setSubsubtitle(texts[id].text),
                TypesTxt.text => setText(texts[id].text),
                _ => null,
            };
        }
        
        /// <summary>
        /// Return a new Image correctly setup
        /// </summary>
        /// <param name="images"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private Image setupImage(ObservableCollection<ImageManager> images, int id)
        {
            return (TypesImage)images[id].type switch
            {
                TypesImage.small => setSmallImage(images[id].datas),
                TypesImage.medium => setMediumImage(images[id].datas),
                TypesImage.big => setBigImage(images[id].datas),
                TypesImage.extraBig => setExtraBigImage(images[id].datas),
                _ => null,
            };
        }

        /// <summary>
        /// Assign text et image elements to the correct collum and row
        /// </summary>
        /// <param name="t"></param>
        /// <param name="i"></param>
        private void assignToRow(TextBlock t, Image i)
        {
            RowDefinition row = new RowDefinition
            {
                Height = GridLength.Auto
            };
            gridContent.RowDefinitions.Add(row);
            if (t != null || i != null)
            {
                if (t == null)
                {
                    Grid.SetColumnSpan(i, 4);
                    Grid.SetRow(i, gridContent.RowDefinitions.Count - 1);
                }
                else if (i == null)
                {
                    Grid.SetColumnSpan(t, 4);
                    Grid.SetRow(t, gridContent.RowDefinitions.Count - 1);
                }
                else
                {
                    Grid.SetColumn(t, 1);
                    Grid.SetRow(t, gridContent.RowDefinitions.Count - 1);
                    Grid.SetColumn(i, 2);
                    Grid.SetRow(i, gridContent.RowDefinitions.Count - 1);
                }
            }
            else
            {
                //Misconfig by the user ...
            }
        }

        /// <summary>
        /// Create a custom Title from TextBox
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private TextBlock setTitle(string text) => new TextBlock
        {
            Text = text,
            Style = (Style)Resources["tTxtTitle"]
        };

        /// <summary>
        /// Create a custom Subtitle from TextBox
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private TextBlock setSubtitle(string text) => new TextBlock
        {
            Text = text,
            Style = (Style)Resources["tTxtSubtitle"]
        };

        /// <summary>
        /// Create a custom Subsubtitle from TextBox
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private TextBlock setSubsubtitle(string text) => new TextBlock
        {
            Text = text,
            Style = (Style)Resources["tTxtSubsubtitle"]
        };

        /// <summary>
        /// Create a custom Text from TextBox
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private TextBlock setText(string text) => new TextBlock
        {
            Text = text,
            Style = (Style)Resources["tTxtText"]
        };

        /// <summary>
        /// Create a custom small image
        /// </summary>
        /// <param name="imageFile"></param>
        /// <returns></returns>
        private Image setSmallImage(byte[] imageFile) => new Image
        {
            Source = loadImage(imageFile),
            Style = (Style)Resources["smallImg"]
        };

        /// <summary>
        /// Create a custom medium image
        /// </summary>
        /// <param name="imageFile"></param>
        /// <returns></returns>
        private Image setMediumImage(byte[] imageFile) => new Image
        {
            Source = loadImage(imageFile),
            Style = (Style)Resources["mediumImg"]
        };

        /// <summary>
        /// Create a custom big image
        /// </summary>
        /// <param name="imageFile"></param>
        /// <returns></returns>
        private Image setBigImage(byte[] imageFile) => new Image
        {
            Source = loadImage(imageFile),
            Style = (Style)Resources["bigImg"]
        };

        /// <summary>
        /// Create a custom extra big image
        /// </summary>
        /// <param name="imageFile"></param>
        /// <returns></returns>
        private Image setExtraBigImage(byte[] imageFile) => new Image
        {
            Source = loadImage(imageFile),
            Style = (Style)Resources["extraBigImg"]
        };
        #endregion

        #region OTHERS METHODS

        /// <summary>
        /// Create WPF BitmapImage from byte[] values
        /// </summary>
        /// <param name="imageData"></param>
        /// <returns></returns>
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
