//using System.Data;
//using System.Data.SQLite;
//using System.IO;

//// ============================================================================
//// (c) Sandy Bultena 2018
//// * Released under the GNU General Public License
//// ============================================================================


//namespace Calendar
//{
//    // ====================================================================
//    // CLASS: HomeCalendar
//    //        - Combines a Categories Class and an Events Class
//    //        - One File defines Category and Events File
//    //        - etc
//    // ====================================================================

//    /// <summary>
//    /// Represents a class containing fuctionalities for a Calendar App. 
//    /// </summary>
//    public class HomeCalendar
//    {
//        private string? _FileName;
//        private string? _DirName;
//        private Categories _categories;
//        private Events _events;
//        private bool _filter;

//        // ====================================================================
//        // Properties
//        // ===================================================================

//        // Properties (location of files etc)
//        /// <summary>
//        /// Name of the calendar file
//        /// </summary>
//        public String? FileName { get { return _FileName; } }
//        /// <summary>
//        /// Name of the calendar directory
//        /// </summary>
//        public String? DirName { get { return _DirName; } }
//        /// <summary>
//        /// Name of the file path
//        /// </summary>
//        public String? PathName
//        {
//            get
//            {
//                if (_FileName != null && _DirName != null)
//                {
//                    return Path.GetFullPath(_DirName + "\\" + _FileName);
//                }
//                else
//                {
//                    return null;
//                }
//            }
//        }

//        // Properties (categories and events object)
//        /// <summary>
//        /// Gets an instance of categories.
//        /// </summary>
//        public Categories categories { get { return _categories; } }
//        /// <summary>
//        /// Gets an instance of events
//        /// </summary>
//        public Events events { get { return _events; } }

//        // -------------------------------------------------------------------
//        // Constructor (new... default categories, no events)
//        // -------------------------------------------------------------------
//        /// <summary>
//        /// Default instance of Home Calendar class without specifying database.        
//        /// </summary>
//        public HomeCalendar()
//        {
//            _categories = new Categories(Database.dbConnection, true);
//            _events = new Events();
//        }


//        // -------------------------------------------------------------------
//        // Constructor (existing calendar ... must specify file)
//        // -------------------------------------------------------------------
//        /// <summary>
//        /// An instance of Home Calendar class with database specification.
//        /// </summary>
//        /// <param name="dbFile">Database to connect to</param>
//        /// <param name="dbIsNew">Create a new database</param>
//        /// <exception cref="FileNotFoundException"></exception>
//        public HomeCalendar(string dbFile, bool dbIsNew)
//        {
//            if (dbIsNew)
//            {
//                if (dbFile == null || dbFile == "")
//                {
//                    throw new FileNotFoundException("Provided file path to the database is null");
//                }
//                Database.newDatabase(dbFile);
//            }
//            else
//            {
//                if (dbFile == null || dbFile == "")
//                {
//                    throw new FileNotFoundException("Provided file path to the database is null");
//                }
//                else if (System.IO.File.Exists(dbFile))
//                {
//                    Database.existingDatabase(dbFile);
//                }
//                else
//                {
//                    throw new FileNotFoundException("Provided database file does not exist.");
//                }
//            }

//            _categories = new Categories(Database.dbConnection, false);
//            _events = new Events();
//        }

