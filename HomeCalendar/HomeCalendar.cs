using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================


namespace Calendar
{
    // ====================================================================
    // CLASS: HomeCalendar
    //        - Combines a Categories Class and an Events Class
    //        - One File defines Category and Events File
    //        - etc
    // ====================================================================

    /// <summary>
    /// This class will retrieve <see cref="Events">events</see> and <see cref="Categories">categories</see> from a database.
    /// </summary>
    public class HomeCalendar
    {
        private SQLiteConnection _dbConnection;
        private Categories _categories;
        private Events _events;

        // ====================================================================
        // Properties
        // ===================================================================
        public SQLiteConnection DbConnection { get { return _dbConnection; } }

        // Properties (categories and events object)
        /// <summary>
        /// Gets multiple groups of different activities.
        /// </summary>
        public Categories categories { get { return _categories; } }

        /// <summary>
        /// Gets the activities. 
        /// </summary>
        public Events events { get { return _events; } }

        // -------------------------------------------------------------------
        // Constructor (new... default categories, no events)
        // -------------------------------------------------------------------
        public HomeCalendar()
        {
            
        }

        // -------------------------------------------------------------------
        // // Constructor (existing calendar ... must specify database)
        // -------------------------------------------------------------------
        public HomeCalendar(string databaseFile, bool newDB)
        {
            if (newDB)
            {
                Database.newDatabase(databaseFile);
            }
            else
            {
                Database.existingDatabase(databaseFile);
            }

            this._dbConnection = Database.dbConnection;
            _categories = new Categories(this._dbConnection, newDB);
            _events = new Events(this._dbConnection);
        }

        #region OpenNewAndSave
        
        #endregion OpenNewAndSave

        #region GetList



