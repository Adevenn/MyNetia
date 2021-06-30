using MyNetia.Model;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MyNetia
{
    public partial class DisplayWindow : Window
    {
        private readonly DB_Element element;
        public DisplayWindow(string name)
        {
            element = AppResources.dbManager.getElement(name);
            InitializeComponent();
            setValues();
        }

        #region UI CREATOR
        private void setUI(List<string> txt, List<string> img, StackPanel sp)
        {
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
                        string path = Path.GetFullPath(@".\AppResources\Images\" + element.name + @"\" + img[idImg]);
                        if (File.Exists(path))
                            spHoriz.Children.Add(image(File.ReadAllBytes(path)));
                        else
                            spHoriz.Children.Add(image(AppResources.defaultImage()));
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
        private void setValues()
        {
            txtTitle.Text = element.name;
            if (!string.IsNullOrWhiteSpace(element.port))
                txtPort.Text = "PORT : " + element.port;
            setUI(element.theoryTxt, element.theoryImg, spContentTheory);
            setUI(element.hackingTxt, element.hackingImg, spContentHacking);
            txtLastUpdate.Text = "Last update : " + element.lastUpdate.Month.ToString() + "/" + element.lastUpdate.Day.ToString() + "/" + element.lastUpdate.Year.ToString();
        }
        private static BitmapImage loadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
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
