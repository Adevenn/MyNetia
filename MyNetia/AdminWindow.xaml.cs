using MyNetia.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MyNetia
{
    public partial class AdminWindow : Window
    {
        private readonly App currentApp = (App)Application.Current;
        private Point dragOriginPoint;
        private bool isDraging = false;
        private readonly InfoBinding binding = new InfoBinding();
        private List<string> matchingResearch = null;
        private bool isElemSelected;
        private void setIsElemSelected(bool value)
        {
            if (value && value != isElemSelected)
            {
                spElemPart.Visibility = Visibility.Visible;
                chaptersListPart.Visibility = Visibility.Visible;
                chapContentPart.Visibility = Visibility.Visible;
                isElemSelected = value;
            }
            else if (value != isElemSelected)
            {
                spElemPart.Visibility = Visibility.Hidden;
                chaptersListPart.Visibility = Visibility.Hidden;
                chapContentPart.Visibility = Visibility.Hidden;
                isElemSelected = value;
            }
        }
        private readonly string infoContent =
            "Select element :\n" +
            "   - Return => add/update the element selected\n\n" +
            "Chapter text :\n" +
            "   - LCtrl + Return => New paragraph\n" +
            "   - LAlt + Return => Delete paragraph\n\n" +
            "Chapter image :\n" +
            "   - Enter => New image zone\n" +
            "   - LAlt + Return => Delete image zone\n" +
            "   - Enter the name of your image\n" +
            "       e.g.: Image1.png and save your file inside\n" +
            "       MyNetia/AppRessources/Images/ElemTitle";

        public AdminWindow()
        {
            DataContext = binding;
            setIsElemSelected(false);
            InitializeComponent();
        }


        #region EVENTS ADD/UPDATE

        #region Elem Selection
        private void selectAddUpdate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && AppResources.dbManager.isElementExist(selectAddUpdate.Text))
            {
                //Apply element selection
                setElement(selectAddUpdate.Text);
                listChapters.SelectedIndex = 0;
                setIsElemSelected(true);
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
                    setIsElemSelected(true);
                }
                else
                    setIsElemSelected(false);
            }
        }

        private void setElement(string title)
        {
            DB_Element elem = AppResources.dbManager.getElement(title);
            binding.oldElemTitle = elem.title;
            binding.elemTitle = elem.title;
            binding.elemSubtitle = elem.subtitle;
            binding.chapters = new ObservableCollection<Chapter>(elem.chapters);
        }
        #endregion

        #region ChaptersList Setup
        private void listChapters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Select by default the first item
            if (listChapters.SelectedIndex == -1)
                listChapters.SelectedIndex = 0;
            //Set by default 1 empty chapter
            if (binding.chapters.Count == 0)
                binding.chapters.Add(new Chapter());
            setChapterValues((Chapter)listChapters.SelectedItem);
        }

        private void setChapterValues(Chapter ch)
        {
            if (!isDraging)
            {
                binding.chapTitle = ch.chapTitle;
                binding.setTxtList(ch.texts);
                binding.setImgList(ch.images);
            }
        } 

        private void addChapter_Click(object sender, RoutedEventArgs e)
        {
            //Add and select the new chapter
            binding.chapters.Add(new Chapter());
            listChapters.SelectedIndex = listChapters.Items.Count - 1;
        }
        #endregion

        #region ChaptersListItems Setup
        private void chapTitle_KeyDown(object sender, KeyEventArgs e)
        {
            //Delete the current chapter
            if (Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.Return))
            {
                ConfirmationWindow window = new ConfirmationWindow("Do you realy want to delete\n" + chapTitle.Text + " ?")
                {
                    Owner = this
                };
                if (window.ShowDialog() == true)
                {
                    //Delete the chapter
                    int id = listChapters.SelectedIndex;
                    binding.chapters.RemoveAt(id);
                }
            }
        }

        private void chapTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Update chapList with the new chapTitle
            int id = listChapters.SelectedIndex;
            binding.chapters[id].chapTitle = chapTitle.Text;
        }

        private void listTxt_KeyDown(object sender, KeyEventArgs e)
        {
            //Create a new text zone
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.Enter))
            {
                binding.texts.Add(new InfoBinding.ItemContent(""));
                listTxt.SelectedIndex = listTxt.Items.Count - 1;
            }
            //Remove text zone
            if (Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.Enter) && listTxt.SelectedIndex != -1)
            {
                int id = listTxt.SelectedIndex;
                binding.texts.RemoveAt(id);
                if (binding.texts.Count == 0)
                {
                    binding.texts.Add(new InfoBinding.ItemContent(""));
                    listTxt.SelectedIndex = listTxt.Items.Count - 1;
                }
            }
        }

        private void listImg_KeyDown(object sender, KeyEventArgs e)
        {
            //Create a new image zone
            if (e.Key == Key.Return)
            {
                binding.images.Add(new InfoBinding.ItemContent(""));
                listImg.SelectedIndex = listImg.Items.Count - 1;
            }
            //Remove image zone
            if (Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.Enter) && listImg.SelectedIndex != -1)
            {
                int id = listImg.SelectedIndex;
                binding.images.RemoveAt(id);
                if (binding.images.Count == 0)
                {
                    binding.images.Add(new InfoBinding.ItemContent(""));
                    listImg.SelectedIndex = listImg.Items.Count - 1;
                }
            }
        }

        private void chapContent_LostFocus(object sender, RoutedEventArgs e)
        {
            //Save chapter's content
            int id = listChapters.SelectedIndex;
            binding.chapters[id].texts = new List<string>(binding.getTxtList());
            binding.chapters[id].images = new List<string>(binding.getImgList());
        }

        private void itemListText_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ListViewItem item = (ListViewItem)sender;
            item.IsSelected = true;
        }

        private void itemList_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)sender;
            item.IsSelected = true;
        }

        private void valid_Click(object sender, RoutedEventArgs e)
        {
            //Update selected element
            AppResources.dbManager.updateElement(binding.oldElemTitle, binding.elemTitle, binding.elemSubtitle, new List<Chapter>(binding.chapters));
            DirectoryManager.createDirectory(Path.GetFullPath(@".\AppResources\Images\" + binding.elemTitle));
            imageValid.Visibility = Visibility.Visible;
            animImageOpacity(imageValid);
        }
        #endregion

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
            if (sender is ListBoxItem item)
            {
                isDraging = true;
                Chapter source = e.Data.GetData(typeof(Chapter)) as Chapter;
                Chapter target = item.DataContext as Chapter;
                int sourceIndex = listChapters.Items.IndexOf(source);
                int targetIndex = listChapters.Items.IndexOf(target);

                moveChapter(source, sourceIndex, targetIndex);
                listChapters.SelectedIndex = targetIndex;
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
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            currentApp.deleteWindow(Title);
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
                get => _elemTitle;
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
                get => _elemSubtitle;
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
                get => _chapTitle;
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
                get => _chapters;
                set
                {
                    if(_chapters != value)
                    {
                        _chapters = value;
                        OnPropertyChanged();
                    }
                }
            }

            private ObservableCollection<ItemContent> _texts;
            public ObservableCollection<ItemContent> texts
            {
                get => _texts;
                set
                {
                    if (_texts != value)
                    {
                        _texts = value;
                        OnPropertyChanged();
                    }
                }
            }

            private ObservableCollection<ItemContent> _images;
            public ObservableCollection<ItemContent> images
            {
                get => _images;
                set
                {
                    if (_images != value)
                    {
                        _images = value;
                        OnPropertyChanged();
                    }
                }
            }

            public List<string> getTxtList()
            {
                List<string> stringList = new List<string>();
                foreach (ItemContent item in texts)
                    stringList.Add(item.content);
                return stringList;
            }

            public List<string> getImgList()
            {
                List<string> stringList = new List<string>();
                foreach (ItemContent item in images)
                    stringList.Add(item.content);
                return stringList;
            }

            public void setTxtList(List<string> stringList)
            {
                texts = new ObservableCollection<ItemContent>();
                foreach(string s in stringList)
                    texts.Add(new ItemContent(s));
            }

            public void setImgList(List<string> stringList)
            {
                images = new ObservableCollection<ItemContent>();
                foreach (string s in stringList)
                    images.Add(new ItemContent(s));
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName] string name = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }

            //Needed to correctly bind Text boxes inside ListBox (texts and images) with TwoWay mode
            public class ItemContent
            {
                public ItemContent(string content)
                {
                    this.content = content;
                }
                public string content { get; set; }
            }
        }
    }
}