        // ============================================================================
        // Get all events list
        // ============================================================================
        /// <summary>
        /// Gets all the activities that have been marked in the calendar between a beginning date and a stop date and puts them inside of a list.
        /// </summary>
        /// <param name="Start"> A DateTime object that represents the oldest date marked in the calendar. </param>
        /// <param name="End">A DateTime object that represents the most recent date marked in the calendar. </param>
        /// <param name="FilterFlag"> A boolean value that only gets activities for the specified category ID </param>
        /// <param name="CategoryID"> An integer that represents the unique number that identifies a category. </param>
        /// <returns> A list with all activities in the calendar. </returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// DateTime start1 = new DateTime(2020,01,01); 
        /// HomeCalendar h1 = new HomeCalendar("./test.calendar");
        /// List<CalendarItem> calendarItems1 = h1.GetCalendarItems(start1, DateTime.Now, false, 5);
        /// for(int i = 0; i<calendarItems1.Count; i++)
        /// {
        ///     Console.WriteLine($"Item {i+1}");
        ///     Console.Write($"BusyTime: {calendarItems1[i].BusyTime} min\n" +
        ///     $"Category: {calendarItems1[i].Category} \n" +
        ///     $"CategoryID: {calendarItems1[i].CategoryID} \n" +
        ///     $"DurationInMinutes: {calendarItems1[i].DurationInMinutes} min\n" +
        ///     $"ShortDescription: {calendarItems1[i].ShortDescription} \n" +
        ///     $"StartDateTime: {calendarItems1[i].StartDateTime.Date}");
        ///     Console.WriteLine("\n");
        /// }
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Item 1
        /// BusyTime: 1440 min
        /// Category: Canadian Holidays
        /// CategoryID: 8
        /// DurationInMinutes: 1440 min
        /// ShortDescription:
        /// StartDateTime: 1/1/2020 12:00:00 AM
        /// 
        /// Item 2
        /// BusyTime: 2880 min
        /// Category: Vacation
        /// CategoryID: 9
        /// DurationInMinutes: 1440 min
        /// ShortDescription:
        /// StartDateTime: 1/9/2020 12:00:00 AM
        ///
        /// Item 3
        ///  BusyTime: 4320 min
        /// Category: Vacation
        /// CategoryID: 9
        /// DurationInMinutes: 1440 min
        /// ShortDescription:
        /// StartDateTime: 1/10/2020 12:00:00 AM
        ///
        /// Item 4
        /// BusyTime: 5760 min
        /// Category: Birthdays
        /// CategoryID: 11
        /// DurationInMinutes: 1440 min
        /// ShortDescription:
        /// StartDateTime: 1/12/2020 12:00:00 AM
        ///
        /// Item 5
        /// BusyTime: 5940 min
        /// Category: On call
        /// CategoryID: 7
        /// DurationInMinutes: 180 min
        /// ShortDescription:
        /// StartDateTime: 1/20/2020 12:00:00 AM
        /// </code>
        /// 
        /// Sample code if the filter flag is true: 
        /// <code>
        /// <![CDATA[
        /// DateTime start1 = new DateTime(2020,01,01); 
        /// HomeCalendar h1 = new HomeCalendar("./test.calendar");
        /// List<CalendarItem> calendarItems1 = h1.GetCalendarItems(start1, DateTime.Now, true, 7);
        /// ]]>
        /// </code>
        ///
        /// Sample output if the filter flag is true:
        /// 
        /// <code>
        /// Item 1
        /// BusyTime: 180 min
        /// Category: On call
        /// CategoryID: 7
        /// DurationInMinutes: 180 min
        /// ShortDescription:
        /// StartDateTime: 1/20/2020 12:00:00 AM
        /// </code>
        /// </example>
        public List<CalendarItem> GetCalendarItems(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // ------------------------------------------------------------------------
            // return joined list within time frame
            // ------------------------------------------------------------------------

            //date parsing whole deal
            Start = Start ?? new DateTime(1900, 1, 1);

            DateTime tmpStart = Start ?? DateTime.Now;
            string y = tmpStart.ToString();
            //getting rid of AM 
            y.Remove(y.Length - 3, 3);
            DateTime Startdate = DateTime.Parse(y);

            End = End ?? new DateTime(2020, 12, 31);

            DateTime tmpEnd = End ?? DateTime.Now;
            string x = tmpEnd.ToString();
            //getting rid of AM 
            x.Remove(x.Length - 3, 3);
            DateTime EndDate = DateTime.Parse(x);
            
            string q = "SELECT c.Id, c.Description, c.TypeId FROM categories c JOIN events ON c.Id == events.CategoryId WHERE events.StartDateTime >= @Start AND events.StartDateTime <= @End ORDER BY events.StartDateTime";
            SQLiteCommand cmd = new SQLiteCommand(q, this._dbConnection);
            //NEED YYYY/MM/DD BUT GETTING YYYY-MM-DD
            cmd.Parameters.AddWithValue("@Start", Startdate.ToString("yyyy/MM/dd hh:mm tt"));
            cmd.Parameters.AddWithValue("@End", EndDate.ToString("yyyy/MM/dd hh:mm tt"));
            cmd.Prepare();

            List<CalendarItem> items = new List<CalendarItem>();
            using SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) 
            {
                int id = reader.GetInt32(0);
                String description = reader.GetString(1);
                int typeId = reader.GetInt32(2);

                items.Add(new CalendarItem
                {
                    CategoryID = id,
                    ShortDescription = description,
                    StartDateTime = Startdate,
                });
                            
            }

            // do the query
            // loop over the read results
            //      add result to items

            var query = from c in _categories.List()
                        join e in _events.List() on c.Id equals e.Category
                        where e.StartDateTime >= Start && e.StartDateTime <= End
                        orderby e.StartDateTime
                        select new { CatId = c.Id, EventId = e.Id, e.StartDateTime, Category = c.Description, e.Details, e.DurationInMinutes };

            // ------------------------------------------------------------------------
            // create a CalendarItem list with totals,
            // ------------------------------------------------------------------------

            Double totalBusyTime = 0;

