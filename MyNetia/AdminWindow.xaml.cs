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
using System.Windows.Media;
using System.Collections.Generic;

namespace MyNetia
{
    public partial class AdminWindow : Window
    {
        private Point dragOriginPoint;
        private bool isDraging = false;
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
        private readonly string infoContent =
            "Select element :\n" +
            "   - Return => add/update the element selected\n\n" +
            "Chapter text :\n" +
            "   - LCtrl + Return => New paragraph\n" +
            "   - LCtrl + Back => Delete paragraph\n\n" +
            "Chapter image :\n" +
            "   - Enter => New image zone\n" +
            "   - LCtrl + Back => Delete image zone\n" +
            "   - Enter the name of your image\n" +
            "       e.g.: Image1.png and save your file inside\n" +
            "       MyNetia/AppRessources/Images/ElemTitle";
        List<string> matchingResearch = null;

        public AdminWindow()
        {
            DataContext = binding;
            isElemSelected = false;
            InitializeComponent();
        }


        #region EVENTS ADD/UPDATE
        private void selectAddUpdate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && AppResources.dbManager.isElementExist(selectAddUpdate.Text))
            {
                //Select the selected element
                setElement(selectAddUpdate.Text);
                listChapters.SelectedIndex = 0;
                isElemSelected = true;
            }
            else if (e.Key == Key.Return)
            {
                //Confirm element's creation
                ConfirmationWindow window = new ConfirmationWindow("Do you want to add " + selectAddUpdate.Text + " ?")
                {
                    Owner = this
                };
                if (window.ShowDialog() == true)
                {
                    //Add new Element
                    AppResources.dbManager.addElement(selectAddUpdate.Text);
                    setElement(selectAddUpdate.Text);
                    listChapters.SelectedIndex = 0;
                    isElemSelected = true;
                }
                else
                    isElemSelected = false;
            }
        }

        private void chapterContent_KeyDown(object sender, KeyEventArgs e)
        {
            //Delete the current text
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.Back))
            {
                
            }
        }

        private void listChapters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            setChapterValues((Chapter)listChapters.SelectedItem);
        }

        private void addChapter_Click(object sender, RoutedEventArgs e)
        {
            //Add and select the new chapter
            binding.chapters.Add(new Chapter());
            listChapters.SelectedIndex = listChapters.Items.Count - 1;
        }

        private void chapTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Update the list with the new chapter title
            int id = listChapters.SelectedIndex;
            binding.chapters[id].chapTitle = chapTitleTxtBox.Text;
        }

        private void txt_KeyDown(object sender, KeyEventArgs e)
        {
            //Create a new text zone
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.Enter))
            {
                spTxtBoxTxt.Children.Add(tBoxMultiLines(""));
                Keyboard.Focus(spTxtBoxTxt.Children[^1]);
            }
        }

        private void img_KeyDown(object sender, KeyEventArgs e)
        {
            //Create a new image zone
            if (e.Key == Key.Return)
            {
                spTxtBoxImg.Children.Add(tBoxSingleLine(""));
                Keyboard.Focus(spTxtBoxImg.Children[^1]);
            }
        }

        private void chapContent_LostFocus(object sender, RoutedEventArgs e)
        {
            //Save chapter's content
            int id = listChapters.SelectedIndex;
            binding.chapters[id].texts = getSPContent(spTxtBoxTxt);
            binding.chapters[id].images = getSPContent(spTxtBoxImg);
        }

        private void validate(object sender, RoutedEventArgs e)
        {
            //Update selected element
            AppResources.dbManager.updateElement(binding.oldElemTitle ,binding.elemTitle, binding.elemSubtitle, new List<Chapter>(binding.chapters));
            DirectoryManager.createDirectory(Path.GetFullPath(@".\AppResources\Images\" + binding.elemTitle));
            imageValid.Visibility = Visibility.Visible;
            animImageOpacity(imageValid);
        }

        private void setElement(string title)
        {
            DB_Element elem = AppResources.dbManager.getElement(title);
            binding.oldElemTitle = elem.title;
            binding.elemTitle = elem.title;
            binding.elemSubtitle = elem.subtitle;
            binding.chapters = new ObservableCollection<Chapter>(elem.chapters);
        }
        private void setChapterValues(Chapter ch)
        {
            if (!isDraging)
            {
                binding.chapTitle = ch.chapTitle;
                setSPTxtContent(spTxtBoxTxt, ch.texts);
                setSPImgContent(spTxtBoxImg, ch.images);
            }
        }

        private List<string> getSPContent(StackPanel sp)
        {
            List<string> texts = new List<string>();
            foreach (TextBox t in sp.Children)
                texts.Add(t.Text);
            return texts;
        }

        private void setSPImgContent(StackPanel sp, List<string> list)
        {
            sp.Children.Clear();
            foreach (string text in list)
                sp.Children.Add(tBoxSingleLine(text));
        }

        private void setSPTxtContent(StackPanel sp, List<string> list)
        {
            sp.Children.Clear();
            foreach (string text in list)
                sp.Children.Add(tBoxMultiLines(text));
        }

        # region Drag and drop ItemListBox
        private void listChaptersItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragOriginPoint = e.GetPosition(null);
        }

        private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null)
                return null;
            if (parentObject is T parent)
                return parent;
            return FindVisualParent<T>(parentObject);
        }

        private void listChapters_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(null);
            Vector diff = dragOriginPoint - point;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                ListBoxItem lbi = FindVisualParent<ListBoxItem>((DependencyObject)e.OriginalSource);
                if (lbi != null)
                    DragDrop.DoDragDrop(lbi, lbi.DataContext, DragDropEffects.Move);
            }
        }

        private void listChaptersItem_Drop(object sender, DragEventArgs e)
        {
            if (sender is ListBoxItem)
            {
                isDraging = true;
                Chapter source = e.Data.GetData(typeof(Chapter)) as Chapter;
                Chapter target = ((ListBoxItem)sender).DataContext as Chapter;
                int sourceIndex = listChapters.Items.IndexOf(source);
                int targetIndex = listChapters.Items.IndexOf(target);

                moveChapter(source, sourceIndex, targetIndex);
                isDraging = false;
            }
        }

        private void moveChapter(Chapter source, int sourceId, int targetId)
        {
            if (sourceId < targetId)
            {
                binding.chapters.Insert(targetId + 1, source);
                binding.chapters.RemoveAt(sourceId);
            }
            else
            {
                int removeIndex = sourceId + 1;
                if (binding.chapters.Count + 1 > removeIndex)
                {
                    binding.chapters.Insert(targetId, source);
                    binding.chapters.RemoveAt(removeIndex);
                }
            }
        }
        #endregion

        #endregion


        #region EVENTS DELETE
        private void selectionDelete_GotFocus(object sender, RoutedEventArgs e)
        {
            helpResearchBar();
        }

        private void selectionDelete_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && AppResources.dbManager.isElementExist(selectDelete.Text))
            {
                //Confirmation dialog to confirm element's creation
                ConfirmationWindow window = new ConfirmationWindow("Do you want to delete " + selectDelete.Text + " ?")
                {
                    Owner = this
                };
                if (window.ShowDialog() == true)
                {
                    //Delete element
                    DirectoryManager.deleteDirectory(Path.GetFullPath(@".\AppResources\Images\" + selectDelete.Text));
                    AppResources.dbManager.deleteElement(selectDelete.Text);
                    helpResearchBar();
                }
            }
            else if (e.Key == Key.Tab && matchingResearch.Count > 0)
            {
                selectDelete.Text = matchingResearch[0];
                //Set Keyboard focus at the end
                selectDelete.CaretIndex = selectDelete.Text.Length;
            }
        }

        private void onTextChangedDelete(object sender, TextChangedEventArgs e)
        {
            helpResearchBar();
        }

        private void helpResearchBar()
        {
            matchingResearch = new List<string>();
            tBlockDelete.Text = null;
            foreach (string txt in AppResources.dbManager.getTitles())
            {
                if (txt.Contains(selectDelete.Text))
                {
                    tBlockDelete.Text += txt + "\n";
                    matchingResearch.Add(txt);
                }
            }
        }
        #endregion


        #region UI
        private TextBox tBoxSingleLine(string content) => new TextBox
        {
            Style = (Style)Resources["tBoxSingleLine"],
            Text = content
        };

        private TextBox tBoxMultiLines(string content) => new TextBox
        {
            Style = (Style)Resources["tBoxMultiLines"],
            Text = content
        };
        #endregion


        #region OTHER METHODS
        private void animImageOpacity(Image image)
        {
            //Change the image opacity from 1 to 0 in 3 seconds
            DoubleAnimation da = new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(3))
            };
            image.BeginAnimation(OpacityProperty, da);
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
