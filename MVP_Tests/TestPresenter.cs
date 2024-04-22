using Calendar;
using CalendarCodeTests;
using HomeCalendarGUI;
using System.Data.SQLite;
using Xunit;

namespace MVP_Tests
{
    public class TestView : IView
    {
        public bool calledShowCategoriesOnComboBox = false;
        public List<Category> categories;
        public bool calledDisplayErrorMessage = false;
        public bool calledDisplaySuccessfulMessage = false;    
        public bool calledShowInfoOnCmb = false;
        
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
    }
}