            foreach (var e in query.OrderBy(q => q.StartDateTime))
            {
                // filter out unwanted categories if filter flag is on
                if (FilterFlag && CategoryID != e.CatId)
                {
                    continue;
                }

                // keep track of running totals
                totalBusyTime = totalBusyTime + e.DurationInMinutes;
                items.Add(new CalendarItem
                {
                    CategoryID = e.CatId,
                    EventID = e.EventId,
                    ShortDescription = e.Details,
                    StartDateTime = e.StartDateTime,
                    DurationInMinutes = e.DurationInMinutes,
                    Category = e.Category,
                    BusyTime = totalBusyTime
                });
            }

            return items;
        }

        // ============================================================================
        // Group all events month by month (sorted by year/month)
        // returns a list of CalendarItemsByMonth which is 
        // "year/month", list of calendar items, and totalBusyTime for that month
        // ============================================================================
        /// <summary>
        /// Gets all the activities that were marked in your calendar for a specific month.
        /// </summary>
        /// <param name="Start"> A DateTime object that represents the oldest date marked in the calendar. </param>
        /// <param name="End"> A DateTime object that represents the most recent date marked in the calendar. </param>
        /// <param name="FilterFlag"> A boolean value that only gets activities for the specified category ID </param>
        /// <param name="CategoryID"> An integer that represents the unique number that identifies a category. </param>
        /// <returns> A list with all activities for a specific month in the calendar. </returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// DateTime start1 = new DateTime(2020,01,01); 
        /// HomeCalendar h1 = new HomeCalendar("./test.calendar");
        /// List<CalendarItemsByMonth> itemsByMonths = h1.GetCalendarItemsByMonth(start1, DateTime.Now, false, 7);
        ///
        /// for(int i = 0; i<itemsByMonths.Count; i ++)
        /// {
        ///     Console.WriteLine($"Items for the month of: {itemsByMonths[i].Month}. You were busy for a total of {itemsByMonths[i].TotalBusyTime} minute(s) this month.");
        ///     Console.WriteLine("Your activities for this month: ");
        ///     for(int j = 0; j<itemsByMonths[i].Items.Count; j++)
        ///     {
        ///         Console.WriteLine($"Item {j+1}");
        ///         Console.WriteLine($"Category: {itemsByMonths[i].Items[j].Category}\n" +
        ///                           $"Duration (in minutes): {itemsByMonths[i].Items[j].DurationInMinutes}\n" +
        ///                           $"Start date: {itemsByMonths[i].Items[j].StartDateTime}\n");
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// Sample output:
        /// <code>
        /// Items for the month of: 2020/01. You were busy for a total of 5940 minute(s) this year.
        /// Your activities for this month:
        /// Item 1
        /// Category: Canadian Holidays
        /// Duration(in minutes) : 1440
        /// Start date: 1/1/2020 12:00:00 AM
        ///
        /// Item 2
        /// Category: Vacation
        /// Duration(in minutes): 1440
        /// Start date: 1/9/2020 12:00:00 AM
        ///
        /// Item 3
        /// Category: Vacation
        /// Duration(in minutes): 1440
        /// Start date: 1/10/2020 12:00:00 AM
        ///
        /// Item 4
        /// Category: Birthdays
        /// Duration(in minutes): 1440
        /// Start date: 1/12/2020 12:00:00 AM
        ///
        /// Item 5
        /// Category: On call
        /// Duration(in minutes) : 180
        /// Start date: 1/20/2020 11:00:00 AM
        /// </code>
        /// 
        /// Sample code when the filter flag is true:
        /// <code>
        /// <![CDATA[
        /// DateTime start1 = new DateTime(2020,01,01); 
        /// HomeCalendar h1 = new HomeCalendar("./test.calendar");
        /// List<CalendarItemsByMonth> itemsByMonths = h1.GetCalendarItemsByMonth(start1, DateTime.Now, true, 7);
        /// ]]>
        /// </code>
        /// Sample output when the filter flag is true:
        /// <code>
        /// Items for the month of: 2020/01. You were busy for a total of 180 minute(s) this year.
        /// Your activities for this month:
        /// 
        /// Item 1
        /// Category: On call
        /// Duration(in minutes) : 180
        /// Start date: 1/20/2020 11:00:00 AM
        /// </code>
        /// </example>
        public List<CalendarItemsByMonth> GetCalendarItemsByMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items first
            // -----------------------------------------------------------------------
            List<CalendarItem> items = GetCalendarItems(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // Group by year/month
            // -----------------------------------------------------------------------
            var GroupedByMonth = items.GroupBy(c => c.StartDateTime.Year.ToString("D4") + "/" + c.StartDateTime.Month.ToString("D2"));

            // -----------------------------------------------------------------------
            // create new list
            // -----------------------------------------------------------------------
            var summary = new List<CalendarItemsByMonth>();
            foreach (var MonthGroup in GroupedByMonth)
            {
                // calculate totalBusyTime for this month, and create list of items
                double total = 0;
                var itemsList = new List<CalendarItem>();
                foreach (var item in MonthGroup)
                {
                    total = total + item.DurationInMinutes;
                    itemsList.Add(item);
                }

                // Add new CalendarItemsByMonth to our list
                summary.Add(new CalendarItemsByMonth
                {
                    Month = MonthGroup.Key,
                    Items = itemsList,
                    TotalBusyTime = total
                });
            }

            return summary;
        }

