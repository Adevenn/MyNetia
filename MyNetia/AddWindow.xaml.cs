using MyNetia.Model;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyNetia
{
    public partial class AddWindow : Window
    {
        string infoContent = "- Text part :\n" +
                             "   LCtrl + Return => New Text Zone\n\n" +
                             "- Image part :\n" +
                             "   Enter => New Image Zone";
        public AddWindow()
        {
            InitializeComponent();
            setValues();
        }

        #region EVENTS
        private void validateAdd(object sender, RoutedEventArgs e)
        {
            List<string> theoryTxt = getTexts("theory");
            List<string> hackingTxt = getTexts("hacking");
            List<string> theoryImg = getImagesPath("theory");
            List<string> hackingImg = getImagesPath("hacking");
            AppResources.dbManager.add(name.Text, port.Text, theoryTxt, hackingTxt, theoryImg, hackingImg);
            DirectoryManager.createDirectory(Path.GetFullPath(@".\AppResources\Images\" + name.Text));
            Close();
            ResearchWindow.isAddWindowOpen = false;
        }
        private void stackTxtTheory_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.Enter))
            {
                LstackTxtTheory.Children.Add(setTxtLabel(""));
                stackTxtTheory.Children.Add(txtBoxMultiLines());
                Keyboard.Focus(stackTxtTheory.Children[stackTxtTheory.Children.Count - 1]);
            }
        }
        private void stackTxtHacking_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.Enter))
            {
                LstackTxtHacking.Children.Add(setTxtLabel(""));
                stackTxtHacking.Children.Add(txtBoxMultiLines());
                Keyboard.Focus(stackTxtHacking.Children[stackTxtHacking.Children.Count - 1]);
            }
        }
        private void imgTheory_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                LstackImgTheory.Children.Add(label(""));
                stackImgTheory.Children.Add(txtBoxSingleLine());
                Keyboard.Focus(stackImgTheory.Children[stackImgTheory.Children.Count - 1]);
            }
        }
        private void imgHacking_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                LstackImgHacking.Children.Add(label(""));
                stackImgHacking.Children.Add(txtBoxSingleLine());
                Keyboard.Focus(stackImgHacking.Children[stackImgHacking.Children.Count - 1]);
            }
        }
        #endregion

        #region UI CREATOR
        private Label label(string text) => new Label
        {
            Content = text,
            Style = (Style)Resources["label"]
        };
        private Label setTxtLabel(string text) => new Label
        {
            Content = text,
            Style = (Style)Resources["labelTxt"]
        };
        private TextBox txtBoxSingleLine() => new TextBox
        {
            Style = (Style)Resources["txtBox"]
        };
        private TextBox txtBoxMultiLines() => new TextBox
        {
            Style = (Style)Resources["txtBoxTxt"]
        };
        private List<string> getImagesPath(string type)
        {
            List<string> images = new List<string>();
            if (type == "theory")
            {
                foreach (TextBox t in stackImgTheory.Children)
                    images.Add(t.Text);
            }
            else if (type == "hacking")
            {
                foreach (TextBox t in stackImgHacking.Children)
                    images.Add(t.Text);
            }
            return images;
        }
        private List<string> getTexts(string type)
        {
            List<string> texts = new List<string>();
            if (type == "theory")
            {
                foreach (TextBox t in stackTxtTheory.Children)
                    texts.Add(t.Text);
            }
            else if (type == "hacking")
            {
                foreach (TextBox t in stackTxtHacking.Children)
                    texts.Add(t.Text);
            }
            return texts;
        }
        #endregion

        #region OTHER Methods
        private void setValues()
        {
            LstackTxtTheory.Children.Add(setTxtLabel("Text Theory :"));
            stackTxtTheory.Children.Add(txtBoxMultiLines());
            LstackTxtHacking.Children.Add(setTxtLabel("Text Hacking :"));
            stackTxtHacking.Children.Add(txtBoxMultiLines());
            LstackImgTheory.Children.Add(label("Image Theory :"));
            stackImgTheory.Children.Add(txtBoxSingleLine());
            LstackImgHacking.Children.Add(label("Image Hacking :"));
            stackImgHacking.Children.Add(txtBoxSingleLine());
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
            ResearchWindow.isAddWindowOpen = false;
        }
        private void titleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                    AdjustWindowSize();
                else if (WindowState == WindowState.Maximized)
                    WindowState = WindowState.Normal;
                else
                    DragMove();
            }
        }
        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            InfoWindow infosWindow= new InfoWindow(infoContent);
            infosWindow.Owner = this;
            infosWindow.ShowDialog();
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
