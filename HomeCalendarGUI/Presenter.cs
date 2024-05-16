using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;
using static System.Net.WebRequestMethods;
using System.Data.Entity.Migrations.Model;
using System.Printing;
using TeamHeavyWeight_HomeCalendarApp;
using static Calendar.Category;
using System.Collections;
using System.Data;
using System.Windows;

namespace HomeCalendarGUI
{
    public class Presenter
    {
        private readonly HomeCalendar model;
        private readonly IView view;
        

        /// <summary>
        /// Initiates presenter with default settings
        /// </summary>
        /// <param name="v">IView interface implemented class</param>
        public Presenter(IView v) 
        {
            model = new HomeCalendar();
            view = v;
        }

        /// <summary>
        /// Instantiates presenter with an existing database
        /// </summary>
        /// <param name="v">IView interface implemented class</param>
        /// <param name="dbFile">File path to the database</param>
        public Presenter(IView v,string dbFile)
        {
            view = v; 
            try
            {
                model = new HomeCalendar(dbFile, false);
            }
            catch (FileNotFoundException ex)
            {
                if (ex is FileNotFoundException)
                {
                    // Notify the view to display an error message
                    view.DisplayErrorMessage("Database file could not be loaded. Please check the database path.");
                    model = null;
                }
                else
                {
                    // Handle other unexpected exceptions
                    view.DisplayErrorMessage("An unexpected error occurred during initialization.");
                    model = null;
                }
            }
        }

        /// <summary>
        /// Gets all categories listed in the database
        /// </summary>
        public void GetCategoriesForComboBox()
        {
            List<Category> categories = model.categories.List();
            view.PopulateCategoriesComboBox(categories);
        }

        public List<Category> RetrieveCategories()
        {
            return model.categories.List().OrderBy(c => c.Description).ToList();
        }

        /// <summary>
        /// Adds a new category to the database with the details and the type the user provided.
        /// </summary>
        /// <param name="desc"> A string that holds the details about this category. </param>
        /// <param name="type"> An enum that contains the kind of activity. </param>
        public void AddNewCategory(string desc, CategoryType type)
        {
            if (desc == null || type == null)
            {
                view.DisplayErrorMessage("You can not leave any empty boxes."); 
            }
            else
            {
                model.categories.Add(desc, type);
                view.DisplaySuccessfulMessage("Category has been successfully added!");
            }
            
        }

        /// <summary>
        /// Adds new events to database
        /// </summary>
        /// <param name="startDate">Start date of event</param>        
        /// <param name="categoryId">Category Id of event</param>
        /// <param name="description">Description of event</param>
        /// <param name="duration">Duration of event</param>
        public void AddNewEvent(DateTime startDate, int categoryId, string description, double duration)
        {
            // Here we call the Add method of the Events class from your model
            model.events.Add(startDate, categoryId, duration, description);

            // You might want to call a method to update the UI or a list of events here as well
            view.DisplaySuccessfulMessage("Event added successfully.");
            view.ShowCalendarItemsOnDataGrid(model.GetCalendarItems(null, null, false, 0));
        }

        /// <summary>
        /// Gets the all the types of activity to display it. 
        /// </summary>
        public void GetCategoriesTypeInList() 
        {
            view.PopulateCategoryTypesComboBox(model.categories.List());
        }

        /// <summary>
        /// Gets all events from the database
        /// </summary>
        public void GetCalendarItems()
        {
            if (model == null)
            {
                view.DisplayErrorMessage("Unable to load calendar items because the model is not initialized.");
                return;
            }
            view.ShowCalendarItemsOnDataGrid(model.GetCalendarItems(null,null,false,0));
        }

        /// <summary>
        /// Filters events by date
        /// </summary>
        /// <param name="startDate">Start date of events</param>
        /// <param name="endDate">End date of events</param>
        public void GetEventsFilteredByDateRange(DateTime? startDate,DateTime? endDate)
        {
            List<CalendarItem> items =  model.GetCalendarItems(startDate,endDate,false,0);
            view.ShowCalendarItemsWithDateFiltersOn(items);
        }
        
