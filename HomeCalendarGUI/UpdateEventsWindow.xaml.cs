using Calendar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace HomeCalendarGUI
{
    /// <summary>
    /// Interaction logic for UpdateEventsWindow.xaml
    /// </summary>
    public partial class UpdateEventsWindow : Window, IView
    {
        private Presenter presenter;
        private CalendarItem itemToUpdate;
        
        private MainWindow mainWindow;
        private OpenFileWindowOnInitialization openFileWindow;


        public UpdateEventsWindow(Presenter p, List<Category> categories, CalendarItem item, MainWindow main)
        {
            presenter = p;
            itemToUpdate = item;
            mainWindow = main;
            InitializeComponent();

            PopulateCategoriesComboxBox(categories);
            CreateTimePicker();
            StartDate.DisplayDateStart = DateTime.Now;
        }

        public void PopulateCategoriesComboxBox(List<Category> categories)
        {
            foreach (Category category in categories) 
            { 
                updatedCategoriesCmb.Items.Add(category);
            }
        }

        /// <summary>
        /// Initializes the timepicker and adds it to the window
        /// </summary>
        public void CreateTimePicker()
        {
            TimePicker startTimePicker = new TimePicker();
            startTimePicker.AllowTextInput = false;
            startTimePicker.Name = "startTime";
            startTimePicker.Margin = new Thickness(0, 5, 0, 0);

            StartTimeGrid.Children.Add(startTimePicker);

        }

        public void DisplayErrorMessage(string msg)
        {
            message.Text = msg;
            message.Foreground = Brushes.Red;
        }

        public void DisplaySuccessfulMessage(string msg)
        {
            message.Text = msg;
            message.Foreground = Brushes.Green;
        }

        public void PopulateAllCategoriesComboBox(List<Category> categories)
        {
            updatedCategoriesCmb.Items.Clear();

            // Sort categories alphabetically by their Description
            var sortedCategories = categories.OrderBy(c => c.Description).ToList();

            const int DEFAULT = 0;
            sortedCategories.ForEach(c => {
                updatedCategoriesCmb.Items.Add(c);
            });
            updatedCategoriesCmb.SelectedIndex = DEFAULT;
        }

        #region UnusedViewMethods

        public void PopulateCategoryTypesComboBox(List<Category> categories)
        {
            throw new NotImplementedException();
        }

        public void ShowCalendarItems(List<CalendarItem> items)
        {
            throw new NotImplementedException();
        }

        public void ShowTotalBusyTimeByCategory(List<CalendarItemsByCategory> itemsByCategory)
        {
            throw new NotImplementedException();
        }

        public void ShowTotalBusyTimeByMonth(List<CalendarItemsByMonth> itemsByMonth)
        {
            throw new NotImplementedException();
        }

        public void ShowTotalBusyTimeByMonthAndCategory(List<Dictionary<string, object>> itemsByCategoryAndMonth)
        {
            throw new NotImplementedException();
        }


        public void PopulateCategoriesInAllCatsComboBox(List<Category> categories)
        {
            throw new NotImplementedException();
        }
        #endregion

        private void Btn_UpdateEvent(object sender, RoutedEventArgs e)
        {

            //duration of the event
            //casting to TimePicker objects because when retrieving them, they are not.
            try
            {
                //this is a must
                int eventId = itemToUpdate.EventID;

                //retrieving the correct date and time
                var startTimePicker = (TimePicker)StartTimeGrid.Children[0];
                DateTime startDate = (DateTime)StartDate.SelectedDate;

                if (startTimePicker == null || startDate == null)
                {
                    DisplayErrorMessage("Time and date must be both filled out");
                    return;
                }

                DateTime tmpTime = (DateTime)startTimePicker.Value;
                TimeSpan time = new TimeSpan(tmpTime.Hour, tmpTime.Minute, tmpTime.Second);
                DateTime dateAndTime = startDate.Add(time);

                //getting all other fields of an event
                string decription = EventDescriptionBox.Text;
                Category category = (Category)updatedCategoriesCmb.SelectedItem;
                int categoryId = category == null ? 12: category.Id ;
                double duration = 0 ; 

                //getting duration in minutes
                if(EndTime.Text != "")
                {
                    if (!double.TryParse(EndTime.Text, out duration))
                    {
                        DisplayErrorMessage("Please enter a number for your duration in minutes.");
                        return;
                    }
                }
                
                presenter.UpdateEvent(eventId, dateAndTime, duration, decription, categoryId);
                

                mainWindow.Show();
                this.Close();
                
                
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show("Something went wrong while updating the event: " + ex.ToString());
            }
            


        }

        
    }
}