//        #region GetList
//        // ============================================================================
//        // Get all events list
//        // ============================================================================
//        /// <summary>
//        /// Retrieves a list of all calendar items. 
//        /// </summary>
//        /// <param name="Start">Start date</param>
//        /// <param name="End">End date</param>
//        /// <param name="FilterFlag">Filters by request category</param>
//        /// <param name="CategoryID">Selected CategoryId</param>
//        /// <returns>A list of CalendarItem with the queried results</returns>
//        /// <example> 
//        /// Assuming that we get the following output:
//        /// <code>
//        /// - Category ID: 9
//        /// - Event ID: 1
//        /// - Start Date Time: 2018-01-10 10:00:00 AM
//        /// - Category: Fun
//        /// - Short Description:
//        /// - DurationInMinutes: 40
//        /// - BusyTime: 40 
//        ///  
//        /// - Category ID: 2
//        /// - Event ID: 8
//        /// - Start Date Time: 1/11/2018 10:15:00 AM
//        /// - Category: Work
//        /// - Short Description:
//        /// - DurationInMinutes: 60
//        /// - BusyTime: 100
//        /// </code>
//        /// 
//        /// <b>Gets a list of calendar items and outputs to the console</b> 
//        ///<code>
//        /// <![CDATA[     
//        /// List<CalendarItem> calendarItems = calendar.GetCalendarItems(Start, End, FilterFlag, CategoryID);
//        /// 
//        ///    Console.WriteLine("Calendar Items");
//        ///    calendarItems.ForEach(calItms =>
//        ///    {
//        ///        Console.WriteLine(" - Category ID: {0}", calItms.CategoryID);
//        ///        Console.WriteLine(" - Event ID: {0}", calItms.EventID);
//        ///        Console.WriteLine(" - Start Date Time: {0}", calItms.StartDateTime);
//        ///        Console.WriteLine(" - Category: {0}", calItms.Category);
//        ///        Console.WriteLine(" - Short Description: {0}", calItms.ShortDescription);
//        ///        Console.WriteLine(" - DurationInMinutes: {0}", calItms.DurationInMinutes);
//        ///        Console.WriteLine(" - BusyTime: {0}", calItms.BusyTime);
//        ///        Console.WriteLine("\n");
//        ///    });
//        /// ]]>
//        /// </code> 
//        /// 
//        /// <b>Output when filter flag is on (assuming that category id is 9)</b>
//        /// 
//        /// <code>
//        /// - Category ID: 9
//        /// - Event ID: 2
//        /// - Start Date Time: 1/9/2020 12:00:00 AM
//        /// - Category: Vacation
//        /// - Short Description:
//        /// - DurationInMinutes: 1440
//        /// - BusyTime: 1440
//        ///
//        /// - Category ID: 9
//        /// - Event ID: 3
//        /// - Start Date Time: 1/10/2020 12:00:00 AM
//        /// - Category: Vacation
//        /// - Short Description:
//        /// - DurationInMinutes: 1440
//        /// - BusyTime: 2880
//        /// </code>
//        /// </example>
//        public List<CalendarItem> GetCalendarItems(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
//        {
//            // ------------------------------------------------------------------------
//            // return joined list within time frame
//            // ------------------------------------------------------------------------
//            Start = Start ?? new DateTime(1900, 1, 1);
//            End = End ?? new DateTime(2500, 1, 1);

//            var query = from c in _categories.List()
//                        join e in _events.List() on c.Id equals e.Category
//                        where e.StartDateTime >= Start && e.StartDateTime <= End
//                        orderby e.StartDateTime
//                        select new { CatId = c.Id, EventId = e.Id, e.StartDateTime, Category = c.Description, e.Details, e.DurationInMinutes };

//            // ------------------------------------------------------------------------
//            // create a CalendarItem list with totals,
//            // ------------------------------------------------------------------------
//            List<CalendarItem> items = new List<CalendarItem>();
//            Double totalBusyTime = 0;

//            foreach (var e in query.OrderBy(q => q.StartDateTime))
//            {
//                // filter out unwanted categories if filter flag is on
//                if (FilterFlag && CategoryID != e.CatId)
//                {
//                    continue;
//                }

//                // keep track of running totals
//                //Category c = _categories.GetCategoryFromId(e.CatId);
//                //if (c.Type != Category.CategoryType.Availability)
//                //{
//                //    totalBusyTime = totalBusyTime + e.DurationInMinutes;
//                //}

//                items.Add(new CalendarItem
//                {
//                    CategoryID = e.CatId,
//                    EventID = e.EventId,
//                    ShortDescription = e.Details,
//                    StartDateTime = e.StartDateTime,
//                    DurationInMinutes = e.DurationInMinutes,
//                    Category = e.Category,
//                    BusyTime = totalBusyTime
//                });
//            }

//            return items;
//        }

