
using Calendar;
using System.Data.SQLite;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Data.Entity.Core.Objects;
using System.Xml.Linq;
using System.Windows.Interop;
using static Calendar.Category;
using System.Windows.Markup;
using System.CodeDom;
using System.Reflection.PortableExecutable;
using System.Linq;


namespace HomeCalendarGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window, IView
    {
        private readonly Presenter presenter;
        private UpdateEventsWindow updateEventsWindow;
        private List<Category> categories; 

        private OpenFolderDialog openFolderDialog;
        private string fileDirectoryToStore;

        public MainWindow(bool useDefaultDb, string filePath = null)
        {
            InitializeComponent();

            //Create Calendar directory if it doesn't exist
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Calendar"))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Calendar");
            }

            //Open Folder Dialog properties
            openFolderDialog = new OpenFolderDialog
            {
                DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Calendar",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Calendar",
            };

            //Validate if user is using the default database or specified database
            if (useDefaultDb)
            {
                presenter = new Presenter(this);
            }
            else
            {
                presenter = new Presenter(this, filePath);

            }

            presenter.GetCategoriesForAllCatsComboBoxes();
            presenter.GetCategoriesTypeInList();            
            presenter.GetHomeCalendarItems(null,null,0,false,false,false,false);
            SetTodaysDateOnDatePicker();            
        }

        #region IView
        public void DisplayErrorMessage(string msg)
        {
            message.Foreground = Brushes.Red;
            message.Text = msg;
        }

        public void DisplaySuccessfulMessage(string msg)
        {
            message.Foreground = Brushes.Green;
            message.Text = msg;
        }

        public void PopulateAllCategoriesComboBox(List<Category> categories)
        {
            this.categories = categories;
            catsComboBox.Items.Clear();
            CategoryFilterCmb.Items.Clear();
            
            // Sort categories alphabetically by their Description
            var sortedCategories = categories.OrderBy(c => c.Description).ToList();

            const int DEFAULT = 0;
            sortedCategories.ForEach(c => {
                catsComboBox.Items.Add(c);
                CategoryFilterCmb.Items.Add(c);
            });
            catsComboBox.SelectedIndex = DEFAULT;
            CategoryFilterCmb.SelectedIndex = DEFAULT;
        }      

        public void PopulateCategoryTypesComboBox(List<Category> categories)
        {
            foreach (var category in categories)
            {
                if (categoryTypecmbBox.Items.Contains(category.Type))
                {
                    //ignoring event types that have already been added because we do not want duplicates
                    continue;
                }
                categoryTypecmbBox.Items.Add(category.Type);
            }
        }

        public void ShowCalendarItems(List<CalendarItem> items)
        {   
            SetDataGridColumnsToDefault();
            CalendarItemsDataGrid.ItemsSource = items;
            //DGBusyTime.Visibility = Visibility.Visible;
            //DGStartTime.Visibility = Visibility.Visible;
            //DGDurationInMinutes.Visibility = Visibility.Visible;
            //DGDescription.Visibility = Visibility.Visible;
            //DGCategory.Visibility = Visibility.Visible;
            //DGStartDate.Visibility = Visibility.Visible;

            ////DGKeyColumn.Visibility = Visibility.Hidden;
            ////DGValueColumn.Visibility = Visibility.Hidden;
            //DGTotalBusyTime.Visibility = Visibility.Hidden;
            //DGMonth.Visibility = Visibility.Hidden;
        }        
        
        public void ShowTotalBusyTimeByMonth(List<CalendarItemsByMonth> itemsByMonth)
        {
            SetDataGridColumnsToSummaryByMonth();
            CalendarItemsDataGrid.ItemsSource = itemsByMonth;
            //DGStartDate.Visibility = Visibility.Hidden;
            //DGBusyTime.Visibility = Visibility.Hidden;
            //DGStartTime.Visibility = Visibility.Hidden;
            //DGDurationInMinutes.Visibility = Visibility.Hidden;
            //DGDescription.Visibility = Visibility.Hidden;
            //DGCategory.Visibility = Visibility.Hidden;

            //DGTotalBusyTime.Visibility = Visibility.Visible;
            //DGMonth.Visibility = Visibility.Visible;
        }

        public void ShowTotalBusyTimeByCategory(List<CalendarItemsByCategory> itemsByCategory)
        {
            SetDataGridColumnsToSummaryByCategory();
            CalendarItemsDataGrid.ItemsSource = itemsByCategory;
            //DGStartDate.Visibility = Visibility.Hidden;
            //DGBusyTime.Visibility = Visibility.Hidden;
            //DGStartTime.Visibility = Visibility.Hidden;
            //DGDurationInMinutes.Visibility = Visibility.Hidden;
            //DGDescription.Visibility = Visibility.Hidden;
            
            //DGCategory.Visibility = Visibility.Visible;
            //DGTotalBusyTime.Visibility = Visibility.Visible;           
        }

        public void ShowTotalBusyTimeByMonthAndCategory(List<Dictionary<string, object>> itemsByCategoryAndMonth)
        {

            CalendarItemsDataGrid.ItemsSource = itemsByCategoryAndMonth;
            CalendarItemsDataGrid.Columns.Clear();

            // get list of column name from first dictionary in the list
            // and create column and bind to dictionary element

            bool skipKey = false;

            //Go through each section of the dictionary
            for (int i = 0; i < itemsByCategoryAndMonth.Count; i++)
            {   
                //Go through each key
                foreach (string key in itemsByCategoryAndMonth[i].Keys)
                {
                    List<DataGridColumn> currentTextColumns = CalendarItemsDataGrid.Columns.ToList();
                    skipKey = false;

                    //Ommit the ones that have "items:"
                    if (key.Contains("items:"))
                    {
                        continue;
                    }

                    //Checks if a column header from dictionary already exists
                    foreach (var currentDGData in currentTextColumns)
                    {
                        if (currentDGData.Header.Equals(key))
                        {
                            skipKey = true;
                        }
                    }

                    if (skipKey)
                    {
                        continue;
                    }

                    var column = new DataGridTextColumn();
                    column.Header = key;
                    column.Binding = new Binding($"[{key}]");
                    CalendarItemsDataGrid.Columns.Add(column);
                }
            }
        }
        #endregion

        private void RefreshMainView()
        {
            presenter.GetCategoriesTypeInList();
            presenter.GetCategoriesForAllCatsComboBoxes();            
            
            try
            {
                //Check if the user is filtering by category and/ or month
                bool summaryByMonth = (bool)SummaryByMonthCheckBox.IsChecked;
                bool summaryByCategory = (bool)SummaryByCategoryCheckBox.IsChecked;
                bool filterByCategory = (bool)FilterByCategoryCheckBox.IsChecked;

                if (DateFilterCheckBox.IsChecked == true)
                {
                    DateTime? start = Start.SelectedDate;
                    DateTime? end = End.SelectedDate;
                    bool filterByDate = (bool)DateFilterCheckBox.IsChecked;
                    Category cat = CategoryFilterCmb.SelectedItem as Category;
                    int categoryId = cat.Id;

                    presenter.GetHomeCalendarItems(start, end, categoryId, filterByDate, summaryByCategory, summaryByMonth, filterByCategory);
                    
                }
                else
                {
                    bool filterByDate = (bool)DateFilterCheckBox.IsChecked;
                    Category cat = CategoryFilterCmb.SelectedItem as Category;
                    int categoryId = cat.Id;
                    presenter.GetHomeCalendarItems(null, null, categoryId, filterByDate, summaryByCategory, summaryByMonth, filterByCategory);                    
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"An unknown error occured: {ex.Message}","Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void SetTodaysDateOnDatePicker()
        {
            StartDate.DisplayDateStart = DateTime.Now;
        }

        private void SetDataGridColumnsToDefault()
        {
            const int StartDate = 0, StartTime = 1;

            List<DataGridTextColumn> textColumns = new List<DataGridTextColumn>
            {
                new DataGridTextColumn {Header ="Start Date", Binding = new Binding("StartDateTime")},
                new DataGridTextColumn {Header ="Start Time", Binding = new Binding("StartDateTime")},
                new DataGridTextColumn {Header ="Category", Binding = new Binding("Category")},
                new DataGridTextColumn {Header ="Description", Binding = new Binding("ShortDescription")},
                new DataGridTextColumn {Header ="Duration", Binding = new Binding("DurationInMinutes")},
                new DataGridTextColumn {Header ="Busy Time", Binding = new Binding("BusyTime")},
            };
            textColumns[StartDate].Binding.StringFormat = "yyyy/MM/dd";
            textColumns[StartTime].Binding.StringFormat = "hh:mm:ss";


            CalendarItemsDataGrid.Columns.Clear();     // Clear all existing columns on the DataGrid control.                                                                   
            textColumns.ForEach(CalendarItemsDataGrid.Columns.Add);
        }

        private void SetDataGridColumnsToSummaryByMonth()
        {
            //< DataGridTextColumn x: Name = "DGMonth" Visibility = "Hidden" Header = "Month" Binding = "{Binding Month}" ></ DataGridTextColumn >
            //< DataGridTextColumn x: Name = "DGTotalBusyTime" Visibility = "Hidden"  Header = "Total Busy Time" Binding = "{Binding TotalBusyTime}" ></ DataGridTextColumn >

            List<DataGridTextColumn> textColumns = new List<DataGridTextColumn>
            {
                new DataGridTextColumn {Header ="Month", Binding = new Binding("Month")},
                new DataGridTextColumn {Header ="Total Busy Time", Binding = new Binding("TotalBusyTime")},
            };

            CalendarItemsDataGrid.Columns.Clear();     // Clear all existing columns on the DataGrid control.                                                                   
            textColumns.ForEach(CalendarItemsDataGrid.Columns.Add);
        }

        private void SetDataGridColumnsToSummaryByCategory()
        {
            List<DataGridTextColumn> textColumns = new List<DataGridTextColumn>
            {
                new DataGridTextColumn {Header ="Category", Binding = new Binding("Category")},
                new DataGridTextColumn {Header ="Total Busy Time", Binding = new Binding("TotalBusyTime")},
            };

            CalendarItemsDataGrid.Columns.Clear();     // Clear all existing columns on the DataGrid control.                                                                   
            textColumns.ForEach(CalendarItemsDataGrid.Columns.Add);
        }

        private bool IsValidDescription(string desc)
        {
            if (string.IsNullOrEmpty(desc))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(desc))
            {
                return false;
            }
            if (float.TryParse(desc, out float s))
            {
                return false;
            }
            return true;
        }

        #region Btn Operations
        private void Btn_SaveCalendarFileTo(object sender, RoutedEventArgs e)
        {
            if (openFolderDialog.ShowDialog() == true)
            {
                fileDirectoryToStore = openFolderDialog.FolderName;
                openFolderDialog.InitialDirectory = fileDirectoryToStore;
                openFolderDialog.FolderName = "";
            }
        }

        private void Button_ClickAddCategory(object sender, RoutedEventArgs e)
        {
            var eventTypeChoice = categoryTypecmbBox.SelectedItem;
            string desc = DescriptionBox.Text;

            if (eventTypeChoice != null && !string.IsNullOrEmpty(desc))
            {
                CategoryType type = (CategoryType)eventTypeChoice;

                //checking if description is filled of numbers which would be unvalid 
                if (!IsValidDescription(desc))
                {
                    DisplayErrorMessage("Please enter a valid description.");
                    return;
                }

                presenter.AddNewCategory(desc, type);
                DescriptionBox.Clear();
                categoryTypecmbBox.SelectedIndex = -1;
                RefreshMainView();                
            }
            else
            {
                DisplayErrorMessage("You cannot leave any fields empty.");
            }
        }
        
        private void Button_ClickAddEvent(object sender, RoutedEventArgs e)
        {

            //verifying input data for events 
            if (StartDate.SelectedDate == null)
            {
                DisplayErrorMessage("Please select a start date for the event.");
                return;
            }
            if (!DateTime.TryParse(StartTime.Text, out DateTime time))
            {
                DisplayErrorMessage("Time isn't provided in the right format");
                return;
            }
            if (catsComboBox.SelectedItem == null)
            {
                DisplayErrorMessage("Please select a category for the event.");
                return;
            }
            if (!IsValidDescription(EventDescriptionBox.Text))
            {
                DisplayErrorMessage("Please enter a description for the event.");
                return;
            }
            if (DurationInMinutes.Text == null)
            {
                DisplayErrorMessage("Please choose a duration in minutes for your event.");
                return;
            }            


            //Add the time
            DateTime startDate = StartDate.SelectedDate.Value;            
            startDate = startDate.AddHours(time.Hour);
            startDate = startDate.AddMinutes(time.Minute);
            startDate = startDate.AddSeconds(time.Second);

            //getting duration in minutes
            if (!double.TryParse(DurationInMinutes.Text, out double endTimeInMinutes))
            {
                DisplayErrorMessage("Please enter a number for your duration in minutes.");
                return;
            }

            if (endTimeInMinutes <= 0)
            {
                DisplayErrorMessage("The duration of your event must not be negative.");
                return;
            }

            Category selectedCategory = (Category)catsComboBox.SelectedItem;
            string description = EventDescriptionBox.Text;

            presenter.AddNewEvent(startDate, selectedCategory.Id, description, endTimeInMinutes);

            // Clear the input fields
            StartDate.SelectedDate = null;            
            catsComboBox.SelectedIndex = -1;
            StartTime.Clear();
            DurationInMinutes.Clear();
            EventDescriptionBox.Clear();   
            DisplaySuccessfulMessage("Event added successfully.");
            RefreshMainView();
        }

        private void Button_ClickCancelEvent(object sender, RoutedEventArgs e)
        {
            EventDescriptionBox.Clear();
        }

        private void Btn_DeleteEvent(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to permanently delete this event?", "Deleting an Event", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                CalendarItem item = CalendarItemsDataGrid.SelectedItem as CalendarItem;
                try
                {
                    presenter.DeleteAnEvent(item.EventID);
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
                }
                RefreshMainView();
            }
        }

        private void Btn_UpdateEvent(object sender, RoutedEventArgs e)
        {
            CalendarItem item = CalendarItemsDataGrid.SelectedItem as CalendarItem;

            updateEventsWindow = new UpdateEventsWindow(presenter, categories, item, this);
            //creating update window to open it 
            //UpdateEventsWindow updateWindow = new UpdateEventsWindow(this.presenter, item);

            //creating this element because i will need this ID later to update and i can not access the updated element in the update window

            //TextBlock textBlock = new TextBlock();
            //textBlock.Text = $"{item.EventID}";

            ////i do not want the user to see this, it is only for me
            //textBlock.Visibility = Visibility.Hidden;

            ////adding it to the update events window
            //updateWindow.UpdateEventGrid.Children.Add(textBlock); 

            this.Hide();
            updateEventsWindow.Show();
        }

        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region Filters CheckBox Operations
        private void DateFilterCheckBoxClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //Check if the user is filtering by category and/ or month
                bool summaryByMonth = (bool)SummaryByMonthCheckBox.IsChecked;
                bool summaryByCategory = (bool)SummaryByCategoryCheckBox.IsChecked;
                bool filterByCategory = (bool)FilterByCategoryCheckBox.IsChecked;

                if (DateFilterCheckBox.IsChecked == true)
                {
                    DateTime? start = Start.SelectedDate;
                    DateTime? end = End.SelectedDate;
                    bool filterByDate = (bool)DateFilterCheckBox.IsChecked;
                    Category cat = CategoryFilterCmb.SelectedItem as Category;
                    int categoryId = cat.Id;

                    presenter.GetHomeCalendarItems(start, end, categoryId, filterByDate, summaryByCategory, summaryByMonth, filterByCategory);
                    //presenter.GetEventsFilteredByDateRange(Start.SelectedDate, End.SelectedDate);
                }
                else
                {
                    bool filterByDate = (bool)DateFilterCheckBox.IsChecked;
                    Category cat = CategoryFilterCmb.SelectedItem as Category;
                    int categoryId = cat.Id;
                    presenter.GetHomeCalendarItems(null, null, categoryId, filterByDate, summaryByCategory, summaryByMonth, filterByCategory);
                    //presenter.GetEventsFilteredByDateRange(Start.SelectedDate, End.SelectedDate);
                }
            }
            catch (InvalidOperationException ex)
            {
                if (ex is InvalidOperationException)
                {
                    MessageBox.Show($"Date Filter Error: {ex.Message}", "DateTime Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Unknown Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                DateFilterCheckBox.IsChecked = false;
                Start.SelectedDate = null;
                End.SelectedDate = null;
            }
        }

        private void FilterByCategoryCheckBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Check if the user is filtering by category and/ or month
                bool summaryByMonth = (bool)SummaryByMonthCheckBox.IsChecked;
                bool summaryByCategory = (bool)SummaryByCategoryCheckBox.IsChecked;
                bool filterByCategory = (bool)FilterByCategoryCheckBox.IsChecked;

                if (DateFilterCheckBox.IsChecked == true)
                {
                    DateTime? start = Start.SelectedDate;
                    DateTime? end = End.SelectedDate;
                    bool filterByDate = (bool)DateFilterCheckBox.IsChecked;
                    Category cat = CategoryFilterCmb.SelectedItem as Category;
                    int categoryId = cat.Id;

                    presenter.GetHomeCalendarItems(start, end, categoryId, filterByDate, summaryByCategory, summaryByMonth, filterByCategory);
                }
                else
                {
                    bool filterByDate = (bool)DateFilterCheckBox.IsChecked;
                    Category cat = CategoryFilterCmb.SelectedItem as Category;
                    int categoryId = cat.Id;
                    presenter.GetHomeCalendarItems(null, null, categoryId, filterByDate, summaryByCategory, summaryByMonth, filterByCategory);
                }
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException)
                {
                    MessageBox.Show($"Date Filter Error: {ex.Message}", "DateTime Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Unknown Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                SummaryByCategoryCheckBox.IsChecked = false;
            }
        }

        private void SummaryByMonthCheckBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Check if the user is filtering by category and/ or month
                bool summaryByMonth = (bool)SummaryByMonthCheckBox.IsChecked;
                bool summaryByCategory = (bool)SummaryByCategoryCheckBox.IsChecked;
                bool filterByCategory = (bool)FilterByCategoryCheckBox.IsChecked;

                if (DateFilterCheckBox.IsChecked == true)
                {
                    DateTime? start = Start.SelectedDate;
                    DateTime? end = End.SelectedDate;
                    bool filterByDate = (bool)DateFilterCheckBox.IsChecked;
                    Category cat = CategoryFilterCmb.SelectedItem as Category;
                    int categoryId = cat.Id;

                    presenter.GetHomeCalendarItems(start, end, categoryId, filterByDate, summaryByCategory, summaryByMonth, filterByCategory);
                }
                else
                {
                    bool filterByDate = (bool)DateFilterCheckBox.IsChecked;
                    Category cat = CategoryFilterCmb.SelectedItem as Category;
                    int categoryId = cat.Id;
                    presenter.GetHomeCalendarItems(null, null, categoryId, filterByDate, summaryByCategory, summaryByMonth, filterByCategory);
                }


                //if (FilterByMonthCheckBox.IsChecked == true)
                //{
                //    DateTime start = Start.SelectedDate.Value;
                //    DateTime end = End.SelectedDate.Value;

                //    presenter.GetCalendarItemsFilteredByMonth(start, end);
                //}
                //else
                //{
                //    presenter.GetCalendarItems();
                //}
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException)
                {
                    MessageBox.Show($"Date Filter Error: {ex.Message}", "DateTime Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Unknown Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                SummaryByMonthCheckBox.IsChecked = false;
            }
        }

        private void SummaryByCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Check if the user is filtering by category and/ or month
                bool summaryByMonth = (bool)SummaryByMonthCheckBox.IsChecked;
                bool summaryByCategory = (bool)SummaryByCategoryCheckBox.IsChecked;
                bool filterByCategory = (bool)FilterByCategoryCheckBox.IsChecked;

                if (DateFilterCheckBox.IsChecked == true)
                {
                    DateTime? start = Start.SelectedDate;
                    DateTime? end = End.SelectedDate;
                    bool filterByDate = (bool)DateFilterCheckBox.IsChecked;
                    Category cat = CategoryFilterCmb.SelectedItem as Category;
                    int categoryId = cat.Id;

                    presenter.GetHomeCalendarItems(start, end, categoryId, filterByDate, summaryByCategory, summaryByMonth, filterByCategory);
                }
                else
                {
                    bool filterByDate = (bool)DateFilterCheckBox.IsChecked;
                    Category cat = CategoryFilterCmb.SelectedItem as Category;
                    int categoryId = cat.Id;
                    presenter.GetHomeCalendarItems(null, null, categoryId, filterByDate, summaryByCategory, summaryByMonth, filterByCategory);
                }
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException)
                {
                    MessageBox.Show($"Date Filter Error: {ex.Message}", "DateTime Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Unknown Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                SummaryByCategoryCheckBox.IsChecked = false;
            }
        }

        private void CategoryFilterCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //Check if the user is filtering by category and/ or month
                bool summaryByMonth = (bool)SummaryByMonthCheckBox.IsChecked;
                bool summaryByCategory = (bool)SummaryByCategoryCheckBox.IsChecked;
                bool filterByCategory = (bool)FilterByCategoryCheckBox.IsChecked;

                if (DateFilterCheckBox.IsChecked == true)
                {
                    DateTime? start = Start.SelectedDate;
                    DateTime? end = End.SelectedDate;
                    bool filterByDate = (bool)DateFilterCheckBox.IsChecked;
                    Category cat = CategoryFilterCmb.SelectedItem as Category;
                    int categoryId = cat == null ? 0 : cat.Id;

                    presenter.GetHomeCalendarItems(start, end, categoryId, filterByDate, summaryByCategory, summaryByMonth, filterByCategory);
                }
                else
                {
                    bool filterByDate = (bool)DateFilterCheckBox.IsChecked;
                    Category cat = CategoryFilterCmb.SelectedItem as Category;
                    int categoryId = cat == null ? 0 : cat.Id;
                    presenter.GetHomeCalendarItems(null, null, categoryId, filterByDate, summaryByCategory, summaryByMonth, filterByCategory);
                }
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException)
                {
                    MessageBox.Show($"Date Filter Error: {ex.Message}", "DateTime Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Unknown Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                SummaryByCategoryCheckBox.IsChecked = false;
            }
        }
        #endregion        
    }
}
