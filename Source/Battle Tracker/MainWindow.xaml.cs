using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Hardcodet.Wpf.Util;
using System.Xml.Serialization;
using System.IO;
using wforms = System.Windows.Forms;
using xwpf = Xceed.Wpf.Toolkit;
using System.Windows.Controls.Primitives;
using Weather_Calendar;
using Weather_Calendar.Extensions;

namespace Battle_Tracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constants

        //private const int TURN_LENGTH = 5;
        //private const string TURN_LENGTH_STRING = "TurnLength";
        private const string TURN_COUNT_STRING = "Turn Count";
        private const string CURRENT_TURN_STRING = "Current Turn";
        private const string LAST_BATTLE_FILENAME = ".\\Battles\\LastBattle.xml";
        private const string FILE_DIALOG_FILTER = "Extensible Markup Language (.xml)|*.xml";

        #endregion

        #region Properties

        #region DraggedItem

        /// <summary>
        /// DraggedItem Dependency Property
        /// </summary>
        public static readonly DependencyProperty DraggedItemProperty =
            DependencyProperty.Register("DraggedItem", typeof(CombatantData), typeof(MainWindow));

        /// <summary>
        /// Gets or sets the DraggedItem property.  This dependency property 
        /// indicates ....
        /// </summary>
        public CombatantData DraggedItem
        {
            get { return (CombatantData)GetValue(DraggedItemProperty); }
            set { SetValue(DraggedItemProperty, value); }
        }

        #endregion

        public CombatantCollection CombatantList { get; set; } = new CombatantCollection();
        public DispatcherTimer turnTimer { get; private set; } = new DispatcherTimer();
        private int TurnLength { get; set; }
        private WeatherNotesWindow notesWindow;

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            LoadBattle(LAST_BATTLE_FILENAME);

            TurnLength = Properties.Settings.Default.TurnLength;

            CombatantsGrid.Columns[0].Width = Properties.Settings.Default.ColNameWidth;
            CombatantsGrid.Columns[1].Width = Properties.Settings.Default.ColInitWidth;
            CombatantsGrid.Columns[2].Width = Properties.Settings.Default.ColModWidth;
            CombatantsGrid.Columns[3].Width = Properties.Settings.Default.ColHPWidth;
        }

        #endregion

        #region GUI Event Handlers

        private void MnuFileSave_Click(object sender, RoutedEventArgs e)
        {
            SaveBattle();
        }

        private void MnuFileLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadBattle();
        }

        private void MnuFileExit_Click(object sender, RoutedEventArgs e)
        {
            OnApplicationExit();
        }

        private void MnuWeatherNotes_Click(object sender, RoutedEventArgs e)
        {
            if(notesWindow == null)
            {
                notesWindow = new WeatherNotesWindow();
            }

            notesWindow.Closed += NotesWindow_Closed;

            //notesWindow.Show();

            if(notesWindow.IsVisible)
            {
                WindowState state = notesWindow.WindowState;
                if(state == WindowState.Normal || state == WindowState.Maximized)
                {
                    notesWindow.WindowState = WindowState.Minimized;
                }
                else if(state == WindowState.Minimized)
                {
                    notesWindow.WindowState = WindowState.Normal;
                }
            }
            else
            {
                notesWindow.Show();
            }
        }

        private void NotesWindow_Closed(object sender, EventArgs e)
        {
            notesWindow = null;
        }

        private void BtnSort_Click(object sender, RoutedEventArgs e)
        {
            SortCollection();
        }

        private void BtnTurn_Click(object sender, RoutedEventArgs e)
        {
            StartTurn();
        }

        private void TbTimer_Click(object sender, MouseButtonEventArgs e)
        {
            ToggleTimer();
        }

        #endregion

        #region Collection

        public void SortCollection()
        {
            if (CombatantList.Count > 0)
            {
                turnTimer.Stop();

                var orderedList = CombatantList.OrderByDescending(i => i.Initiative).ThenByDescending(i => i.InitModifier).ThenBy(i => i.CombatantName).ToList();

                CombatantList.Reset();

                foreach (var combatant in orderedList)
                {
                    CombatantList.Add(combatant);
                }

                SetTurnText(CombatantList.CurrentCombatantName, TurnLength);
                IudTurnCount.Value = CombatantList.TurnCount;
            }
        }

        private void HighlightGrid(bool highlight = true)
        {
            if (CombatantList.Count > 1)
            {
                ((DataGridRow)CombatantsGrid.ItemContainerGenerator.ContainerFromIndex(CombatantList.CurrentTurn)).Background = highlight ? Brushes.LightGreen : Brushes.White;
                ((DataGridRow)CombatantsGrid.ItemContainerGenerator.ContainerFromIndex(CombatantList.NextTurn)).Background = highlight ? Brushes.LightBlue : Brushes.White;
            }
        }

        #endregion

        #region Turn

        public void StartTurn()
        {
            if (CombatantList.Count > 1)
            {
                HighlightGrid();

                if (turnTimer.IsEnabled)
                {
                    turnTimer.Stop();

                    IterateTurn();
                }
                else
                    Countdown(cur => SetTurnText(time: cur), () => IterateTurn());
            }
        }

        private void IterateTurn()
        {
            ((DataGridRow)CombatantsGrid.ItemContainerGenerator.ContainerFromIndex(CombatantList.CurrentTurn)).Background = Brushes.White;
            
            CombatantList.IterateTurn();

            IudTurnCount.Value = CombatantList.TurnCount;

            StartTurn();
        }

        private void SetTurnText(string combatantName = null, int time = 0)
        {
            if(string.IsNullOrEmpty(combatantName))
                TbTimer.Text = CombatantList.CurrentCombatantName + "'s Turn: " + time.ToString();
            else
            TbTimer.Text = combatantName + "'s Turn: " + time.ToString();
        }

        #endregion

        #region Timer

        public void ToggleTimer()
        {
            if (turnTimer.IsEnabled)
                turnTimer.Stop();
            else
                turnTimer.Start();
        }

        private void Countdown(Action<int> ts, Action fin)
        {
            //var dt = new System.Windows.Threading.DispatcherTimer();
            if (turnTimer.IsEnabled)
            {
                turnTimer.Stop();

                fin();
            }
            //turnTimer.Interval = interval;

            turnTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 1)
            };

            int count = TurnLength;
            turnTimer.Tick += (_, a) =>
            {
                if (count-- <= 0)
                {
                    turnTimer.Stop();

                    fin();
                }
                else
                    ts(count);
            };
            ts(count);
            turnTimer.Start();
        }

        #endregion

        #region Save/Load

        public void SaveBattle(string filename = null)
        {
            if (string.IsNullOrEmpty(filename))
            {
                using (var dialog = new wforms.SaveFileDialog())
                {
                    dialog.Filter = FILE_DIALOG_FILTER;

                    if (dialog.ShowDialog() == wforms.DialogResult.OK)
                        SaveBattle(dialog.FileName);
                }
            }
            else if(CombatantList.Count > 0)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CombatantCollection));

                FileInfo finfo = new FileInfo(filename);
                finfo.Directory.Create();

                using (FileStream stream = new FileStream(filename, FileMode.Create))
                {
                    CombatantData currentTurn = new CombatantData { CombatantName = CURRENT_TURN_STRING, Initiative = CombatantList.CurrentTurn };
                    CombatantList.Add(currentTurn);
                    CombatantData turnCount = new CombatantData { CombatantName = TURN_COUNT_STRING, Initiative = CombatantList.TurnCount };
                    CombatantList.Add(turnCount);

                    serializer.Serialize(stream, CombatantList);

                    CombatantList.Remove(currentTurn);
                    CombatantList.Remove(turnCount);
                }
            }
        }

        public void LoadBattle(string filename = null)
        {
            if (string.IsNullOrEmpty(filename))
            {
                using (var dialog = new wforms.OpenFileDialog())
                {
                    dialog.Filter = FILE_DIALOG_FILTER;

                    if (dialog.ShowDialog() == wforms.DialogResult.OK)
                        LoadBattle(dialog.FileName);
                }
            }
            else if(File.Exists(filename))
            {
                if (turnTimer.IsEnabled)
                    turnTimer.Stop();

                CombatantList.Clear();

                XmlSerializer serializer = new XmlSerializer(typeof(CombatantCollection));

                using (FileStream stream = new FileStream(filename, FileMode.Open))
                {
                    IEnumerable<CombatantData> combatantData = (IEnumerable<CombatantData>)serializer.Deserialize(stream);

                    foreach (CombatantData combatant in combatantData)
                    {
                        if (combatant.CombatantName == CURRENT_TURN_STRING)
                            CombatantList.CurrentTurn = combatant.Initiative;
                        else if (combatant.CombatantName == TURN_COUNT_STRING)
                            CombatantList.TurnCount = combatant.Initiative;
                        else
                            CombatantList.Add(combatant);
                    }

                    SetTurnText(CombatantList.CurrentCombatantName, TurnLength);
                    IudTurnCount.Value = CombatantList.TurnCount;
                }
            }
            else
            {
                for(int i = 1; i < 9; i++)
                    CombatantList.Add(new CombatantData { CombatantName = "Combatant " + i.ToString() });
            }
        }

        #endregion

        #region Exit Handler

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;

            OnApplicationExit();
        }

        private void OnApplicationExit()
        {
            if(notesWindow != null)
            {
                notesWindow.Save();
            }

            SaveBattle(LAST_BATTLE_FILENAME);

            Properties.Settings.Default.ColNameWidth = CombatantsGrid.Columns[0].Width.Value;
            Properties.Settings.Default.ColInitWidth = CombatantsGrid.Columns[1].Width.Value;
            Properties.Settings.Default.ColModWidth = CombatantsGrid.Columns[2].Width.Value;
            Properties.Settings.Default.ColHPWidth = CombatantsGrid.Columns[3].Width.Value;

            Properties.Settings.Default.Save(); 

            Exit();
        }

        private void Exit(int code = 0)
        {
            Environment.Exit(code);
        }

        #endregion

        #region Edit Mode Monitoring

        /// <summary>
        /// State flag which indicates whether the grid is in edit
        /// mode or not.
        /// </summary>
        public bool IsEditing { get; set; }

        private void OnBeginEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            IsEditing = true;
            //in case we are in the middle of a drag/drop operation, cancel it...
            if (IsDragging) ResetDragDrop();
        }

        private void OnEndEdit(object sender, DataGridCellEditEndingEventArgs e)
        {
            IsEditing = false;
        }

        #endregion

        #region Drag and Drop Rows

        /// <summary>
        /// Keeps in mind whether
        /// </summary>
        public bool IsDragging { get; set; }

        /// <summary>
        /// Initiates a drag action if the grid is not in edit mode.
        /// </summary>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsEditing) return;

            var row = UIHelpers.TryFindFromPoint<DataGridRow>((UIElement)sender, e.GetPosition(CombatantsGrid));
            if (row == null || row.IsEditing) return;

            //set flag that indicates we're capturing mouse movements
            IsDragging = true;
            try
            {
                DraggedItem = (CombatantData)row.Item;

                HighlightGrid(false);
                if (turnTimer.IsEnabled)
                    turnTimer.Stop();
            }
            catch
            {
                DraggedItem = null;
                IsDragging = false;
            }
        }


        /// <summary>
        /// Completes a drag/drop operation.
        /// </summary>
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsDragging || IsEditing)
            {
                return;
            }

            //get the target item
            CombatantData targetItem = (CombatantData)CombatantsGrid.SelectedItem;

            if (targetItem == null || !ReferenceEquals(DraggedItem, targetItem))
            {
                //get target index
                int targetIndex = CombatantList.IndexOf(targetItem);

                //remove the source from the list
                CombatantList.Remove(DraggedItem);

                //move source at the target's location
                CombatantList.Insert(Math.Max(targetIndex - 1, 0), DraggedItem);

                //select the dropped item
                //CombatantsGrid.SelectedItem = DraggedItem;
                //CombatantList.CurrentTurn = targetIndex;

                if (CombatantList.CurrentTurn == targetIndex)
                {
                    CombatantList.CurrentTurn = targetIndex - 1;
                    SetTurnText(DraggedItem.CombatantName, TurnLength);
                }
                else if (CombatantList.CurrentTurn > targetIndex)
                    CombatantList.CurrentTurn++;

                SelectRowByIndex(CombatantsGrid, Math.Max(targetIndex - 1, 0));
                //currentTurn = targetIndex;
                //nextTurn = (currentTurn + 1) % CombatantList.Count;
            }

            //reset
            ResetDragDrop();
        }


        /// <summary>
        /// Closes the popup and resets the
        /// grid to read-enabled mode.
        /// </summary>
        private void ResetDragDrop()
        {
            IsDragging = false;
            popup1.IsOpen = false;
            CombatantsGrid.IsReadOnly = false;

            if(turnTimer.IsEnabled)
                Task.Delay(50).ContinueWith(task => Dispatcher.Invoke(() => HighlightGrid()));
        }


        /// <summary>
        /// Updates the popup's position in case of a drag/drop operation.
        /// </summary>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDragging || e.LeftButton != MouseButtonState.Pressed) return;

            //display the popup if it hasn't been opened yet
            if (!popup1.IsOpen)
            {
                //switch to read-only mode
                CombatantsGrid.IsReadOnly = true;

                //make sure the popup is visible
                popup1.IsOpen = true;
            }


            Size popupSize = new Size(popup1.ActualWidth, popup1.ActualHeight);
            popup1.PlacementRectangle = new Rect(e.GetPosition(this), popupSize);

            //make sure the row under the grid is being selected
            Point position = e.GetPosition(CombatantsGrid);
            var row = UIHelpers.TryFindFromPoint<DataGridRow>(CombatantsGrid, position);
            if (row != null)
            {
                //CombatantsGrid.SelectedItem = row.Item;

                SelectRowByIndex(CombatantsGrid, Math.Max(row.GetIndex(), 0));
            }
        }

        #endregion

        #region Duplicate/Delete Entry

        private void OnCombatantGridKeyDown(object sender, KeyEventArgs e)
        {
            if (!IsEditing && CombatantsGrid.SelectedItem is CombatantData)
            {
                if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) && (e.Key == Key.D || e.Key == Key.C))
                {
                    CombatantData dupeData = (CombatantData)CombatantsGrid.SelectedItem;
                    int idx = CombatantList.IndexOf(dupeData) + 1;
                    string name = dupeData.CombatantName;

                    string[] tok = name.Split(' ');

                    if (tok.Length > 1 && int.TryParse(tok[1], out int count))
                        CombatantList.Insert(idx, new CombatantData { CombatantName = tok[0] + " " + (count + 1), HitPoints = dupeData.HitPoints, Initiative = dupeData.Initiative, InitModifier = dupeData.InitModifier, Notes = dupeData.Notes });
                    else
                        CombatantList.Insert(idx, new CombatantData { CombatantName = dupeData.CombatantName, HitPoints = dupeData.HitPoints, Initiative = dupeData.Initiative, InitModifier = dupeData.InitModifier, Notes = dupeData.Notes });

                    //CombatantsGrid.SelectedIndex = idx;
                    //CombatantsGrid.SelectedItem = CombatantList[idx];
                    SelectRowByIndex(CombatantsGrid, idx);
                }
                else if (e.Key == Key.Delete || e.Key == Key.Back)
                {
                    DeleteEntry((CombatantData)CombatantsGrid.SelectedItem);
                }
            }
        }

        #endregion

        #region Insert Entry

        private void OnInsertKeyUp(object sender, KeyEventArgs e)
        {
            if (!IsEditing)
            {
                if (((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) && e.Key == Key.A) || e.Key == Key.Insert)
                {
                    if (CombatantsGrid.SelectedItem is CombatantData)
                    {
                        int idx = CombatantList.IndexOf((CombatantData)CombatantsGrid.SelectedItem) + 1;

                        CombatantList.Insert(idx, new CombatantData { CombatantName = "Combatant " + (CombatantList.Count + 1) });

                        SelectRowByIndex(CombatantsGrid, idx);
                    }
                    else
                    {
                        CombatantList.Add(new CombatantData { CombatantName = "Combatant " + (CombatantList.Count + 1) });

                        SelectRowByIndex(CombatantsGrid, CombatantList.Count - 1);
                    }
                }
            }
        }

        #endregion

        #region Delete Entry

        private void OnDeleteKeyDown(object sender, KeyEventArgs e)
        {
            if(!IsEditing && CombatantsGrid.SelectedItem is CombatantData)
            {
                if (e.Key == Key.Delete || e.Key == Key.Back)
                {
                    DeleteEntry((CombatantData)CombatantsGrid.SelectedItem);
                }
            }
        }

        private void DeleteEntry(CombatantData toDelete)
        {
            int idx = CombatantList.IndexOf(toDelete);

            CombatantList.Remove(toDelete);

            if(idx >= CombatantList.Count)
            {
                SelectRowByIndex(CombatantsGrid, CombatantList.Count - 1);
            }
            else
            {
                SelectRowByIndex(CombatantsGrid, idx);
            }
        }

        #endregion

        #region Entry Index Helpers

        // Code written by Magnus at https://social.technet.microsoft.com/wiki/contents/articles/21202.wpf-programmatically-selecting-and-focusing-a-row-or-cell-in-a-datagrid.aspx?Redirected=true

        public static void SelectRowByIndex(DataGrid dataGrid, int rowIndex)
        {
            if (!dataGrid.SelectionUnit.Equals(DataGridSelectionUnit.FullRow))
                throw new ArgumentException("The SelectionUnit of the DataGrid must be set to FullRow.");

            if (rowIndex < 0 || rowIndex > (dataGrid.Items.Count - 1))
                throw new ArgumentException(string.Format("{0} is an invalid row index.", rowIndex));

            if(dataGrid.SelectionMode == DataGridSelectionMode.Extended)
                dataGrid.SelectedItems.Clear();
            /* set the SelectedItem property */
            object item = dataGrid.Items[rowIndex]; // = Product X
            dataGrid.SelectedItem = item;

            DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            if (row == null)
            {
                /* bring the data item (Product object) into view
                 * in case it has been virtualized away */
                dataGrid.ScrollIntoView(item);
                row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            }

            if (row != null)
            {
                DataGridCell cell = GetCell(dataGrid, row, 0);
                if (cell != null)
                    cell.Focus();
            }
        }

        public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
        {
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {
                    /* if the row has been virtualized away, call its ApplyTemplate() method
                     * to build its visual tree in order for the DataGridCellsPresenter
                     * and the DataGridCells to be created */
                    rowContainer.ApplyTemplate();
                    presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                }
                if (presenter != null)
                {
                    DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    if (cell == null)
                    {
                        /* bring the column into view
                         * in case it has been virtualized away */
                        dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    }
                    return cell;
                }
            }
            return null;
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        #endregion
        
    }

    [Serializable]
    public class CombatantCollection : ObservableCollection<CombatantData>
    {
        #region Properties

        #region Current Turn

        private int currentTurn = 0;
        
        [XmlAttribute("CurrentTurn")]
        public int CurrentTurn
        {
            get
            {
                return currentTurn;
            }
            set
            {
                currentTurn = value;
                OnPropertyChanged("CurrentTurn");
            }
        }

        #endregion

        #region Next Turn

        public int NextTurn
        {
            get
            {
                return (currentTurn + 1) % Count;
            }
        }

        #endregion

        #region Current Combatant Name

        public string CurrentCombatantName
        {
            get
            {
                if (Count <= currentTurn)
                    return "";

                return this[currentTurn].CombatantName;
            }
        }

        #endregion

        #region Turn Count

        private int turnCount = 1;

        [XmlAttribute("CurrentRound")]
        public int TurnCount
        {
            get
            {
                return turnCount;
            }
            set
            {
                turnCount = value;
                OnPropertyChanged("TurnCount");
            }
        }

        #endregion

        #endregion

        #region CombatantCollection

        public void Reset()
        {
            Clear();

            CurrentTurn = 0;
            TurnCount = 1;
        }

        public void IterateTurn()
        {
            CurrentTurn = (currentTurn + 1) % Count;

            if (CurrentTurn == 0)
                TurnCount++;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    [Serializable]
    public class CombatantData : INotifyPropertyChanged
    {
        #region Properties

        #region Combatant Name

        private string combatantName = "Combatant";

        public string CombatantName
        {
            get { return combatantName; }
            set
            {
                combatantName = value;
                OnPropertyChanged("CombatantName");
            }
        }

        #endregion

        #region Initiative

        private int initiative = 0;

        public int Initiative
        {
            get { return initiative; }
            set
            {
                initiative = value;
                OnPropertyChanged("Initiative");
            }
        }

        #endregion

        #region InitModifier

        private int initModifier = 0;

        public int InitModifier
        {
            get { return initModifier; }
            set
            {
                initModifier = value;
                OnPropertyChanged("InitModifier");
            }
        }

        #endregion

        #region HitPoints

        private int hitPoints = 0;

        public int HitPoints
        {
            get { return hitPoints; }
            set
            {
                hitPoints = value;
                OnPropertyChanged("HitPoints");
            }
        }

        #endregion

        #region Notes

        private string notes = "";

        public string Notes
        {
            get { return notes; }
            set
            {
                notes = value;
                OnPropertyChanged("Notes");
            }
        }

        #endregion

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