//        // ============================================================================
//        // Group all events month by month (sorted by year/month)
//        // returns a list of CalendarItemsByMonth which is 
//        // "year/month", list of calendar items, and totalBusyTime for that month
//        // ============================================================================
//        /// <summary>
//        /// Retrieves a list of events by month
//        /// </summary>
//        /// <param name="Start">Starting day</param>
//        /// <param name="End">Ending date</param>
//        /// <param name="FilterFlag">Filters by category</param>
//        /// <param name="CategoryID">Current Category ID</param>
//        /// <returns>A list of Calendar Items Categorized by month</returns>
//        /// <example>  
//        /// Assuming we have the following output
//        /// <code>
//        /// Month: 2018/01
//        ///--Items--
//        ///    Detail #1
//        ///     - Start Date Time: 1/10/2018 10:00:00 AM
//        ///     - Category: Fun
//        ///     - Short Description:
//        ///     - DurationInMinutes: 40
//        ///     - BusyTime: 40
//        ///
//        ///    Detail #2
//        ///     - Start Date Time: 1/11/2018 10:15:00 AM
//        ///     - Category: Work
//        ///     - Short Description:
//        ///     - DurationInMinutes: 60
//        ///     - BusyTime: 100
//        ///
//        ///    Detail #3
//        ///     - Start Date Time: 1/11/2018 7:30:00 PM
//        ///     - Category: Work
//        ///     - Short Description:
//        ///     - DurationInMinutes: 15
//        ///     - BusyTime: 115
//        ///
//        /// Total Busy Time: 115
//        /// </code>  
//        /// 
//        /// <b>Retrieves a list of category items by month and outputs to screen</b>  
//        /// <code>
//        /// <![CDATA[ 
//        /// Console.WriteLine("Calendar Items by Month");
//        /// calendarItemsByMonth.ForEach(calItmsByMtn =>
//        /// { 
//        ///     Console.WriteLine("Month: {0}", calItmsByMtn.Month);
//        ///     Console.WriteLine("--Items--");
//        ///     int detailNum = 1;
//        ///     calItmsByMtn.Items.ForEach(i =>
//        ///     {
//        ///         Console.WriteLine($"    Detail #{detailNum}");
//        ///         Console.WriteLine("     - Start Date Time: {0}", i.StartDateTime);
//        ///         Console.WriteLine("     - Category: {0}", i.Category);
//        ///         Console.WriteLine("     - Short Description: {0}", i.ShortDescription);
//        ///         Console.WriteLine("     - DurationInMinutes: {0}", i.DurationInMinutes);
//        ///         Console.WriteLine("     - BusyTime: {0}", i.BusyTime);
//        ///         Console.WriteLine("");
//        ///         detailNum++;
//        ///     });
//        ///     Console.WriteLine("Total Busy Time: {0}", calItmsByMtn.TotalBusyTime);
//        ///     Console.WriteLine("\n");
//        /// });
//        /// ]]>
//        /// </code>
//        /// 
//        /// <b>Output results with filter flag on (assuming category id is 9)</b>
//        /// 
//        /// <code>
//        /// Month: 2020/01
//        /// --Items--
//        ///    Detail #1
//        ///     - Start Date Time: 2020-01-09 12:00:00 AM
//        ///     - Category: Vacation
//        ///     - Short Description:
//        ///     - DurationInMinutes: 1440
//        ///     - BusyTime: 1440
//        ///
//        ///    Detail #2
//        ///     - Start Date Time: 2020-01-10 12:00:00 AM
//        ///     - Category: Vacation
//        ///     - Short Description:
//        ///     - DurationInMinutes: 1440
//        ///     - BusyTime: 2880
//        ///
//        /// Total Busy Time: 2880
//        /// </code> 
//        /// </example>
//        public List<CalendarItemsByMonth> GetCalendarItemsByMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
//        {

//            /*
//            =============================================================================
//                Initializes the CalendarItemsByMonth list to store calendar items         
//            =============================================================================
//            */
//            List<CalendarItemsByMonth> calendarItemsForEveryMonth = new List<CalendarItemsByMonth>();


//            /*
//            =============================================================================
//                If no start or end date are provided it will initiaze to the default one
//                And Formatting start and end date strings
//            =============================================================================
//            */

//            Start = Start ?? new DateTime(1900, 1, 1);
//            End = End ?? new DateTime(2500, 1, 1);

//            string startDateStringFormat = Start?.ToString("yyyy-MM-dd HH:mm:ss");
//            string endDateStringFormat = End?.ToString("yyyy-MM-dd HH:mm:ss");

//            /*
//            =============================================================================
//                This Is to get the connection to the database using the database Class
//                and create a command using the connection to the database
//            =============================================================================
//                */
//            var databaseConnection = Database.dbConnection;
//            using var cmd = new SQLiteCommand(databaseConnection);

//            /*
//            =============================================================================
//                Constructing the SQL query and executing it
//            =============================================================================
//            */
//            cmd.CommandText = "SELECT e.*, c.Description " +
//                        "FROM Events e Join categories c  " +
//                        "ON e.CategoryId = c.Id Where e.StartDateTime >= @startDate " +
//                        "AND e.StartDateTime <= @endDate " +
//                        "ORDER BY e.StartDateTime; ";

