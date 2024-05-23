using Calendar;
using CalendarItem = Calendar.CalendarItem;
using CalendarCodeTests;
using HomeCalendarGUI;
using System.Data.SQLite;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Controls.Primitives;
using Xunit;
using Xunit.Sdk;

namespace MVP_Tests
{
    public class TestView : IView
    {
        public List<Category> categories;
        public List<Event> events;
        public List<CalendarItem> calendarItems;
        public List<CalendarItemsByCategory> calendarItemsByCategory;
        public List<CalendarItemsByMonth> calendarItemsByMonth;
        public List<Dictionary<string, object>> calendarItemsByCategoryAndMonth;
        public bool calledPopulateAllCategoriesComboBox = false;
        public bool calledDisplayErrorMessage = false;
        public bool calledDisplaySuccessfulMessage = false;    
        public bool calledPopulateCategoryTypesComboBox = false;
        public bool calledShowCalendarItems = false;
        public bool calledShowTotalBusyTimeByMonth = false;
        public bool calledShowTotalBusyTimeByMonthAndCategory = false;
        public bool calledShowTotalBusyTimeByCategory = false;

        public void PopulateCategoriesInAllCatsComboBox(List<Category> categories)
        {
            calledPopulateAllCategoriesComboBox = true;
            this.categories = categories;
        }

        public void DisplayErrorMessage(string msg)
        {
            calledDisplayErrorMessage = true;
        }

        public void DisplaySuccessfulMessage(string msg)
        {
            calledDisplaySuccessfulMessage = true;
        }

        public void PopulateCategoryTypesComboBox(List<Category> categories)
        {
            calledPopulateCategoryTypesComboBox = true;
        }

        public void ShowCalendarItems(List<CalendarItem> items)
        {
            calledShowCalendarItems=true;
            calendarItems = items;
        }

        public void ShowTotalBusyTimeByMonth(List<CalendarItemsByMonth> itemsByMonth)
        {
            calledShowTotalBusyTimeByMonth = true;
            calendarItemsByMonth = itemsByMonth;
        }

        public void ShowTotalBusyTimeByCategory(List<CalendarItemsByCategory> itemsByCategory)
        {
            calledShowTotalBusyTimeByCategory = true;
            calendarItemsByCategory = itemsByCategory;
        }

        public void ShowTotalBusyTimeByMonthAndCategory(List<Dictionary<string, object>> itemsByCategoryAndMonth)
        {
            calledShowTotalBusyTimeByMonthAndCategory = true;
            calendarItemsByCategoryAndMonth = itemsByCategoryAndMonth;
        }

        #region Dead Code
        //public bool calledShowCalendarItemOnDataGrid = false;
        //public bool calledShowCalendarItemsWithDateFiltersOn = false;
        //public bool calledShowEventsOnDataGrid = false;
        //public bool calledShowEventsWithFiltersOn = false;
        //public bool calledShowCalendarItemsWithCategoryFiltersOn = false;
        //public bool calledShowCalendarItemsByMonth = false;

        //public void ShowCalendarItemsOnDataGrid(List<CalendarItem> calendarItems)
        //{
        //    calledShowCalendarItemOnDataGrid = true;   
        //    this.calendarItems = calendarItems;
        //}

        //public void ShowCalendarItemsWithCategoryFiltersOn(List<CalendarItem> calendarItems)
        //{
        //    this.calendarItems = calendarItems;
        //    calledShowCalendarItemsWithCategoryFiltersOn = true;
        //}

        //public void ShowCalendarItemsFilteredByMonth(Dictionary<string, double> itemsByMonthAndTime)
        //{
        //    calledShowCalendarItemsByMonth = true; 
        //}


        //public void ShowCalendarItemsWithDateFiltersOn(List<CalendarItem> calendarItems)
        //{
        //    calledShowCalendarItemsWithDateFiltersOn = true;
        //    this.calendarItems = calendarItems;

        //}
        #endregion
    }

    public class TestPresenter
    {
        [Fact]
        public void TestConstructor()
        {
            //arrange
            TestView view = new TestView();

            //act
            Presenter p = new Presenter(view);
          
            //assert
            Assert.IsType<Presenter>(p);
        }

        [Fact]
        public void Test_Constructor_DatabaseConnection_Failure()
        {
            //arrange
            TestView view = new TestView();
            List<CalendarItem> expectedResults = TestConstants.getCalendarItems_NoFilters();

            //act and assert
            Presenter p = new Presenter(view, "../fail.db");

            Assert.True(view.calledDisplayErrorMessage);
        }

