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

        public void AddNewEvent(DateTime startDate, DateTime endDate, int categoryId, string description, double duration)
        {
            // Here we call the Add method of the Events class from your model
            model.events.Add(startDate, categoryId, duration, description);

            // You might want to call a method to update the UI or a list of events here as well
            view.DisplaySuccessfulMessage("Event added successfully.");
        }

        /// <summary>
        /// Gets the all the types of activity to display it. 
        /// </summary>
        public void GetCategoriesTypeInList() 
        {
            view.ShowInformationOnCmb(model.categories.List());
        }

        public void GetCalendarItemsFilteredByMonth(DateTime startMonth, DateTime endMonth)
        {
            //good

            //get start date time, duration in minutes, details, category ID (translate this number to its category description)
            //show this on datagrid
            //List<CalendarItem> calendarItemsByMonths = model.GetCalendarItems(startMonth, endMonth, false, 0);
            //calendarItemsByMonths = calendarItemsByMonths.OrderBy(i => i.StartDateTime.Month).ToList();

            //List<Dictionary<string, object>> items = itemsByMonth.ToList();

            //Dictionary<object, object> c2 = new Dictionary<object, object>();
            
            //List<CalendarItem> c1 = new List<CalendarItem>();
            //Dictionary<string, object> c2 = new Dictionary<string, object>();
            //List < Dictionary<string, object> > c3 = new List<Dictionary<string, object>>();
            //c2["Month"] = months[0]["Month"];
            //c2["TotalBusyTime"] = months[0]["TotalBusyTime"];
            //foreach (var month in months[0]) 
            //{ 

            //}


            //List<Event> eventsByMonth = new List<Event>();

            //foreach (CalendarItem item in calendarItemsByMonths)
            //{
            //    eventsByMonth.Add(new Event(item.EventID, item.StartDateTime, item.CategoryID, item.DurationInMinutes, item.ShortDescription));
            //}
            List<string> months = new List<string>();
            List<Double> totalBusyTimes = new List<Double>();
            List<Dictionary<string, object>> itemsByMonth = model.GetCalendarDictionaryByCategoryAndMonth(new DateTime(2018,01,01), new DateTime(2024, 04, 01), false, 0);
            

            for (int i = 0; i < itemsByMonth.Count - 1; i++)
            {
                foreach (var item in itemsByMonth[i])
                {
                    if (item.Key == "Month")
                    {
                        months.Add(item.Value.ToString());
                        //c2.Add([$"Month : {item.Value}"]);
                        //["Month"] = item.Value
                    }
                    else if (item.Key == "TotalBusyTime")
                    {
                        totalBusyTimes.Add((Double)item.Value); 
                        //c2[i]["TotalBusyTime"] = item.Value;
                    }
                    continue;
                }
            }
            Console.WriteLine();
            //view.ShowCalendarItemsFilteredByMonth(items);
        }


    }
}
