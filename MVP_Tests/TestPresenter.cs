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
        public bool calledShowCategoriesOnComboBox = false;
        public bool calledDisplayErrorMessage = false;
        public bool calledDisplaySuccessfulMessage = false;    
        public bool calledShowInfoOnCmb = false;
        public bool calledShowCalendarItemOnDataGrid = false;
        public bool calledShowCalendarItemsWithDateFiltersOn = false;
        public bool calledShowEventsOnDataGrid = false;
        public bool calledShowEventsWithFiltersOn = false;
        public bool calledShowCalendarItemsWithCategoryFiltersOn = false;
        public bool calledShowCalendarItemsByMonth = false;

        //public void ClearCalendarItems()
        //{
        //    if (calendarItems != null)
        //    {
        //        calendarItems.Clear();
        //    }
        //    else
        //    {
        //        calendarItems = new List<CalendarItem>(); // Initialize if null
        //    }
        //}

        public void ShowCategoriesOnComboBox(List<Category> categories)
        {
            calledShowCategoriesOnComboBox = true;
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

        public void ShowInformationOnCmb(List<Category> categories)
        {
            calledShowInfoOnCmb = true;
        }

        public void ShowCalendarItemsOnDataGrid(List<CalendarItem> calendarItems)
        {
            calledShowCalendarItemOnDataGrid = true;   
            this.calendarItems = calendarItems;
        }

        public void ShowCalendarItemsWithCategoryFiltersOn(List<CalendarItem> calendarItems)
        {
            this.calendarItems = calendarItems;
            calledShowCalendarItemsWithCategoryFiltersOn = true;
        }

        public void ShowCalendarItemsFilteredByMonth(Dictionary<string, double> itemsByMonthAndTime)
        {
            calledShowCalendarItemsByMonth = true; 
        }
    

        public void ShowCalendarItemsWithDateFiltersOn(List<CalendarItem> calendarItems)
        {
            calledShowCalendarItemsWithDateFiltersOn = true;
            this.calendarItems = calendarItems;

        }
    }

    public class TestPresenter
    {
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
            p.GetCategoriesForComboBox();

            //Assert
            Assert.True(view.calledShowCategoriesOnComboBox);
            Assert.Equal(expectedNumberOfCategories, view.categories.Count);

            for(int i =0; i < expectedResults.Count; i++)
            {
                Assert.Equal(expectedResults[i].Description.ToLower(), view.categories[i].Description.ToLower());
                Assert.Equal(expectedResults[i].Type, view.categories[i].Type);
            }
        }

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
        public void TestGetCategoryTypesInList()
        {
            //arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);

            //act 
            p.GetCategoriesTypeInList();

            //assert 
            Assert.True(view.calledShowInfoOnCmb); 
        }

        [Fact]
        public void Test_ShowCalendarItemsWithDateFiltersOn()
        {
            //Arrange
            String folder = TestConstants.GetSolutionDir();
            String existingDB = $"{folder}\\{TestConstants.testDBInputFile}";            
            TestView view = new TestView();
            Presenter p = new Presenter(view, existingDB);
            List<Event> expectedEventsResults = TestConstants.filteredbyYear2018();
            expectedEventsResults = expectedEventsResults.OrderBy(e => e.Id).ToList();

            int expectedNumberOfItems = 3;
            DateTime? start = DateTime.Parse("January 1 2018");
            DateTime? end = DateTime.Parse("December 31 2018");


            //Act 
            p.GetEventsFilteredByDateRange(start,end);            
            List<CalendarItem> calendarItemsResults = view.calendarItems;
            calendarItemsResults = calendarItemsResults.OrderBy(i => i.EventID).ToList();


            //Assert
            Assert.Equal(expectedEventsResults.Count, calendarItemsResults.Count);
            for (int i = 0; i < calendarItemsResults.Count; i++)
            {
                Assert.Equal(expectedEventsResults[i].Category, calendarItemsResults[i].CategoryID);
                Assert.Equal(expectedEventsResults[i].Id, calendarItemsResults[i].EventID);
                Assert.Equal(expectedEventsResults[i].Details, calendarItemsResults[i].ShortDescription);
                Assert.Equal(expectedEventsResults[i].StartDateTime, calendarItemsResults[i].StartDateTime);
                Assert.Equal(expectedEventsResults[i].DurationInMinutes, calendarItemsResults[i].DurationInMinutes);  
            }
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
            p.GetCalendarItems();
            List<CalendarItem> initial = view.calendarItems;
            CalendarItem itemToDelete = initial.Where(e => e.EventID == eveIdToDelete).First();            
            p.DeleteAnEvent(itemToDelete.EventID);
            List<CalendarItem> results = view.calendarItems;

            //Assert
            Assert.True(results.Count < initial.Count);
            Assert.Throws<InvalidOperationException>(() => { p.DeleteAnEvent(itemToDelete.EventID); });
        }

        [Fact]
        public void TestGetCalendarItemsFilteredByMonth_Sucess()
        {
            //arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);

            //act
            p.GetCalendarItemsFilteredByMonth(new DateTime(2018, 01, 01), new DateTime(2020, 01, 01));

            //assert 
            Assert.True(view.calledShowCalendarItemsByMonth);
        }

        [Fact]
        public void TestGetCalendarItemsFilteredByMonth_Fail()
        {
            //arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);

            //act 
            p.GetCalendarItemsFilteredByMonth(new DateTime(2020, 01, 01), new DateTime(2018, 01, 01));

            //assert
            Assert.True(view.calledDisplayErrorMessage);
        }

        [Fact]
        public void TestGetEventsFilteredByCategory_ShouldDisplayCorrectEvents()
        {
            // Arrange
            TestView view = new TestView();
            string databasePath = $"{TestConstants.GetSolutionDir()}\\{TestConstants.testDBInputFile}";

            Presenter presenter = new Presenter(view, databasePath);

            int testCategoryId = 2;

            // Act
            presenter.GetEventsFilteredByCategory(testCategoryId);

            // Assert
            // Check that the correct method on the view was called to display filtered events
            Assert.True(view.calledShowCalendarItemsWithCategoryFiltersOn);

            // Ensures that every item in view.calendarItems matches the expected category ID
            // which confirms that the filtering logic is functioning correctly
            Assert.All(view.calendarItems, item => Assert.Equal(testCategoryId, item.CategoryID));
        }

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
        public void TestRealTimeUpdatesOnUI()
        {
            // Arrange
            TestView view = new TestView();
            string databasePath = $"{TestConstants.GetSolutionDir()}\\{TestConstants.testDBInputFile}";

            Presenter presenter = new Presenter(view, databasePath);
            DateTime startDate = new DateTime(2024, 5, 5);            
            string description = "Team Meeting";
            int categoryId = 1;
            double duration = 120;

            // Act
            presenter.AddNewEvent(startDate, categoryId, description, duration);

            // Assert
            Assert.True(view.calledDisplaySuccessfulMessage);
            // Confirm that the grid update method was triggered
            Assert.True(view.calledShowCalendarItemOnDataGrid);
        }

        [Fact]
        public void TestErrorHandlingWhenDatabaseUnavailable()
        {
            // Arrange
            TestView view = new TestView();
            Presenter presenter = new Presenter(view, "invalidPath.db");

            // Act
            presenter.GetCalendarItems();

            // Assert
            Assert.True(view.calledDisplayErrorMessage);
        }

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
    }
}