        [Fact]
        public void TestAddCategory_Fail()
        {
            //arrange 
            TestView view = new TestView();
            Presenter p = new Presenter(view);

            //act
            p.AddNewCategory(null, Category.CategoryType.Event);

            //assert   
            Assert.True(view.calledDisplayErrorMessage);
        }

        [Fact]
        public void TestAddCategory_Success()
        {
            //arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);

            //act
            p.AddNewCategory("description", Category.CategoryType.Holiday);

            //assert
            Assert.True(view.calledDisplaySuccessfulMessage);
        }

        [Fact]
        public void Test_Categories_Drop_Down()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String existingDB = $"{folder}\\{TestConstants.testDBInputFile}";
            SQLiteConnection conn = Database.dbConnection;
            TestView view = new TestView();
            Presenter p = new Presenter(view, existingDB);
            int expectedNumberOfCategories = 12;
            List<Category> expectedResults = TestConstants.getDefaultCategories();

            //Act
            p.GetCategoriesForAllCatsComboBoxes();

            //Assert
            Assert.True(view.calledPopulateAllCategoriesComboBox);
            Assert.Equal(expectedNumberOfCategories, view.categories.Count);

            for (int i = 0; i < expectedResults.Count; i++)
            {
                Assert.Equal(expectedResults[i].Description.ToLower(), view.categories[i].Description.ToLower());
                Assert.Equal(expectedResults[i].Type, view.categories[i].Type);
            }
        }

        [Fact]
        public void TestGetCategoryTypesInList()
        {
            //arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);

            //act 
            p.GetCategoriesTypeInList();

            //assert 
            Assert.True(view.calledPopulateCategoryTypesComboBox); 
        }

        [Fact]
        public void Test_DeleteAnEvent()
        {
            //Arrange 
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            File.Copy(goodDB, messyDB, true);
            TestView view = new TestView();
            Presenter p = new Presenter(view,messyDB);
            int eveIdToDelete = 3;

            //Act
            p.GetHomeCalendarItems(null,null,0,false,false,false,false);
            List<CalendarItem> initial = view.calendarItems;
            CalendarItem itemToDelete = initial.Where(e => e.EventID == eveIdToDelete).First();            
            p.DeleteAnEvent(itemToDelete.EventID);
            p.GetHomeCalendarItems(null, null, 0, false, false, false, false);
            List<CalendarItem> results = view.calendarItems;

            //Assert
            Assert.True(results.Count < initial.Count);        
        }

        [Fact]
        public void Test_DeleteAnEvent_Fail()
        {
            //Arrange 
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            File.Copy(goodDB, messyDB, true);
            TestView view = new TestView();
            Presenter p = new Presenter(view, messyDB);
            int eveIdToDelete = 3;

            //Act
            p.GetHomeCalendarItems(null, null, 0, false, false, false, false);
            List<CalendarItem> initial = view.calendarItems;
            CalendarItem itemToDelete = initial.Where(e => e.EventID == eveIdToDelete).First();
            p.DeleteAnEvent(itemToDelete.EventID);  

            //Assert            
            Assert.Throws<InvalidOperationException>(() => { p.DeleteAnEvent(itemToDelete.EventID); });
        }
                 
        [Fact]
        public void Test_GetHomeCalendarItems_No_Filters()
        {
            //arrange
            TestView view = new TestView();
            List<CalendarItem> expectedResults = TestConstants.getCalendarItems_NoFilters();
            string databasePath = $"{TestConstants.GetSolutionDir()}\\{TestConstants.testDBInputFile}";
            Presenter p = new Presenter(view,databasePath);


            //act
            p.GetHomeCalendarItems(null,null,0,false,false,false,false);
            expectedResults = expectedResults.OrderBy(e => e.EventID).ToList();
            view.calendarItems = view.calendarItems.OrderBy(e => e.EventID).ToList();


            //assert 
            Assert.True(view.calledShowCalendarItems);
            Assert.Equal(expectedResults.Count, view.calendarItems.Count);

            
            for (int i = 0; i < view.calendarItems.Count; i++)
            {
                Assert.Equal(expectedResults[i].CategoryID, view.calendarItems[i].CategoryID);
                Assert.Equal(expectedResults[i].EventID, view.calendarItems[i].EventID);
                Assert.Equal(expectedResults[i].DurationInMinutes, view.calendarItems[i].DurationInMinutes);                
            }
        }

