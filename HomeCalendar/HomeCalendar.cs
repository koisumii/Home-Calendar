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
        public Categories categories { get { return _categories; } }
        public Events events { get { return _events; } }

        // -------------------------------------------------------------------
        // Constructor (new... default categories, no events)
        // -------------------------------------------------------------------
        public HomeCalendar()
        {
            //_categories = new Categories();
            //_events = new Events();
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
        //public HomeCalendar(string databaseFile, string eventsXMLFile, bool newDB = false)
        //{
        //    if (!newDB && File.Exists(databaseFile))
        //    {
        //        Database.existingDatabase(databaseFile);
        //    }
        //    else
        //    {
        //        Database.newDatabase(databaseFile);
        //        newDB = true;
        //    }
        //    _categories = new Categories(Database._dbConnection, newDB);
        //    _events = new Events();
        //    _events.ReadFromFile(eventsXMLFile);
        //}

        #region OpenNewAndSave
        // ---------------------------------------------------------------
        // Read
        // Throws Exception if any problem reading this file
        // ---------------------------------------------------------------
        /*public void ReadFromFile(String? calendarFileName)
        {
            // ---------------------------------------------------------------
            // read the calendar file and process
            // ---------------------------------------------------------------
            try
            {
                // get filepath name (throws exception if it doesn't exist)
                calendarFileName = CalendarFiles.VerifyReadFromFileName(calendarFileName, "");

                // If file exists, read it
                string[] filenames = System.IO.File.ReadAllLines(calendarFileName);

                // ----------------------------------------------------------------
                // Save information about the calendar file
                // ----------------------------------------------------------------
                string? folder = Path.GetDirectoryName(calendarFileName);
                _FileName = Path.GetFileName(calendarFileName);

                // read the events and categories from their respective files
                _categories.ReadFromFile(folder + "\\" + filenames[0]);
                _events.ReadFromFile(folder + "\\" + filenames[1]);

                // Save information about calendar file
                _DirName = Path.GetDirectoryName(calendarFileName);
                _FileName = Path.GetFileName(calendarFileName);

            }

            // ----------------------------------------------------------------
            // throw new exception if we cannot get the info that we need
            // ----------------------------------------------------------------
            catch (Exception e)
            {
                throw new Exception("Could not read calendar info: \n" + e.Message);
            }

        }*/

        // ====================================================================
        // save to a file
        // saves the following files:
        //  filepath_events.evts  # events file
        //  filepath_categories.cats # categories files
        //  filepath # a file containing the names of the events and categories files.
        //  Throws exception if we cannot write to that file (ex: invalid dir, wrong permissions)
        // ====================================================================
        //public void SaveToFile(String filepath)
        //{

        //    ---------------------------------------------------------------
        //    just in case filepath doesn't exist, reset path info
        //    -------------------------------------------------------------- -
        //   _DirName = null;
        //        _FileName = null;

        //        ---------------------------------------------------------------
        //        get filepath name(throws exception if we can't write to the file)
        //        -------------------------------------------------------------- -
        //       filepath = CalendarFiles.VerifyWriteToFileName(filepath, "");

        //        String? path = Path.GetDirectoryName(Path.GetFullPath(filepath));
        //        String file = Path.GetFileNameWithoutExtension(filepath);
        //        String ext = Path.GetExtension(filepath);

        //        ---------------------------------------------------------------
        //        construct file names for events and categories
        //        -------------------------------------------------------------- -
        //       String eventpath = path + "\\" + file + "_events" + ".evts";
        //        String categorypath = path + "\\" + file + "_categories" + ".cats";

        //        ---------------------------------------------------------------
        //        save the events and categories into their own files
        //     ---------------------------------------------------------------
        //    _events.SaveToFile(eventpath);
        //        _categories.SaveToFile(categorypath);

        //        ---------------------------------------------------------------
        //        save filenames of events and categories to calendar file
        //     ---------------------------------------------------------------
        //    string[] files = { Path.GetFileName(categorypath), Path.GetFileName(eventpath) };
        //        System.IO.File.WriteAllLines(filepath, files);

        //        ----------------------------------------------------------------
        //        save filename info for later use

        //        ----------------------------------------------------------------
        //       _DirName = path;
        //       _FileName = Path.GetFileName(filepath);
        //    }
        #endregion OpenNewAndSave

        #region GetList



        // ============================================================================
        // Get all events list
        // ============================================================================
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
