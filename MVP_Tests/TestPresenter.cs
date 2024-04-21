using Calendar;
using HomeCalendarGUI;
using Xunit;
namespace MVP_Tests
{
    public class TestView : ViewInterface
    {
        public bool calledDisplayErrorMessage = false;
        public bool calledDisplaySuccessfulMessage = false;    
        public bool calledShowInfoOnCmb = false;

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
        public void TestShowInfoOnCmb()
        {
            //arrange
            TestView view = new TestView(); 

            //act
            Presenter presenter = new Presenter(view);

            //assert
            Assert.True(view.calledShowInfoOnCmb); 
        }

        [Fact]
        public void TestDisplayErrorMessage()
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
        public void TestDisplaySuccessfulMessage() 
        {
            //arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);

            //act
            p.AddNewCategory("description", Category.CategoryType.Holiday); 

            //assert
            Assert.True(view.calledDisplaySuccessfulMessage);
        }
    }
}