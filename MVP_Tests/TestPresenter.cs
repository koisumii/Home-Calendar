using Calendar;
using HomeCalendarGUI;
namespace MVP_Tests
{
    public class TestView : ViewInterface
    {
        bool calledDisplayErrorMessage = false;
        bool calledDisplaySuccessfulMessage = false;    
        bool calledShowInfoOnCmb = false;

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
        public void TestAddNewCategory_Fail()
        {
            TestView view = new TestView(); 
            Presenter presenter = new Presenter(view);
        }
    }
}