        [Fact]
        public void Test_GetHomeCalendarItems_Calendar_FilterByCategory_On()
        {
            //arrange
            TestView view = new TestView();
            List<CalendarItem> expectedResults = TestConstants.getCalendarItemsByCatId2();
            string databasePath = $"{TestConstants.GetSolutionDir()}\\{TestConstants.testDBInputFile}";
            Presenter p = new Presenter(view, databasePath);
            int categoryId = 2;


            //act
            p.GetHomeCalendarItems(null, null, categoryId, false, false, false, true);
            expectedResults = expectedResults.OrderBy(e => e.EventID).ToList();
            view.calendarItems = view.calendarItems.OrderBy(e => e.EventID).ToList();

            //assert 
            Assert.True(view.calledShowCalendarItems);
            Assert.Equal(expectedResults.Count, view.calendarItems.Count);

            for (int i = 0; i < view.calendarItems.Count; i++)
            {
                Assert.Equal(expectedResults[i].CategoryID, view.calendarItems[i].CategoryID);
                Assert.Equal(expectedResults[i].EventID, view.calendarItems[i].EventID);
                Assert.Equal(expectedResults[i].DurationInMinutes, view.calendarItems[i].DurationInMinutes);
            }
        }

        [Fact]
        public void Test_GetHomeCalendarItems_Calendar_DateFilter_On()
        {
            //Arrange
            String folder = TestConstants.GetSolutionDir();
            String existingDB = $"{folder}\\{TestConstants.testDBInputFile}";
            TestView view = new TestView();
            Presenter p = new Presenter(view, existingDB);
            List<CalendarItem> expectedResults = TestConstants.getCalendarItems2018();
                        
            DateTime? start = DateTime.Parse("January 1 2018");
            DateTime? end = DateTime.Parse("December 31 2018");

            //Act 
            p.GetHomeCalendarItems(start,end,0,true,false,false,false);
            expectedResults = expectedResults.OrderBy(e => e.EventID).ToList();
            view.calendarItems = view.calendarItems.OrderBy(e => e.EventID).ToList();


            //Assert
            Assert.True(view.calledShowCalendarItems);
            Assert.Equal(expectedResults.Count, view.calendarItems.Count);

            for (int i = 0; i < view.calendarItems.Count; i++)
            {
                Assert.Equal(expectedResults[i].CategoryID, view.calendarItems[i].CategoryID);
                Assert.Equal(expectedResults[i].EventID, view.calendarItems[i].EventID);
                Assert.Equal(expectedResults[i].DurationInMinutes, view.calendarItems[i].DurationInMinutes);               
                Assert.Equal(expectedResults[i].StartDateTime, view.calendarItems[i].StartDateTime);               
            }
        }

        [Fact]
        public void Test_GetHomeCalendarItems_SummaryByCategory_On()
        {
            //This is tested with category filter on

            //Arrange
            TestView view = new TestView();            
            string databasePath = $"{TestConstants.GetSolutionDir()}\\{TestConstants.testDBInputFile}";
            Presenter p = new Presenter(view, databasePath);
            List<CalendarItemsByCategory> expectedResults = TestConstants.getCalendarItemsByCategoryCat11();
            int catId = 11;

            //Act
            p.GetHomeCalendarItems(null, null, catId, false, true, false, true);


            //Assert
            Assert.True(view.calledShowTotalBusyTimeByCategory);
            Assert.Equal(expectedResults.Count, view.calendarItemsByCategory.Count);

            for (int i = 0; i < view.calendarItemsByCategory.Count; i++)
            {
                Assert.Equal(expectedResults[i].Category, view.calendarItemsByCategory[i].Category);
                Assert.Equal(expectedResults[i].TotalBusyTime, view.calendarItemsByCategory[i].TotalBusyTime);
                Assert.Equal(expectedResults[i].Items.Count, view.calendarItemsByCategory[i].Items.Count);
            }
        }

