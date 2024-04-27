using Calendar;
using CalendarCodeTests;
using HomeCalendarGUI;
using System.Data.SQLite;
using System.Reflection;
using Xunit;
using Xunit.Sdk;

namespace MVP_Tests
{
    public class TestView : IView
    {        
        public List<Category> categories;
        public List<Event> events;
        public bool calledShowCategoriesOnComboBox = false;
        public bool calledDisplayErrorMessage = false;
        public bool calledDisplaySuccessfulMessage = false;    
        public bool calledShowInfoOnCmb = false;
        public bool calledShowEventsOnDataGrid = false;
        public bool calledShowEventsWithFiltersOn = false;


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

        public void ShowEventsOnDataGrid(List<Event> events)
        {
            calledShowEventsOnDataGrid = true;
            this.events = events;
        }

        public void ShowEventsWithFiltersOn(List<Event> events)
        {
            calledShowEventsWithFiltersOn = true;
            this.events = events;
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
        public void Test_ShowEventsWithDateFiltersOn()
        {
            //Arrange
            String folder = TestConstants.GetSolutionDir();
            String existingDB = $"{folder}\\{TestConstants.testDBInputFile}";
            SQLiteConnection conn = Database.dbConnection;
            TestView view = new TestView();
            Presenter p = new Presenter(view, existingDB);
            List<Event> expectedResults = TestConstants.filteredbyYear2018();
            int expectedNumberOfItems = 3;
            DateTime? start = DateTime.Parse("January 1 2018");
            DateTime? end = DateTime.Parse("December 31 2018");


            //Act 
            p.GetEventsFilteredByDateRange(start,end);
            List<Event> results = view.events;

            //Assert

            Assert.Equal(expectedNumberOfItems, results.Count);            
            for (int i = 0; i < results.Count; i++)
            {                
                Assert.Equal(expectedResults[i].StartDateTime, results[i].StartDateTime);
                Assert.Equal(expectedResults[i].DurationInMinutes, results[i].DurationInMinutes);
                Assert.Equal(expectedResults[i].Category, results[i].Category);
                Assert.Equal(expectedResults[i].Details, results[i].Details);
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
            p.GetEvents();
            List<Event> initial = view.events;
            Event eveToDelete = initial.Where(e => e.Id == eveIdToDelete).First();
            p.DeleteAnEvent(eveToDelete);
            List<Event> results = view.events;

            //Assert
            Assert.True(results.Count < initial.Count);
            Assert.Throws<InvalidOperationException>(() => { p.DeleteAnEvent(eveToDelete); });
        }
    }
}