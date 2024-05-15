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


namespace HomeCalendarGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window, IView
    {
        private readonly Presenter presenter;

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

            presenter.GetCategoriesForComboBox();
            presenter.GetCategoriesTypeInList();
            presenter.GetCalendarItems();
            SetTodaysDateOnDatePicker();
            LoadCategoriesForFiltering();
        }
        #region IView
        public void SetTodaysDateOnDatePicker()
        {
            StartDate.DisplayDateStart = DateTime.Now;           
        }

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

        public void PopulateCategoriesComboBox(List<Category> categories)
        {
            catsComboBox.Items.Clear();
            // Sort categories alphabetically by their Description
            var sortedCategories = categories.OrderBy(c => c.Description).ToList();

            const int DEFAULT = 0;
            sortedCategories.ForEach(c => {
                catsComboBox.Items.Add(c);
            });
            catsComboBox.SelectedIndex = DEFAULT;
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

        public void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            //????
            if (CategoryFilter.SelectedItem != null)
            {
                var selectedCategory = (Category)CategoryFilter.SelectedItem;
                presenter.GetEventsFilteredByCategory(selectedCategory.Id);

            }
            else
            {
                DisplayErrorMessage("Please select a category to filter by.");
            }
        }

        public void ShowCalendarItemsWithCategoryFiltersOn(List<CalendarItem> calendarItems)
        {
            if (calendarItems.Count == 0)
            {
                message2.Text = "No events found for this category.";
                message2.Foreground = Brushes.Red;
            }
            else
            {
                CalendarItemsDataGrid.ItemsSource = calendarItems;
                // Clear any previous messages
                message2.Text = "";
            }
        }             

        public void ShowCalendarItemsOnDataGrid(List<CalendarItem> calendarItems)
        {
            CalendarItemsDataGrid.ItemsSource = calendarItems;
            DGBusyTime.Visibility = Visibility.Visible;
            DGStartTime.Visibility = Visibility.Visible;
            DGDurationInMinutes.Visibility = Visibility.Visible;
            DGDescription.Visibility = Visibility.Visible;
            DGCategory.Visibility = Visibility.Visible;
            DGStartDate.Visibility = Visibility.Visible;

            DGKeyColumn.Visibility = Visibility.Hidden;
            DGValueColumn.Visibility = Visibility.Hidden;
        }
        
        public void ShowCalendarItemsWithDateFiltersOn(List<CalendarItem> calendarItems)
        {
            CalendarItemsDataGrid.ItemsSource = calendarItems;
        }
       
        public bool IsValidDescription(string desc)
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

        public void ShowCalendarItemsFilteredByMonth(Dictionary<string, Double> itemsByMonthAndTime)
        {
            CalendarItemsDataGrid.ItemsSource = itemsByMonthAndTime;
            DGStartDate.Visibility = Visibility.Hidden;
            DGBusyTime.Visibility = Visibility.Hidden;
            DGStartTime.Visibility = Visibility.Hidden;
            DGDurationInMinutes.Visibility = Visibility.Hidden;
            DGDescription.Visibility = Visibility.Hidden;
            DGCategory.Visibility = Visibility.Hidden;

            DGKeyColumn.Visibility = Visibility.Visible;
            DGValueColumn.Visibility = Visibility.Visible;
        }

        public void ShowCalendarItems(List<CalendarItem> items)
        {
            CalendarItemsDataGrid.ItemsSource = items;
        }

        public void ShowCalendarItemsByMonth(List<CalendarItemsByMonth> itemsByMonth)
        {
            throw new NotImplementedException();
        }

        public void ShowCalendarItemsByCategory(List<CalendarItemsByCategory> itemsByCategory)
        {
            throw new NotImplementedException();
        }

        public void ShowCalendarItemsByMonthAndCategory(List<Dictionary<string, object>> itemsByCategoryAndMonth)
        {
            throw new NotImplementedException();
        }


        #endregion

        private void RefreshMainView()
        {
            presenter.GetCategoriesForComboBox();
        }

        private void LoadCategoriesForFiltering()
        {
            CategoryFilter.ItemsSource = presenter.RetrieveCategories();
            CategoryFilter.DisplayMemberPath = "Description";
            CategoryFilter.SelectedValuePath = "Id";
            CategoryFilter.SelectedIndex = 0;  
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
                LoadCategoriesForFiltering();
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
        }

        private void Button_ClickCancelEvent(object sender, RoutedEventArgs e)
        {
            EventDescriptionBox.Clear();
        }

        private void Btn_DeleteEvent(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to permanently delete this event?", "Deleting an Event",MessageBoxButton.YesNo,MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                CalendarItem item = CalendarItemsDataGrid.SelectedItem as CalendarItem;                
                try
                {
                    presenter.DeleteAnEvent(item.EventID);
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Hand);
                }                
            }
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
                //Check if the user is filtering by category and/or month
                bool filterByMonth = (bool)FilterByMonthCheckBox.IsChecked;
                bool filterByCategory = (bool)FilterByCategoryCheckBox.IsChecked;                

                if (DateFilterCheckBox.IsChecked == true)
                {
                    DateTime? start = Start.SelectedDate;
                    DateTime? end = End.SelectedDate;
                    bool filterByDate = (bool)DateFilterCheckBox.IsChecked;
                    int categoryId = CategoryFilter.SelectedIndex;
                    presenter.GetHomeCalendarItems(start,end,categoryId,filterByDate,filterByCategory,filterByMonth);

                    //presenter.GetEventsFilteredByDateRange(Start.SelectedDate, End.SelectedDate);
                }
                else
                {
                    bool filterByDate = (bool)DateFilterCheckBox.IsChecked;
                    int categoryId = CategoryFilter.SelectedIndex;
                    presenter.GetHomeCalendarItems(null,null,categoryId,filterByDate,filterByCategory,filterByMonth);
                    
                    //presenter.GetCalendarItems();
                }
            }
            catch(InvalidOperationException ex)
            {
                if(ex is InvalidOperationException)
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

        private void FilterByMonthCheckBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Check if the user is filtering by category and/ or month
                bool filterByMonth = (bool)FilterByMonthCheckBox.IsChecked;
                bool filterByCategory = (bool)FilterByCategoryCheckBox.IsChecked;

                if (DateFilterCheckBox.IsChecked == true)
                {
                    DateTime? start = Start.SelectedDate;
                    DateTime? end = End.SelectedDate;
                    bool filterByDate = (bool)DateFilterCheckBox.IsChecked;
                    int categoryId = CategoryFilter.SelectedIndex;

                    presenter.GetHomeCalendarItems(start, end, categoryId, filterByDate, filterByCategory, filterByMonth);
                }
                else
                {
                    bool filterByDate = (bool)DateFilterCheckBox.IsChecked;
                    int categoryId = CategoryFilter.SelectedIndex;
                    presenter.GetHomeCalendarItems(null, null, categoryId, filterByDate, filterByCategory, filterByMonth);
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
                MessageBox.Show("Must select a Start and End Date.","Start or End date not selected",MessageBoxButton.OK,MessageBoxImage.Error);
                FilterByMonthCheckBox.IsChecked = false;
            }
        }

        private void FilterByCategoryCheckBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch(Exception ex)
            {
                FilterByCategoryCheckBox.IsChecked = false;
            }
        }
        #endregion
    }
}