//            cmd.Parameters.Add(new SQLiteParameter("@startDate", startDateStringFormat));
//            cmd.Parameters.Add(new SQLiteParameter("@endDate", endDateStringFormat));

//            using SQLiteDataReader reader = cmd.ExecuteReader();

//            List<CalendarItem> monthlyEventsList = new List<CalendarItem>();

//            const int EVENTID = 0, STARTDATETIME = 1, DURATIONINMINUTES = 2, EVENTDETAILS = 3, CATEGORYID = 4, CATEGORYDESCRIPTION = 5;
//            while (reader.Read())
//            {  /*
//                =============================================================================
//                    filter out unwanted categories if filter flag is on
//                =============================================================================
//                */
//                if (FilterFlag && CategoryID != reader.GetInt32(CATEGORYID))
//                {
//                    continue;
//                }

//                /*
//                =============================================================================
//                    Adds Current Event To List
//                =============================================================================
//                */
//                monthlyEventsList.Add(new CalendarItem
//                {
//                    CategoryID = reader.GetInt32(CATEGORYID),
//                    EventID = reader.GetInt32(EVENTID),
//                    ShortDescription = reader.GetString(EVENTDETAILS),
//                    StartDateTime = DateTime.Parse(reader.GetString(STARTDATETIME)),
//                    DurationInMinutes = reader.GetDouble(DURATIONINMINUTES),
//                    Category = reader.GetString(CATEGORYDESCRIPTION),
//                    BusyTime = reader.GetDouble(DURATIONINMINUTES),
//                });
//            }

//            var groupByYear = monthlyEventsList.GroupBy(e => e.StartDateTime.Year).ToList();

//            //In each year
//            foreach (var year in groupByYear)
//            {

//                double totalBusyTime = 0;
//                var itemsInOneMonth = year.GroupBy(e => e.StartDateTime.Month).ToList();

//                //Calculate BusyTime
//                List<CalendarItem> itemsInThatMonth = new List<CalendarItem>();

//                //Go through each item in that month
//                foreach (var itemsByMonth in year)
//                {
//                    Category c = _categories.GetCategoryFromId(itemsByMonth.CategoryID);
//                    if (c.Type != Category.CategoryType.Availability)
//                    {
//                        totalBusyTime += itemsByMonth.DurationInMinutes;
//                    }



//                    //All all items in specific month
//                    itemsInThatMonth.Add(new CalendarItem
//                    {
//                        CategoryID = itemsByMonth.CategoryID,
//                        EventID = itemsByMonth.EventID,
//                        ShortDescription = itemsByMonth.ShortDescription,
//                        StartDateTime = itemsByMonth.StartDateTime,
//                        DurationInMinutes = itemsByMonth.DurationInMinutes,
//                        Category = itemsByMonth.Category,
//                        BusyTime = itemsByMonth.BusyTime,
//                    });
//                }

//                string monthAppender = itemsInOneMonth[0].Key <= 9 ? $"0{itemsInOneMonth[0].Key}" : $"{itemsInOneMonth[0].Key}";

//                calendarItemsForEveryMonth.Add(new CalendarItemsByMonth
//                {
//                    Month = $"{year.Key}/{monthAppender}",
//                    Items = itemsInThatMonth.OrderBy(i => i.StartDateTime).ToList(),
//                    TotalBusyTime = totalBusyTime
//                });

//            }

//            /*
//            =============================================================================
//                Returns the list of calendar items by month
//            =============================================================================
//            */

//            return calendarItemsForEveryMonth;

//        }

