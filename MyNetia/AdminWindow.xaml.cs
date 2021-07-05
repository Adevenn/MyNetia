using MyNetia.Model;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Animation;
using System;

namespace MyNetia
{
    public partial class AdminWindow : Window
    {
        private readonly InfoBinding binding = new InfoBinding();
        private bool _isElemSelected;
        private bool isElemSelected
        {
            get { return _isElemSelected; }
            set
            {
                if (value == true && value != _isElemSelected)
                {
                    spElemPart.Visibility = Visibility.Visible;
                    chaptersListPart.Visibility = Visibility.Visible;
                    chapContentPart.Visibility = Visibility.Visible;
                    _isElemSelected = value;
                }
                else if (value != _isElemSelected)
                {
                    spElemPart.Visibility = Visibility.Hidden;
                    chaptersListPart.Visibility = Visibility.Hidden;
                    chapContentPart.Visibility = Visibility.Hidden;
                    _isElemSelected = value;
                }
            }
        }
        private readonly string infoContent = "- Select element :\n" +
                                              "   Return => add or update the\n" +
                                              "             element selected\n\n" +
                                              "- Text part :\n" +
                                              "   LCtrl + Return => New paragraph\n\n" +
                                              "- Image part :\n" +
                                              "   Enter => New Image Zone";
        ObservableCollection<string> matchingResearch = null;

        public AdminWindow()
        {
            DataContext = binding;
            isElemSelected = false;
            InitializeComponent();
        }


