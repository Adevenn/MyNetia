using MyNetia.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MyNetia
{
    public partial class AdminWindow : Window, INotifyPropertyChanged
    {
        private readonly App currentApp = (App)Application.Current;
        private string oldElemTitle;
        private Element _currentElem;
        public Element currentElem
        {
            get => _currentElem;
            set
            {
                if(_currentElem != value)
                {
                    _currentElem = value;
                    OnPropertyChanged();
                }
            }
        }
        private int idSelectedChap = -1;
        private string _selectAddUpdate = "";
        public string selectAddUpdate
        {
            get => _selectAddUpdate;
            set
            {
                if (_selectAddUpdate != value)
                {
                    _selectAddUpdate = value;
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
        private ObservableCollection<string> _texts;
        public ObservableCollection<string> texts
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
        private ObservableCollection<byte[]> _images;
        public ObservableCollection<byte[]> images
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
        private string _selectionDel = "";
        public string selectionDel
        {
            get => _selectionDel;
            set
            {
                if (_selectionDel != value)
                {
                    _selectionDel = value;
                    OnPropertyChanged();
                }
            }
        }
        private ObservableCollection<string> _matchingResearch;
        public ObservableCollection<string> matchingResearch
        {
            get => _matchingResearch;
            set
            {
                if (value != _matchingResearch)
                {
                    _matchingResearch = value;
                    OnPropertyChanged();
                }
            }
        }
        private Point dragOriginPoint;
        private bool isDraging = false;
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
            "   - Can not containt this characters :\n" +
            "       < > : \" / \\ | ? *\n" +
            "       and can't be empty or white space\n" +
            "   - Return => Apply selection\n\n" +
            "Chapter text :\n" +
            "   - LCtrl + Return => New paragraph\n" +
            "   - LAlt + Return => Delete paragraph\n\n" +
            "Chapter image :\n" +
            "   - Enter => New image zone\n" +
            "   - LAlt + Return => Delete image zone";

        public AdminWindow()
        {
            this.DataContext = this;
            InitializeComponent();
            matchingResearchUpdate();
        }


        #region EVENTS ADD/UPDATE

        /// <summary>
        /// When Return is pressed and if the text value is valid, set element values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAddUpdate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (DirectoryManager.isValidName(selectAddUpdate))
                {
                    currentElem = DB_Manager.getElement(selectAddUpdate);
                    oldElemTitle = currentElem.title;
                    listChapters.SelectedIndex = 0;
                    setIsElemSelected(true);
                }
                else
                {
                    //TODO : Display something in the window : *invalid
                    setIsElemSelected(false);
                }
            }
        }

        /// <summary>
        /// Set the new chapter values on listChapters selection has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listChapters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isDraging)
            {
                //Set by default 1 empty chapter
                if (currentElem.chapters.Count == 0)
                    currentElem.chapters.Add(new Chapter("1"));
                //Select by default the first item if nothing is selected
                if (listChapters.SelectedIndex == -1)
                    listChapters.SelectedIndex = 0;
                setChapterValues((Chapter)listChapters.SelectedItem);
                idSelectedChap = listChapters.SelectedIndex;
            }
        }

        /// <summary>
        /// Set chapter values
        /// </summary>
        /// <param name="ch"></param>
        private void setChapterValues(Chapter ch)
        {
            chapTitle = ch.title;
            texts = new ObservableCollection<string>(ch.texts);
            images = new ObservableCollection<byte[]>(ch.images);
        }

        /// <summary>
        /// Add a new chapter on clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addChapter_Click(object sender, RoutedEventArgs e)
        {
            //Add and select the new chapter
            currentElem.chapters.Add(new Chapter((listChapters.Items.Count + 1).ToString()));
            listChapters.SelectedIndex = listChapters.Items.Count - 1;
        }
        
        /// <summary>
        /// Update item inside ListBox when title text has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chapTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            int id = listChapters.SelectedIndex;
            currentElem.chapters[id].title = chapTitle;
            currentElem.chapters[id].title = chapTitle;
        }

        /// <summary>
        /// Delete the current chapter when Return + LeftAlt are pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chapTitle_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.Return))
            {
                ConfirmationWindow window = new ConfirmationWindow("Do you realy want to delete\n" + chapTitle + " ?")
                {
                    Owner = this
                };
                if (window.ShowDialog() == true)
                {
                    int id = listChapters.SelectedIndex;
                    currentElem.chapters.RemoveAt(id);
                }
            }
        }

        /// <summary>
        /// Add a text zone when Enter + LCtrl are pressed, Remove the current text zone when Enter + LAlt are pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listTxt_KeyDown(object sender, KeyEventArgs e)
        {
            //Create a new text zone
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.Enter))
            {
                texts.Add("");
                listTxt.SelectedIndex = listTxt.Items.Count - 1;
            }
            //Remove text zone
            if (Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.Enter) && listTxt.SelectedIndex != -1)
            {
                int id = listTxt.SelectedIndex;
                texts.RemoveAt(id);
                if (texts.Count == 0)
                {
                    texts.Add("");
                    listTxt.SelectedIndex = listTxt.Items.Count - 1;
                }
            }
        }

        /// <summary>
        /// Add a image when Enter + LCtrl are pressed, Remove the current image when Enter + LAlt are pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listImg_KeyDown(object sender, KeyEventArgs e)
        {
            //Create a new image zone
            if (e.Key == Key.Return)
            {
                images.Add(new byte[0]);
                listImg.SelectedIndex = listImg.Items.Count - 1;
            }
            //Remove image zone
            if (Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.Enter) && listImg.SelectedIndex != -1)
            {
                int id = listImg.SelectedIndex;
                images.RemoveAt(id);
                if (images.Count == 0)
                {
                    images.Add(new byte[0]);
                    listImg.SelectedIndex = listImg.Items.Count - 1;
                }
            }
        }

        /// <summary>
        /// Select the item that has the keyboard focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemList_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)sender;
            item.IsSelected = true;
        }

        /// <summary>
        /// Save content when lost focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxItem_LostFocus(object sender, RoutedEventArgs e)
        {
            currentElem.chapters[idSelectedChap].texts = texts;
            currentElem.chapters[idSelectedChap].images = images;
        }

        /// <summary>
        /// Valid element content and save it in the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void valid_Click(object sender, RoutedEventArgs e)
        {
            if (DB_Manager.isElementExist(oldElemTitle))
                DB_Manager.updateElement(oldElemTitle, currentElem);
            else
                DB_Manager.addElement(currentElem);
            //Show Validation image
            imageValid.Visibility = Visibility.Visible;
            animImageOpacity(imageValid);
        }
        #endregion

        #region Drag and drop ItemListBox
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
                currentElem.chapters.Insert(targetId + 1, source);
                currentElem.chapters.RemoveAt(sourceId);
            }
            else
            {
                int removeIndex = sourceId + 1;
                if (currentElem.chapters.Count + 1 > removeIndex)
                {
                    currentElem.chapters.Insert(targetId, source);
                    currentElem.chapters.RemoveAt(removeIndex);
                }
            }
        }
        #endregion


        #region EVENTS DELETE

        /// <summary>
        /// Delete element selected when Enter is pressed, Auto-complete when Tab is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectionDelete_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && DB_Manager.isElementExist(selectionDel))
            {
                //Confirm element creation
                ConfirmationWindow window = new ConfirmationWindow("Do you want to delete " + selectionDel + " ?")
                {
                    Owner = this
                };
                if (window.ShowDialog() == true)
                {
                    //Delete element
                    DB_Manager.deleteElement(selectionDel);
                    DB_Manager.getTitles();
                    matchingResearchUpdate();
                }
            }
            else if (e.Key == Key.Tab && matchingResearch.Count > 0)
            {
                selectionDel = matchingResearch[0];
                //Set Keyboard focus at the end
                selectDelete.CaretIndex = selectionDel.Length;
            }
        }

        /// <summary>
        /// Update the matching list when text has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onTextChangedDelete(object sender, TextChangedEventArgs e)
        {
            matchingResearchUpdate();
        }

        /// <summary>
        /// Update the matching list
        /// </summary>
        private void matchingResearchUpdate()
        {
            matchingResearch = new ObservableCollection<string>(DB_Manager.matchingResearch(selectionDel));
        }
        #endregion


        #region OTHER METHODS

        /// <summary>
        /// Change the image opacity from 1 to 0 in 3 seconds
        /// </summary>
        /// <param name="image"></param>
        private void animImageOpacity(Image image)
        {
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