//        // ============================================================================
//        // Group all events by category (ordered by category name)
//        // ============================================================================
//        /// <summary>
//        /// Retrieves a list of events by Category
//        /// </summary>
//        /// <param name="Start">Starting date</param>
//        /// <param name="End">Ending date</param>
//        /// <param name="FilterFlag">Filters by month</param>
//        /// <param name="CategoryID">Current Category ID</param>
//        /// <returns></returns>
//        /// <example>
//        /// 
//        /// Assuming we get the following output:
//        /// <code> 
//        /// Category: Birthdays
//        /// --Items--
//        ///    Detail #1
//        ///     - Category ID: 11
//        ///     - Event ID: 7
//        ///     - Start Date Time: 2020-01-12 12:00:00 AM
//        ///     - Category: Birthdays
//        ///     - Short Description:
//        ///     - DurationInMinutes: 1440
//        ///     - BusyTime: 5875
//        ///
//        /// Total Busy Time: 1440
//        ///
//        /// Category: Canadian Holidays
//        /// --Items--
//        ///    Detail #1
//        ///     - Category ID: 8
//        ///     - Event ID: 6
//        ///     - Start Date Time: 2020-01-01 12:00:00 AM
//        ///     - Category: Canadian Holidays
//        ///     - Short Description:
//        ///     - DurationInMinutes: 1440
//        ///     - BusyTime: 1555
//        ///
//        /// Total Busy Time: 1440
//        /// </code>
//        /// 
//        /// <b>Gets a list of calendar items by category and outputs data to the screen</b>
//        /// 
//        /// <code>
//        /// <![CDATA[
//        /// List<CalendarItemsByCategory> calendarItemsByCategory = calendar.GetCalendarItemsByCategory(Start, End, FilterFlag, CategoryID);
//        /// 
//        /// Console.WriteLine("Calendar Items by Category");
//        /// calendarItemsByCategory.ForEach(calItmsByCat =>
//        ///{
//        ///     Console.WriteLine("Category: {0}", calItmsByCat.Category);
//        ///     Console.WriteLine("--Items--");
//        ///     int detailNum = 1;
//        ///     calItmsByCat.Items.ForEach(i =>
//        ///     {
//        ///         Console.WriteLine($"    Detail #{detailNum}");
//        ///         Console.WriteLine("     - Category ID: {0}", i.CategoryID);
//        ///         Console.WriteLine("     - Event ID: {0}", i.EventID);
//        ///         Console.WriteLine("     - Start Date Time: {0}", i.StartDateTime);
//        ///         Console.WriteLine("     - Category: {0}", i.Category);
//        ///         Console.WriteLine("     - Short Description: {0}", i.ShortDescription);
//        ///         Console.WriteLine("     - DurationInMinutes: {0}", i.DurationInMinutes);
//        ///         Console.WriteLine("     - BusyTime: {0}", i.BusyTime);
//        ///         Console.WriteLine("");
//        ///         detailNum++;
//        ///     });
//        ///     Console.WriteLine("Total Busy Time: {0}", calItmsByCat.TotalBusyTime);
//        ///     Console.WriteLine("\n");
//        /// });
//        /// ]]>
//        /// </code> 
//        /// 
//        /// <b>If the filter flag is on, output will look like the following(assuming that the category id is 9)</b> 
//        /// <code>
//        /// Category: Vacation
//        ///--Items--
//        ///    Detail #1
//        ///     - Category ID: 9
//        ///     - Event ID: 2
//        ///     - Start Date Time: 2020-01-09 12:00:00 AM
//        ///     - Category: Vacation
//        ///     - Short Description:
//        ///     - DurationInMinutes: 1440
//        ///     - BusyTime: 1440
//        ///
//        ///    Detail #2
//        ///     - Category ID: 9
//        ///     - Event ID: 3
//        ///     - Start Date Time: 2020-01-10 12:00:00 AM
//        ///     - Category: Vacation
//        ///     - Short Description:
//        ///     - DurationInMinutes: 1440
//        ///     - BusyTime: 2880
//        ///
//        /// Total Busy Time: 2880
//        /// </code> 
//        /// </example>
//        public List<CalendarItemsByCategory> GetCalendarItemsByCategory(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
//        {
//            List<CalendarItemsByCategory> CalendarItemsByCat = new List<CalendarItemsByCategory>();

//            //Default date time            
//            Start = Start ?? new DateTime(1900, 1, 1);
//            End = End ?? new DateTime(2500, 1, 1);

//            string startDateStringFormat = Start?.ToString("yyyy-MM-dd HH:mm:ss");
//            string endDateStringFormat = End?.ToString("yyyy-MM-dd HH:mm:ss");


//            //SQL Query Operations to get results accordingly 
//            var databaseConnection = Database.dbConnection;
//            using var cmd = new SQLiteCommand(databaseConnection);

//            cmd.CommandText = "SELECT e.*, c.Description " +
//                        "FROM Events e Inner Join categories c " +
//                        "ON e.CategoryId = c.Id Where e.StartDateTime >= @startDate " +
//                        "AND e.StartDateTime <= @endDate " +
//                        "ORDER BY e.StartDateTime;";
//            cmd.Parameters.Add(new SQLiteParameter("@startDate", startDateStringFormat));
//            cmd.Parameters.Add(new SQLiteParameter("@endDate", endDateStringFormat));
//            using SQLiteDataReader reader = cmd.ExecuteReader();