        #region EVENTS ADD/UPDATE
        private void selectionTxtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && AppResources.dbManager.isElementExist(selectionTxtBox.Text))
            {
                setElement(selectionTxtBox.Text);
                listChapters.SelectedIndex = 0;
                isElemSelected = true;
            }
            else if (e.Key == Key.Return)
            {
                //Confirmation dialog to confirm element's creation
                ConfirmationWindow window = new ConfirmationWindow("Do you want to add " + selectionTxtBox.Text + " ?")
                {
                    Owner = this
                };
                if (window.ShowDialog() == true)
                {
                    //Add new Element
                    AppResources.dbManager.addElement(selectionTxtBox.Text);
                    setElement(selectionTxtBox.Text);
                    listChapters.SelectedIndex = 0;
                    isElemSelected = true;
                }
                else
                    isElemSelected = false;
            }
        }

        private void listBoxChapters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            setChapterValues((Chapter)listChapters.SelectedItem);
        }

        private void addChapter_Click(object sender, RoutedEventArgs e)
        {
            binding.chapters.Add(new Chapter());
            //Select the new chapter
        }

        private void chapTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            int id = listChapters.SelectedIndex;
            binding.chapters[id].chapTitle = chapTitleTxtBox.Text;
        }

        private void txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.Enter))
            {
                spTxtBoxTxt.Children.Add(txtBoxMultiLines(""));
                Keyboard.Focus(spTxtBoxTxt.Children[^1]);
            }
        }

        private void img_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                spTxtBoxImg.Children.Add(txtBoxSingleLine(""));
                Keyboard.Focus(spTxtBoxImg.Children[^1]);
            }
        }

        private void txtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //Save chapter's content
            binding.chapters[listChapters.SelectedIndex].texts = getSPContent(spTxtBoxTxt);
            binding.chapters[listChapters.SelectedIndex].images = getSPContent(spTxtBoxImg);
        }

        private void validate(object sender, RoutedEventArgs e)
        {
            //Update selected element
            AppResources.dbManager.updateElement(binding.oldElemTitle ,binding.elemTitle, binding.elemSubtitle, binding.chapters);
            DirectoryManager.createDirectory(Path.GetFullPath(@".\AppResources\Images\" + binding.elemTitle));
            imageValidation.Visibility = Visibility.Visible;
            animValidationOpacity();
        }

        private void setElement(string title)
        {
            DB_Element elem = AppResources.dbManager.getElement(title);
            binding.oldElemTitle = elem.title;
            binding.elemTitle = elem.title;
            binding.elemSubtitle = elem.subtitle;
            binding.chapters = elem.chapters;
        }
        private void setChapterValues(Chapter ch)
        {
            binding.chapTitle = ch.chapTitle;
            setSPTxtContent(spTxtBoxTxt, ch.texts);
            setSPImgContent(spTxtBoxImg, ch.images);
        }

        private ObservableCollection<string> getSPContent(StackPanel sp)
        {
            ObservableCollection<string> texts = new ObservableCollection<string>();
            foreach (TextBox t in sp.Children)
                texts.Add(t.Text);
            return texts;
        }

        private void setSPImgContent(StackPanel sp, ObservableCollection<string> list)
        {
            sp.Children.Clear();
            foreach (string text in list)
                sp.Children.Add(txtBoxSingleLine(text));
        }

        private void setSPTxtContent(StackPanel sp, ObservableCollection<string> list)
        {
            sp.Children.Clear();
            foreach (string text in list)
                sp.Children.Add(txtBoxMultiLines(text));
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


        #region EVENTS DELETE
        private void selectionDelete_GotFocus(object sender, RoutedEventArgs e)
        {
            helpResearchBar();
        }

        private void selectionDelete_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && AppResources.dbManager.isElementExist(selectionDelete.Text))
            {
                //Confirmation dialog to confirm element's creation
                ConfirmationWindow window = new ConfirmationWindow("Do you want to delete " + selectionDelete.Text + " ?")
                {
                    Owner = this
                };
                if (window.ShowDialog() == true)
                {
                    //Delete element
                    DirectoryManager.deleteDirectory(Path.GetFullPath(@".\AppResources\Images\" + selectionDelete.Text));
                    AppResources.dbManager.deleteElement(selectionDelete.Text);
                    helpResearchBar();
                }
            }
            else if (e.Key == Key.Tab && matchingResearch.Count > 0)
            {
                selectionTxtBox.Text = matchingResearch[0];
                //Set Keyboard focus at the end
                selectionTxtBox.CaretIndex = selectionTxtBox.Text.Length;
            }
        }

        private void onTextChangedDelete(object sender, TextChangedEventArgs e)
        {
            helpResearchBar();
        }

        private void helpResearchBar()
        {
            matchingResearch = new ObservableCollection<string>();
            txtBlockDelete.Text = null;
            foreach (string txt in AppResources.dbManager.getTitles())
            {
                if (txt.Contains(selectionDelete.Text))
                {
                    txtBlockDelete.Text += txt + "\n";
                    matchingResearch.Add(txt);
                }
            }
        }
        #endregion


        #region UI
        private TextBox txtBoxSingleLine(string content) => new TextBox
        {
            Style = (Style)Resources["txtBox"],
            Text = content
        };

        private TextBox txtBoxMultiLines(string content) => new TextBox
        {
            Style = (Style)Resources["txtBoxTxt"],
            Text = content
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
            ResearchWindow.isAdminWindowOpen = false;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ResearchWindow.isAdminWindowOpen = false;
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
            InfoWindow infosWindow = new InfoWindow(infoContent)
            {
                Owner = this
            };
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


        private class InfoBinding : INotifyPropertyChanged
        {
            public string oldElemTitle;
            private string _elemTitle;
            public string elemTitle
            {
                get { return _elemTitle; }
                set
                {
                    if(_elemTitle != value)
                    {
                        _elemTitle = value;
                        OnPropertyChanged();
                    }
                }
            }

            private string _elemSubtitle;
            public string elemSubtitle
            {
                get { return _elemSubtitle; }
                set
                {
                    if (_elemSubtitle != value)
                    {
                        _elemSubtitle = value;
                        OnPropertyChanged();
                    }
                }
            }

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
                    if(_chapters != value)
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