        public void GetCalendarItemsFilteredByMonth(DateTime startMonth, DateTime endMonth)
        {
            if(endMonth < startMonth)
            {
                view.DisplayErrorMessage("End month must be after start month. ");
                return; 
            }

            //good
            List<string> months = new List<string>();
            List<Double> totalBusyTimes = new List<Double>();
            List<Dictionary<string, object>> itemsByMonth = model.GetCalendarDictionaryByCategoryAndMonth(startMonth, endMonth, false, 0);

            for (int i = 0; i < itemsByMonth.Count - 1; i++)
            {
                foreach (var item in itemsByMonth[i])
                {
                    if (item.Key == "Month")
                    {
                        months.Add(item.Value.ToString());
                    }
                    else if (item.Key == "TotalBusyTime")
                    {
                        totalBusyTimes.Add((Double)item.Value); 
                    }
                    continue;
                }
            }

            //making dictionary
            Dictionary<string, Double> itemsByMonthAndTime = new Dictionary<string, Double>(); 
            for(int i = 0; i < months.Count; i++)
            {
                itemsByMonthAndTime[$"{months[i]}"] = totalBusyTimes[i]; 
            }

            view.ShowCalendarItemsFilteredByMonth(itemsByMonthAndTime);
        }
        
        public void GetEventsFilteredByCategory(int categoryId)
        {
            var filteredEvents = model.GetCalendarItems(null, null, true, categoryId);
            view.ShowCalendarItemsWithCategoryFiltersOn(filteredEvents);
        }

        /// <summary>
        /// Gets CalendarItems with corresponding filters
        /// </summary>
        /// <param name="startDate">Start Date</param>
        /// <param name="endDate">End Date</param>
        /// <param name="categoryId">Category Id</param>
        /// <param name="dateFilter">If true, filters by specified date</param>
        /// <param name="summaryByCategory">If true, filters by category</param>
        /// <param name="summaryByMonth">If true,filters by month</param>        
        public void GetHomeCalendarItems(DateTime? startDate, DateTime? endDate, int categoryId, bool dateFilter ,bool summaryByCategory, bool summaryByMonth,bool filterDataByCategory)
        {
            //if dateFilter is set to true, throw if the start and end dates values are null
            // or when the end date is before the start date.
            if (dateFilter)
            {
                if(startDate == null || endDate == null)
                {
                    throw new InvalidOperationException("Must provide a start and end date");
                }
                else if(endDate < startDate)
                {
                    throw new InvalidOperationException("End date cannot be set before the starting date");
                }
            }

            //If the user wants to filter by category and month, get dictionary while considering the date filter flag
            if (summaryByCategory && summaryByMonth)
            {
                List<Dictionary<string, object>> itemsByCategoryAndMonth;
                if (dateFilter)
                {
                    itemsByCategoryAndMonth = model.GetCalendarDictionaryByCategoryAndMonth(startDate, endDate, true, categoryId);                    
                }
                else
                {
                    itemsByCategoryAndMonth = model.GetCalendarDictionaryByCategoryAndMonth(null, null, true, categoryId);
                }
                view.ShowTotalBusyTimeByMonthAndCategory(itemsByCategoryAndMonth);
            }
            else if (summaryByMonth)
            {
                //If the user wants to filter by month, get a list of calendar items by month while considering the date filter flag
                List<CalendarItemsByMonth> itemsByMonth;
                if (dateFilter)
                {
                    itemsByMonth = model.GetCalendarItemsByMonth(startDate, endDate, false, categoryId);                   
                }
                else
                {
                    itemsByMonth = model.GetCalendarItemsByMonth(null, null, false, categoryId);                    
                }
                view.ShowTotalBusyTimeByMonth(itemsByMonth);
            }
            else if (summaryByCategory)
            {
                List<CalendarItemsByCategory> itemsByCategory;
                //If the user wants to filter by category, get a list of calendar items by category while considering the date filter flag
                if (dateFilter)
                {
                    itemsByCategory = model.GetCalendarItemsByCategory(startDate, endDate, false, categoryId);                    
                }
                else
                {
                    itemsByCategory = model.GetCalendarItemsByCategory(null, null, false, categoryId);                   
                }
                view.ShowTotalBusyTimeByCategory(itemsByCategory);
            }
            else
            {
                //If the user doesn't apply filters, get a list of calendar items while considering the date filter flag

                List<CalendarItem> items;
                if (dateFilter)
                {
                    items = model.GetCalendarItems(startDate, endDate, filterDataByCategory, categoryId);
                }
                else
                {
                    items = model.GetCalendarItems(null, null, filterDataByCategory, categoryId);
                }                
                
                view.ShowCalendarItems(items);
                
                //view.ShowCalendarItemsWithDateFiltersOn(items);
            }
        }

        /// <summary>
        /// Deletes an event from the database
        /// </summary>
        public void DeleteAnEvent(int eventId)
        {
            model.events.DeleteEvent(eventId);
            view.ShowCalendarItemsOnDataGrid(model.GetCalendarItems(null, null, false, 0));
        }
    }
}