//            const int EVENTID = 0, STARTDATETIME = 1, DURATIONINMINUTES = 2, EVENTDETAILS = 3, CATEGORYID = 4, CATEGORYDESCRIPTION = 5;
//            List<CalendarItemsByCategory> results = new List<CalendarItemsByCategory>();

//            //Get all data it found
//            while (reader.Read())
//            {
//                /*
//                =============================================================================
//                    filter out unwanted categories if filter flag is on
//                =============================================================================
//                */
//                if (FilterFlag && CategoryID != reader.GetInt32(CATEGORYID))
//                {
//                    continue;
//                }

//                results.Add(new CalendarItemsByCategory
//                {
//                    Category = reader.GetString(5),
//                    Items = new List<CalendarItem>
//                    {
//                        new CalendarItem
//                        {
//                            CategoryID = reader.GetInt16(CATEGORYID),
//                            EventID = reader.GetInt16(EVENTID),
//                            StartDateTime = reader.GetDateTime(STARTDATETIME),
//                            Category = reader.GetString(CATEGORYDESCRIPTION),
//                            ShortDescription = reader.GetString(EVENTDETAILS),
//                            DurationInMinutes = reader.GetDouble(DURATIONINMINUTES),
//                            BusyTime = reader.GetDouble(DURATIONINMINUTES),
//                        }
//                    },
//                    TotalBusyTime = 0
//                });

//            }

//            var groupedByCategory = results.GroupBy(c => c.Category).ToList();

//            //Calculate the TotalBusyTime per category            
//            const int DEFAULTITEMSINDEX = 0;
//            foreach (var category in groupedByCategory)
//            {
//                double totalBusyTime = 0;
//                List<CalendarItem> itemsInOneCategory = new List<CalendarItem>();

//                //Put all calendar items into one category
//                foreach (var itemsByCategory in category)
//                {
//                    //Add in a list that will redefine category Items
//                    foreach (var calItems in itemsByCategory.Items)
//                    {
//                        // keep track of running totals
//                        Category c = _categories.GetCategoryFromId(calItems.CategoryID);
//                        if (c.Type != Category.CategoryType.Availability)
//                        {
//                            totalBusyTime += calItems.DurationInMinutes;
//                        }


//                        itemsInOneCategory.Add(new CalendarItem
//                        {
//                            BusyTime = calItems.BusyTime,
//                            Category = calItems.Category,
//                            CategoryID = calItems.CategoryID,
//                            DurationInMinutes = calItems.DurationInMinutes,
//                            EventID = calItems.EventID,
//                            ShortDescription = calItems.ShortDescription,
//                            StartDateTime = calItems.StartDateTime
//                        });
//                    }
//                }

//                //Add results
//                CalendarItemsByCat.Add(new CalendarItemsByCategory
//                {
//                    Category = category.Key,
//                    Items = itemsInOneCategory.OrderBy(i => i.StartDateTime).ToList(),
//                    TotalBusyTime = totalBusyTime
//                });

//                //Order by date
//                CalendarItemsByCat = CalendarItemsByCat.OrderBy(i => i.Category).ToList();
//            }
//            return CalendarItemsByCat;
//        }



