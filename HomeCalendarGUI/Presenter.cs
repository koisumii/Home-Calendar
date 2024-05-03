using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;
using static System.Net.WebRequestMethods;
﻿using Calendar;
using System.Data.Entity.Migrations.Model;
using System.Printing;
using TeamHeavyWeight_HomeCalendarApp;
using static Calendar.Category;
using System.Collections;
using System.Data;

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

            //testing 
            DateTime start = new DateTime(2018, 01, 01);
            GetCalendarItemsFilteredByMonth(start, start.AddMonths(8)); 
        }

        /// <summary>
        /// Instantiates presenter with an existing database
        /// </summary>
        /// <param name="v">IView interface implemented class</param>
        /// <param name="dbFile">File path to the database</param>
        public Presenter(IView v,string dbFile)
        {
            model = new HomeCalendar(dbFile,false);
            view = v;
            //testing 
            DateTime start = new DateTime(2018, 01, 01);
            GetCalendarItemsFilteredByMonth(start, start.AddMonths(8));
        }

        /// <summary>
        /// Gets all categories listed in the database
        /// </summary>
        public void GetCategoriesForComboBox()
        {
            List<Category> categories = model.categories.List();
            view.ShowCategoriesOnComboBox(categories);
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
        /// <param name="endDate">End date of event</param>
        /// <param name="categoryId">Category Id of event</param>
        /// <param name="description">Description of event</param>
        /// <param name="duration">Duration of event</param>
        public void AddNewEvent(DateTime startDate, DateTime endDate, int categoryId, string description, double duration)
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
            view.ShowInformationOnCmb(model.categories.List());
        }

        /// <summary>
        /// Gets all events from the database
        /// </summary>
        public void GetCalendarItems()
        {
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