        [Fact]
        public void Test_GetHomeCalendarItems_SummaryByMonth_On()
        {
            //This is tested with specific date range

            //Arrange
            TestView view = new TestView();
            string databasePath = $"{TestConstants.GetSolutionDir()}\\{TestConstants.testDBInputFile}";
            Presenter p = new Presenter(view, databasePath);
            List<CalendarItemsByMonth> expectedResults = TestConstants.getCalendarItemsBy2018_01();

            DateTime? start = DateTime.Parse("January 1 2018");
            DateTime? end = DateTime.Parse("January 31 2018");

            //Act
            p.GetHomeCalendarItems(start, end, 0, true, false, true, false);


            //Assert
            Assert.True(view.calledShowTotalBusyTimeByMonth);
            Assert.Equal(expectedResults.Count, view.calendarItemsByMonth.Count);

            for (int i = 0; i < view.calendarItemsByMonth.Count; i++)
            {
                Assert.Equal(expectedResults[i].Month, view.calendarItemsByMonth[i].Month);
                Assert.Equal(expectedResults[i].TotalBusyTime, view.calendarItemsByMonth[i].TotalBusyTime);
                Assert.Equal(expectedResults[i].Items.Count, view.calendarItemsByMonth[i].Items.Count);
            }
        }

        [Fact]
        public void Test_GetHomeCalendarItems_SummaryByCategory_And_Month_On()
        {
            //Arrange
            TestView view = new TestView();
            string databasePath = $"{TestConstants.GetSolutionDir()}\\{TestConstants.testDBInputFile}";
            Presenter p = new Presenter(view, databasePath);
            List<Dictionary<string, object>> expectedResults = TestConstants.getCalendarItemsByCategoryAndMonth2020();
            DateTime? start = DateTime.Parse("January 1 2020");
            DateTime? end = DateTime.Parse("January 31 2020");

            //Act
            p.GetHomeCalendarItems(start,end,0,true,true,true,false);

            //Assert
            Assert.True(view.calledShowTotalBusyTimeByMonthAndCategory);
            Assert.Equal(expectedResults.Count,view.calendarItemsByCategoryAndMonth.Count);

            
            //Top Loop
            for (int i = 0; i < view.calendarItemsByCategoryAndMonth.Count; i++)
            {
                //Loop through all keys
                string[] expectedKeys = expectedResults[i].Keys.ToArray();
                string[] resultKeys = view.calendarItemsByCategoryAndMonth[i].Keys.ToArray();                
                for (int j = 0; j < view.calendarItemsByCategoryAndMonth[i].Keys.Count; j++)
                {
                    Assert.Equal(expectedKeys[j], resultKeys[j]);
                }

                //Loop through all values
                object[] expectedValues = expectedResults[i].Values.ToArray();
                object[] resultValues = view.calendarItemsByCategoryAndMonth[i].Values.ToArray();
                for (int j = 0; j < view.calendarItemsByCategoryAndMonth[i].Values.Count; j++)
                {
                    if (resultValues[j] is List<CalendarItem>)
                        continue;

                    Assert.Equal(expectedValues[j], resultValues[j]);
                }
            }
        }             

        [Fact]
        public void Test_GetHomeCalendarItems_Calendar_DateFilter_On_Fail()
        {
            //Arrange
            String folder = TestConstants.GetSolutionDir();
            String existingDB = $"{folder}\\{TestConstants.testDBInputFile}";
            TestView view = new TestView();
            Presenter p = new Presenter(view, existingDB);
      
            //Act and Assert
            Assert.Throws<InvalidOperationException>(() => p.GetHomeCalendarItems(null, null, 0, true, false, false, false));
        }

        [Fact]
        public void Test_GetHomeCalendarItems_Calendar_DateFilter_End_Before_Start_Fail()
        {
            //Arrange
            String folder = TestConstants.GetSolutionDir();
            String existingDB = $"{folder}\\{TestConstants.testDBInputFile}";
            TestView view = new TestView();
            Presenter p = new Presenter(view, existingDB);
            List<CalendarItem> expectedResults = TestConstants.getCalendarItems2018();

            DateTime? start = DateTime.Parse("January 1 2018");
            DateTime? end = DateTime.Parse("December 31 2018");

            //Act and Assert
            Assert.Throws<InvalidOperationException>(() => p.GetHomeCalendarItems(end, start, 0, true, false, false, false));           
        }

        [Fact]
        public void TestErrorHandlingWhenDatabaseUnavailable()
        {
            // Arrange
            TestView view = new TestView();
            Presenter presenter = new Presenter(view, "invalidPath.db");

            // Act
            //presenter.GetCalendarItems();

            // Assert
            Assert.True(view.calledDisplayErrorMessage);
        }