//        // ============================================================================
//        // Group all events by category and Month
//        // creates a list of Dictionary objects with:
//        //          one dictionary object per month,
//        //          and one dictionary object for the category total busy times
//        // 
//        // Each per month dictionary object has the following key value pairs:
//        //           "Month", <name of month>
//        //           "TotalBusyTime", <the total durations for the month>
//        //             for each category for which there is an event in the month:
//        //             "items:category", a List<CalendarItem>
//        //             "category", the total busy time for that category for this month
//        // The one dictionary for the category total busy times has the following key value pairs:
//        //             for each category for which there is an event in ANY month:
//        //             "category", the total busy time for that category for all the months
//        // ============================================================================
//        /// <summary>
//        /// Retrieves a list of dictionaries containing data related to events grouped by category and Month
//        /// </summary>
//        /// <param name="Start">Start date</param>
//        /// <param name="End">End Date</param>
//        /// <param name="FilterFlag">Filter by category</param>
//        /// <param name="CategoryID">Given category Id</param>
//        /// <returns>A list of dictionaries with KeyValuePair Data</returns>
//        /// <example>
//        /// 
//        /// Assuming we have the following output
//        /// <code>
//        /// Month: 2018/01
//        /// TotalBusyTime:115
//        ///
//        /// *Categories*
//        /// -- Fun --
//        ///    Details #1
//        ///     - Category ID: 3
//        ///     - Event ID: 1
//        ///     - Start Date Time: 2018-01-10 10:00:00 AM
//        ///     - Category: Fun
//        ///     - Short Description:
//        ///     - DurationInMinutes: 40
//        ///     - BusyTime: 40
//        ///
//        ///
//        /// -- Work --
//        ///    Details #1
//        ///     - Category ID: 2
//        ///     - Event ID: 8
//        ///     - Start Date Time: 2018-01-11 10:15:00 AM
//        ///     - Category: Work
//        ///     - Short Description:
//        ///     - DurationInMinutes: 60
//        ///     - BusyTime: 100
//        ///
//        ///    Details #2
//        ///     - Category ID: 2
//        ///     - Event ID: 5
//        ///     - Start Date Time: 2018-01-11 7:30:00 PM
//        ///     - Category: Work
//        ///     - Short Description:
//        ///     - DurationInMinutes: 15
//        ///     - BusyTime: 115
//        ///     
//        /// TOTALS
//        /// - Work: 75
//        /// - Fun: 40
//        /// - On call: 180
//        /// - Canadian Holidays: 1440
//        /// - Vacation: 2880
//        /// - Birthdays: 1440
//        /// </code>
//        /// 
//        /// <b>Retrieves a list of dictionaries, filters data, prints out the items by category for a given month. 
//        ///    Then prints out the total busytime for all categories.</b>
//        /// <code>
//        /// <![CDATA[
//        /// 
//        /// List<Dictionary<string, object>> calendarDictionaryByCategoryAndMonth = calendar.GetCalendarDictionaryByCategoryAndMonth(Start, End, FilterFlag, CategoryID);
//        /// 
//        /// Calendar items by Category and Month
//        /// Console.WriteLine("Calendar Item by Category and Month");             
//        /// calendarDictionaryByCategoryAndMonth.ForEach(dictionary =>
//        /// {
//        ///    bool printingFinalResults = false;
//        ///    int catDictionaryIndex = 0;
//        ///
//        ///    //Print out current month
//        ///    Console.WriteLine(dictionary["Month"] != "TOTALS" ? $"Month: {dictionary["Month"]}": dictionary["Month"]);
//        ///        
//        ///        
//        ///    //Print out the items of the current category
//        ///    List<string> catDictionaryKeys = dictionary.Keys.Where(k => (!k.Equals("Month") && !k.StartsWith("Total")) &&
//        ///                                                            !k.StartsWith("items"))
//        ///                                                           .Select(s => s).ToList();
//        ///                  
//        ///    foreach (var item in dictionary)
//        ///    {
//        ///            
//        ///        //If the value "TOTAL" passes by, printingFinalResults remains true until the end of the operation
//        ///        printingFinalResults = item.Value == "TOTALS" || printingFinalResults == true ? true : false;
//        ///
//        ///        if (item.Key == "TotalBusyTime")
//        ///        {
//        ///            Console.WriteLine($"{item.Key}:{item.Value}");
//        ///            Console.WriteLine("\n*Categories*");
//        ///        }
//        ///                
//        ///        //If the program is ready to print the final results, it will only render the key value pairs
//        ///        if (!printingFinalResults)
//        ///        { 
//        ///            //Checks if the value type is a list
//        ///            if (item.Value is IList<CalendarItem> valueList)
//        ///            {
//        ///                Console.WriteLine($"-- {catDictionaryKeys[catDictionaryIndex]} --");                        
//        ///                catDictionaryIndex++;
//        ///                int detailNumber = 1;
//        ///                foreach (var val in valueList)
//        ///                {
//        ///                    Console.WriteLine($"    Details #{detailNumber}");
//        ///                    Console.WriteLine("     - Category ID: {0}", val.CategoryID);
//        ///                    Console.WriteLine("     - Event ID: {0}", val.EventID);
//        ///                    Console.WriteLine("     - Start Date Time: {0}", val.StartDateTime);
//        ///                    Console.WriteLine("     - Category: {0}", val.Category);
//        ///                    Console.WriteLine("     - Short Description: {0}", val.ShortDescription);
//        ///                    Console.WriteLine("     - DurationInMinutes: {0}", val.DurationInMinutes);
//        ///                    Console.WriteLine("     - BusyTime: {0}\n", val.BusyTime);
//        ///                    detailNumber++;
//        ///                }
//        ///                Console.WriteLine("");
//        ///            }
//        ///        }
//        ///        else
//        ///        {
//        ///            //Prints out final results
//        ///            if (item.Key != "Month")
//        ///                Console.WriteLine($"    - {item.Key}: {item.Value}");
//        ///        }
//        ///    }
//        ///    Console.WriteLine("");
//        /// }); 
//        /// ]]>
//        /// </code>
//        /// 
//        /// <b>Output with filter flag on (assuming category id is 9)</b>
//        /// 
//        /// <code>
//        ///  Calendar Item by Category and Month
//        ///Month: 2020/01
//        ///TotalBusyTime:2880
//        ///
//        ///*Categories*
//        ///-- Vacation --
//        ///    Details #1
//        ///     - Category ID: 9
//        ///     - Event ID: 2
//        ///     - Start Date Time: 2020-01-09 12:00:00 AM
//        ///     - Category: Vacation
//        ///     - Short Description:
//        ///     - DurationInMinutes: 1440
//        ///     - BusyTime: 1440
//        ///
//        ///    Details #2
//        ///     - Category ID: 9
//        ///     - Event ID: 3
//        ///     - Start Date Time: 2020-01-10 12:00:00 AM
//        ///     - Category: Vacation
//        ///     - Short Description:
//        ///     - DurationInMinutes: 1440
//        ///     - BusyTime: 2880
//        ///
//        ///TOTALS
//        ///    - Vacation: 2880
//        /// </code>
//        /// </example>
//        public List<Dictionary<string, object>> GetCalendarDictionaryByCategoryAndMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
//        {
//            // -----------------------------------------------------------------------
//            // get all items by month 
//            // -----------------------------------------------------------------------
//            List<CalendarItemsByMonth> GroupedByMonth = GetCalendarItemsByMonth(Start, End, FilterFlag, CategoryID);

