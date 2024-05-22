using Calendar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private MainWindow main;
        private OpenFileWindowOnInitialization openFileWindow; 

        public UpdateEventsWindow()
        {
            presenter = new Presenter(this);
            InitializeComponent();
            
            presenter.GetCategoriesForAllCatsComboBoxes();
            CreateTimePicker();
            StartDate.DisplayDateStart = DateTime.Now;
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

            //TimePicker EndTimePicker = new TimePicker();
            //EndTimePicker.AllowTextInput = false;
            //EndTimePicker.Name = "endTime";
            //EndTimePicker.Margin = new Thickness(0, 5, 0, 0);

            //EndTimeGrid.Children.Add(EndTimePicker);

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
            catsComboBox.Items.Clear();

            // Sort categories alphabetically by their Description
            var sortedCategories = categories.OrderBy(c => c.Description).ToList();

            const int DEFAULT = 0;
            sortedCategories.ForEach(c => {
                catsComboBox.Items.Add(c);
            });
            catsComboBox.SelectedIndex = DEFAULT;
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

        #endregion

        private void Btn_UpdateEvent(object sender, RoutedEventArgs e)
        {

            //duration of the event
            //casting to TimePicker objects because when retrieving them, they are not.
            try
            {
                //this is a must
                var eventIDtemp = (TextBlock)UpdateEventGrid.Children[1];
                int eventId = int.Parse(eventIDtemp.Text);

                var startTimePicker = (TimePicker)StartTimeGrid.Children[0];
                //var endTimePicker = (TimePicker)EndTimeGrid.Children[0];

                //getting all other fields of an event
                string decription = EventDescriptionBox.Text;
                DateTime? startDate = StartDate.SelectedDate;
                Category category = (Category)catsComboBox.SelectedItem;

                //getting duration in minutes
                if (!double.TryParse(endTime.Text, out double endTimeInMinutes))
                {
                    DisplayErrorMessage("Please enter a number for your duration in minutes.");
                    return;
                }

                //if (startTimePicker.Value != null && endTimePicker.Value != null)
                //{
                //    DateTime? start = startTimePicker.Value;
                //    DateTime? end = endTimePicker.Value;

                //    TimeSpan duration = end.Value - start.Value;
                //    durationInMinutes = duration.TotalMinutes;

                //}
                //else
                //{
                //    DisplayErrorMessage("End time and start time must be filled out.");
                //    return;
                //}

                presenter.UpdateEvent(eventId, startDate, endTimeInMinutes, decription, category.Id);
                openFileWindow = new OpenFileWindowOnInitialization();
                openFileWindow.Show();
                //main = new MainWindow(false);
                //main.Show();
                //DisplaySuccessfulMessage("Event has been updated successfully!");
                
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show("Something went wrong while updating the event: " + ex.ToString());
            }
            


        }
    }
}