        // ============================================================================
        // Group all events by category (ordered by category name)
        // ============================================================================
        /// <summary>
        /// Gets all the activities that were marked in your calendar for a specific category.
        /// </summary>
        /// <param name="Start"> A DateTime object that represents the oldest date marked in the calendar. </param>
        /// <param name="End"> A DateTime object that represents the most recent date marked in the calendar. </param>
        /// <param name="FilterFlag"> A boolean value that only gets activities for the specified category ID </param>
        /// <param name="CategoryID"> An integer that represents the unique number that identifies a category. </param>
        /// <returns> A list with all activities for a specific category in the calendar. </returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// DateTime start1 = new DateTime(2020,01,01); 
        /// HomeCalendar h1 = new HomeCalendar("./test.calendar");
        /// List<CalendarItemsByCategory> itemsByCategories = h1.GetCalendarItemsByCategory(start1, DateTime.Now, false, 7);
        ///
        /// for(int i = 0; i<itemsByCategories.Count; i++)
        /// {
        ///     Console.WriteLine($"The category is: {itemsByCategories[i].Category}");
        ///     for(int j = 0; j<itemsByCategories[i].Items.Count; j++)
        ///     {
        ///         Console.WriteLine($"Busy time for this category (in minutes): {itemsByCategories[i].Items[j].BusyTime}\n" +
        ///                           $"Start date: {itemsByCategories[i].Items[j].StartDateTime}\n" +
        ///                           $"Category ID: {itemsByCategories[i].Items[j].CategoryID}\n");
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// Sample output:
        /// <code>
        /// The category is: Birthdays
        /// Busy time for this category(in minutes) : 5760
        /// Start date: 1/12/2020 12:00:00 AM
        /// Category ID: 11
        ///
        /// The category is: Canadian Holidays
        /// Busy time for this category(in minutes) : 1440
        /// Start date: 1/1/2020 12:00:00 AM
        /// Category ID: 8
        ///
        /// The category is: On call
        /// Busy time for this category(in minutes) : 5940
        /// Start date: 1/20/2020 11:00:00 AM
        /// Category ID: 7
        ///
        /// The category is: Vacation
        /// Busy time for this category(in minutes) : 2880
        /// Start date: 1/9/2020 12:00:00 AM
        /// Category ID: 9
        ///
        /// Busy time for this category(in minutes) : 4320
        /// Start date: 1/10/2020 12:00:00 AM
        /// Category ID: 9
        /// </code>
        /// 
        /// Sample code when the filter flag is on:
        /// <code>
        /// <![CDATA[
        /// DateTime start1 = new DateTime(2020,01,01); 
        /// HomeCalendar h1 = new HomeCalendar("./test.calendar");
        /// List<CalendarItemsByCategory> itemsByCategories = h1.GetCalendarItemsByCategory(start1, DateTime.Now, true, 7);
        /// ]]>
        /// </code>
        /// 
        /// Sample output when the filter flag is on:
        /// <code>
        /// The category is: On call
        /// Busy time for this category(in minutes) : 180
        /// Start date: 1/20/2020 11:00:00 AM
        /// Category ID: 7
        /// </code>
        /// </example>
        public List<CalendarItemsByCategory> GetCalendarItemsByCategory(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items first
            // -----------------------------------------------------------------------
            List<CalendarItem> filteredItems = GetCalendarItems(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // Group by Category
            // -----------------------------------------------------------------------
            var GroupedByCategory = filteredItems.GroupBy(c => c.Category);

            // -----------------------------------------------------------------------
            // create new list
            // -----------------------------------------------------------------------
            var summary = new List<CalendarItemsByCategory>();
            foreach (var CategoryGroup in GroupedByCategory.OrderBy(g => g.Key))
            {
                // calculate totalBusyTime for this category, and create list of items
                double total = 0;
                var items = new List<CalendarItem>();
                foreach (var item in CategoryGroup)
                {
                    total = total + item.DurationInMinutes;
                    items.Add(item);
                }

                // Add new CalendarItemsByCategory to our list
                summary.Add(new CalendarItemsByCategory
                {
                    Category = CategoryGroup.Key,
                    Items = items,
                    TotalBusyTime = total
                });
            }

            return summary;
        }



        // ============================================================================
        // Group all events by category and Month
        // creates a list of Dictionary objects with:
        //          one dictionary object per month,
        //          and one dictionary object for the category total busy times
        // 
        // Each per month dictionary object has the following key value pairs:
        //           "Month", <name of month>
        //           "TotalBusyTime", <the total durations for the month>
        //             for each category for which there is an event in the month:
        //             "items:category", a List<CalendarItem>
        //             "category", the total busy time for that category for this month
        // The one dictionary for the category total busy times has the following key value pairs:
        //             for each category for which there is an event in ANY month:
        //             "category", the total busy time for that category for all the months
        // ============================================================================
        /// <summary>
        /// Gets all activities for each months of the year and places them in a dictionary for each month.
        /// </summary>
        /// <param name="Start"> A DateTime object that represents the oldest date marked in the calendar. </param>
        /// <param name="End"> A DateTime object that represents the most recent date marked in the calendar. </param>
        /// <param name="FilterFlag"> A boolean value that only gets activities for the specified category ID </param>
        /// <param name="CategoryID"> An integer that represents the unique number that identifies a category. </param>
        /// <returns> A dictionary with all the activities marked in the calendar. </returns>
        /// <example>
        /// Sample code:
        /// <code>
        /// <![CDATA[
        /// DateTime start1 = new DateTime(2020, 01, 01);
        /// HomeCalendar h1 = new HomeCalendar("./test.calendar");
        /// List<Dictionary<string, object>> d1 = h1.GetCalendarDictionaryByCategoryAndMonth(start1, DateTime.Now, false, 7);
        /// int counter = 1;
        ///
        /// for(int i = 0; i<d1.Count; i++)
        /// {
        ///     foreach(string key in d1[i].Keys)
        ///     {
        ///         if (d1[i][key] is double )
        ///         {
        ///             Console.WriteLine($"Item #{counter}\n" +
        ///                               $"Category: {key}\n" +
        ///                               $"Value: {d1[i][key]} minutes\n");
        ///         }
        ///         else
        ///         {
        ///             Console.WriteLine($"Item #{counter}\n" +
        ///                               $"Category: {key}\n" +
        ///                               $"Value: {d1[i][key]}\n");
        ///         }
        ///
        ///         counter++;
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// Item #1
        /// Category: Month
        /// Value: 2020/01
        ///
        /// Item #2
        /// Category: TotalBusyTime
        /// Value: 5940 minutes
        ///
        /// Item #3
        /// Category: items:Birthdays
        /// Value: 1
        ///
        /// Item #4
        /// Category: Birthdays
        /// Value: 1440 minutes
        ///
        /// Item #5
        /// Category: items:Canadian Holidays
        /// Value: 1
        ///
        /// Item #6
        /// Category: Canadian Holidays
        /// Value: 1440 minutes
        ///
        /// Item #7
        /// Category: items:On call
        /// Value: 1
        ///
        /// Item #8
        /// Category: On call
        /// Value: 180 minutes
        ///
        /// Item #9
        /// Category: items:Vacation
        /// Value: 1
        ///
        /// Item #10
        /// Category: Vacation
        /// Value: 2880 minutes
        ///
        /// Item #11
        /// Category: Month
        /// Value: TOTALS
        ///
        /// Item #12
        /// Category: On call
        /// Value: 180 minutes
        ///
        /// Item #13
        /// Category: Canadian Holidays
        /// Value: 1440 minutes
        ///
        /// Item #14
        /// Category: Vacation
        /// Value: 2880 minutes
        ///
        /// Item #15
        /// Category: Birthdays
        /// Value: 1440 minutes
        /// </code>
        /// </example>
        public List<Dictionary<string, object>> GetCalendarDictionaryByCategoryAndMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items by month 
            // -----------------------------------------------------------------------
            List<CalendarItemsByMonth> GroupedByMonth = GetCalendarItemsByMonth(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // loop over each month
            // -----------------------------------------------------------------------
            var summary = new List<Dictionary<string, object>>();
            var totalBusyTimePerCategory = new Dictionary<String, Double>();

            foreach (var MonthGroup in GroupedByMonth)
            {
                // create record object for this month
                Dictionary<string, object> record = new Dictionary<string, object>();
                record["Month"] = MonthGroup.Month;
                record["TotalBusyTime"] = MonthGroup.TotalBusyTime;

                // break up the month items into categories
                var GroupedByCategory = MonthGroup.Items.GroupBy(c => c.Category);

                // -----------------------------------------------------------------------
                // loop over each category
                // -----------------------------------------------------------------------
                foreach (var CategoryGroup in GroupedByCategory.OrderBy(g => g.Key))
                {

                    // calculate totals for the cat/month, and create list of items
                    double totalCategoryBusyTimeForThisMonth = 0;
                    var details = new List<CalendarItem>();

                    foreach (var item in CategoryGroup)
                    {
                        totalCategoryBusyTimeForThisMonth = totalCategoryBusyTimeForThisMonth + item.DurationInMinutes;
                        details.Add(item);
                    }

                    // add new properties and values to our record object
                    record["items:" + CategoryGroup.Key] = details;
                    record[CategoryGroup.Key] = totalCategoryBusyTimeForThisMonth;

                    // keep track of totals for each category
                    if (totalBusyTimePerCategory.TryGetValue(CategoryGroup.Key, out Double currentTotalBusyTimeForCategory))
                    {
                        totalBusyTimePerCategory[CategoryGroup.Key] = currentTotalBusyTimeForCategory + totalCategoryBusyTimeForThisMonth;
                    }
                    else
                    {
                        totalBusyTimePerCategory[CategoryGroup.Key] = totalCategoryBusyTimeForThisMonth;
                    }
                }

                // add record to collection
                summary.Add(record);
            }
            // ---------------------------------------------------------------------------
            // add final record which is the totals for each category
            // ---------------------------------------------------------------------------
            Dictionary<string, object> totalsRecord = new Dictionary<string, object>();
            totalsRecord["Month"] = "TOTALS";

            foreach (var cat in categories.List())
            {
                try
                {
                    totalsRecord.Add(cat.Description, totalBusyTimePerCategory[cat.Description]);
                }
                catch { }
            }
            summary.Add(totalsRecord);


            return summary;
        }




        #endregion GetList

    }
}