        #region DeadCode
        //[Fact]
        //public void Test_GetCalendarItemsFilteredByMonth_Fail()
        //{
        //    //arrange
        //    TestView view = new TestView();
        //    Presenter p = new Presenter(view);

        //    //act 
        //    //p.GetCalendarItemsFilteredByMonth(new DateTime(2020, 01, 01), new DateTime(2018, 01, 01));

        //    //assert
        //    Assert.True(view.calledDisplayErrorMessage);
        //}


        //[Fact]
        //Taken care of by Test_GetHomeCalendarItems_Calendar_FilterByCategory_On
        //public void TestGetEventsFilteredByCategory_ShouldDisplayCorrectEvents()
        //{

        //    // Arrange
        //    TestView view = new TestView();
        //    string databasePath = $"{TestConstants.GetSolutionDir()}\\{TestConstants.testDBInputFile}";

        //    Presenter presenter = new Presenter(view, databasePath);

        //    int testCategoryId = 2;

        //    // Act
        //    //presenter.GetEventsFilteredByCategory(testCategoryId);

        //    // Assert
        //    // Check that the correct method on the view was called to display filtered events
        //    //Assert.True(view.calledShowCalendarItemsWithCategoryFiltersOn);

        //    // Ensures that every item in view.calendarItems matches the expected category ID
        //    // which confirms that the filtering logic is functioning correctly
        //    Assert.All(view.calendarItems, item => Assert.Equal(testCategoryId, item.CategoryID));
        //}

        /*
        [Fact]
        public void TestFilterByCategoryWithNoEventsYet()
        {
            // Arrange
            string databasePath = $"{TestConstants.GetSolutionDir()}\\{TestConstants.testDBInputFile}";
            TestView view = new TestView();
            Presenter presenter = new Presenter(view, databasePath);
            int newCategoryId = ..;  // category ID that currently has no events
            // confused when creating this test because I am not able to fetch any newly created id

            // Act
            presenter.GetEventsFilteredByCategory(newCategoryId);

            // Assert
            Assert.True(view.calledShowCalendarItemsWithCategoryFiltersOn);
            Assert.Empty(view.calendarItems);  // Check if no events are displayed
        }
        */
        //public void TestRealTimeUpdatesOnUI()
        //{
        //    // Arrange
        //    TestView view = new TestView();
        //    string databasePath = $"{TestConstants.GetSolutionDir()}\\{TestConstants.testDBInputFile}";

        //    Presenter presenter = new Presenter(view, databasePath);
        //    DateTime startDate = new DateTime(2024, 5, 5);            
        //    string description = "Team Meeting";
        //    int categoryId = 1;
        //    double duration = 120;

        //    // Act
        //    presenter.AddNewEvent(startDate, categoryId, description, duration);

        //    // Assert
        //    Assert.True(view.calledDisplaySuccessfulMessage);
        //    // Confirm that the grid update method was triggered
        //    //Assert.True(view.calledShowCalendarItemOnDataGrid);
        //}

        //[Fact]
        //public void TestUILoadUnderHeavyUse()
        //{
        // welp... this failed 
        //    // Arrange
        //    TestView view = new TestView();
        //    view.ClearCalendarItems();
        //    string databasePath = $"{TestConstants.GetSolutionDir()}\\{TestConstants.testDBInputFile}";

        //    Presenter presenter = new Presenter(view, databasePath);

        //    // Act
        //    for (int i = 0; i < 1000; i++)
        //    {
        //        presenter.AddNewEvent(DateTime.Now, DateTime.Now.AddHours(1), 1, "Stress Test Event " + i, 60);
        //    }

        //    // Assert
        //    Assert.True(view.calledShowCalendarItemOnDataGrid);
        //    Assert.Equal(1000, view.calendarItems.Count(e => e.ShortDescription.StartsWith("Stress Test Event")));
        //}

        [Fact]
        public void TestUpdateEvent_Success()
        {
            //arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);

            //act
            p.UpdateEvent(1, new DateTime(2024, 05, 11), 60, "updated description", 12);

            //assert 
            Assert.True(view.calledDisplaySuccessfulMessage);
        }

        [Fact]
        public void TestUpdateEvent_Fail()
        {
            //arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);

            //act
            p.UpdateEvent(56, null, 60, "updated description", null);

            //assert 
            Assert.True(view.calledDisplayErrorMessage);
        }
        #endregion
    }
}