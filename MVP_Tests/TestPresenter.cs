using Calendar;
using CalendarCodeTests;
using HomeCalendarGUI;
using System.Data.SQLite;

namespace MVP_Tests
{
    public class TestView : IView
    {
        public bool calledShowCategoriesOnComboBox;
        public List<Category> categories;
        
        public void ShowCategoriesOnComboBox(List<Category> categories)
        {
            calledShowCategoriesOnComboBox = true;
            this.categories = categories;
        }
    }

    public class TestPresenter()
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
    }
}