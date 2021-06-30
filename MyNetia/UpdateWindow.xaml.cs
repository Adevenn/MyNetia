using MyNetia.Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace MyNetia
{
    public partial class UpdateWindow : Window
    {
        private DB_Element element = null;
        private int elementID;
        string infoContent = "- Text part :\n" +
                             "   LCtrl + Return => New Text Zone\n\n" +
                             "- Image part :\n" +
                             "   Enter => New Image Zone";
        public UpdateWindow()
        {
            InitializeComponent();
        }

        #region EVENTS
        private void validateUpdate(object sender, RoutedEventArgs e)
        {
            getValues();
            AppResources.dbManager.update(elementID, element);
            imageValidation.Visibility = Visibility.Visible;
            animValidationOpacity();
        }
        private void spTxtTheory_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.Enter))
            {
                spLabelTxtTheory.Children.Add(labelTxt(""));
                spTxtTheory.Children.Add(txtBoxMultiLines(""));
                Keyboard.Focus(spTxtTheory.Children[spTxtTheory.Children.Count - 1]);
            }
        }
        private void spTxtHacking_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.Enter))
            {
                spLabelTxtHacking.Children.Add(labelTxt(""));
                spTxtHacking.Children.Add(txtBoxMultiLines(""));
                Keyboard.Focus(spTxtHacking.Children[spTxtHacking.Children.Count - 1]);
            }
        }
        private void imgTheory_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                spLabelImgTheory.Children.Add(label(""));
                spImgTheory.Children.Add(txtBoxSingleLine(""));
                Keyboard.Focus(spImgTheory.Children[spImgTheory.Children.Count - 1]);
            }
        }
        private void imgHacking_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                spLabelImgHacking.Children.Add(label(""));
                spImgHacking.Children.Add(txtBoxSingleLine(""));
                Keyboard.Focus(spImgHacking.Children[spImgHacking.Children.Count - 1]);
            }
        }
        private void selectionTxtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && AppResources.dbManager.isExist(selectionTxtBox.Text))
            {
                elementID = AppResources.dbManager.getId(selectionTxtBox.Text);
                element = AppResources.dbManager.getElement(selectionTxtBox.Text);
                spContent.Visibility = Visibility.Visible;
                updateValues();
            }
        }
        #endregion

        #region UI CREATOR
        private Label label(string text) => new Label
        {
            Content = text,
            Style = (Style)Resources["label"]
        };
        private Label labelTxt(string text) => new Label
        {
            Content = text,
            Style = (Style)Resources["labelTxt"]
        };
        private TextBox txtBoxSingleLine(string text) => new TextBox
        {
            Text = text,
            Style = (Style)Resources["txtBox"]
        };
        private TextBox txtBoxMultiLines(string text) => new TextBox
        {
            Text = text,
            Style = (Style)Resources["txtBoxTxt"]
        };
        private List<string> getImagesPath(string type)
        {
            List<string> images = new List<string>();
            if (type == "theory")
            {
                foreach (TextBox t in spImgTheory.Children)
                    images.Add(t.Text);
            }
            else if (type == "hacking")
            {
                foreach (TextBox t in spImgHacking.Children)
                    images.Add(t.Text);
            }
            return images;
        }
        private List<string> getTexts(string type)
        {
            List<string> texts = new List<string>();
            if (type == "theory")
            {
                foreach (TextBox t in spTxtTheory.Children)
                    texts.Add(t.Text);
            }
            else if (type == "hacking")
            {
                foreach (TextBox t in spTxtHacking.Children)
                    texts.Add(t.Text);
            }
            return texts;
        }
        #endregion

        #region OTHER Methods
        private void getValues()
        {
            element.name = name.Text;
            element.port = port.Text;
            element.theoryTxt = getTexts("theory");
            element.hackingTxt = getTexts("hacking");
            element.theoryImg = getImagesPath("theory");
            element.hackingImg = getImagesPath("hacking");
        }
        private void updateValues()
        {
            name.Text = element.name;
            port.Text = element.port;
            updateListValues("Text Theory :", element.theoryTxt, spLabelTxtTheory, spTxtTheory, "text");
            updateListValues("Text Hacking :", element.hackingTxt, spLabelTxtHacking, spTxtHacking, "text");
            updateListValues("Image Theory :", element.theoryImg, spLabelImgTheory, spImgTheory, "image");
            updateListValues("Image Hacking :", element.hackingImg, spLabelImgHacking, spImgHacking, "image");
        }
        private void updateListValues(string labelContent, List<string> list, StackPanel spLabels, StackPanel spTxtBox, string type)
        {
            spLabels.Children.Clear();
            spTxtBox.Children.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                if (type == "image")
                {
                    spTxtBox.Children.Add(txtBoxSingleLine(list[i]));
                    if (i == 0)
                        spLabels.Children.Add(label(labelContent));
                    else
                        spLabels.Children.Add(label(""));
                }
                else if (type == "text")
                {
                    spTxtBox.Children.Add(txtBoxMultiLines(list[i]));
                    if (i == 0)
                        spLabels.Children.Add(labelTxt(labelContent));
                    else
                        spLabels.Children.Add(labelTxt(""));
                }
            }
        }
        private void animValidationOpacity()
        {
            DoubleAnimation da = new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(3))
            };
            imageValidation.BeginAnimation(OpacityProperty, da);
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
            ResearchWindow.isUpdateWindowOpen = false;
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
        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            InfoWindow infosWindow = new InfoWindow(infoContent);
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