//            // -----------------------------------------------------------------------
//            // loop over each month
//            // -----------------------------------------------------------------------
//            var summary = new List<Dictionary<string, object>>();
//            var totalBusyTimePerCategory = new Dictionary<String, Double>();

//            foreach (var MonthGroup in GroupedByMonth)
//            {
//                // create record object for this month
//                Dictionary<string, object> record = new Dictionary<string, object>();
//                record["Month"] = MonthGroup.Month;
//                record["TotalBusyTime"] = MonthGroup.TotalBusyTime;

//                // break up the month items into categories
//                var GroupedByCategory = MonthGroup.Items.GroupBy(c => c.Category);

//                // -----------------------------------------------------------------------
//                // loop over each category
//                // -----------------------------------------------------------------------
//                foreach (var CategoryGroup in GroupedByCategory.OrderBy(g => g.Key))
//                {

//                    // calculate totals for the cat/month, and create list of items
//                    double totalCategoryBusyTimeForThisMonth = 0;
//                    var details = new List<CalendarItem>();

//                    foreach (var item in CategoryGroup)
//                    {
//                        totalCategoryBusyTimeForThisMonth = totalCategoryBusyTimeForThisMonth + item.DurationInMinutes;
//                        details.Add(item);
//                    }

//                    // add new properties and values to our record object
//                    record["items:" + CategoryGroup.Key] = details;
//                    record[CategoryGroup.Key] = totalCategoryBusyTimeForThisMonth;

//                    // keep track of totals for each category
//                    if (totalBusyTimePerCategory.TryGetValue(CategoryGroup.Key, out Double currentTotalBusyTimeForCategory))
//                    {
//                        totalBusyTimePerCategory[CategoryGroup.Key] = currentTotalBusyTimeForCategory + totalCategoryBusyTimeForThisMonth;
//                    }
//                    else
//                    {
//                        totalBusyTimePerCategory[CategoryGroup.Key] = totalCategoryBusyTimeForThisMonth;
//                    }
//                }

//                // add record to collection
//                summary.Add(record);
//            }
//            // ---------------------------------------------------------------------------
//            // add final record which is the totals for each category
//            // ---------------------------------------------------------------------------
//            Dictionary<string, object> totalsRecord = new Dictionary<string, object>();
//            totalsRecord["Month"] = "TOTALS";

//            foreach (var cat in categories.List())
//            {
//                try
//                {
//                    totalsRecord.Add(cat.Description, totalBusyTimePerCategory[cat.Description]);
//                }
//                catch { }
//            }
//            summary.Add(totalsRecord);


//            return summary;
//        }
//        #endregion GetList
//    }
//}