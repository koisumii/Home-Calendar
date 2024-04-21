using Calendar;
using HomeCalendarGUI;

namespace MVP_Tests
{
    public class TestView : IView
    {
        public bool calledShowCategoriesOnComboBox;
        public List<Category> categories;
        
        public void ShowCategoriesOnComboBox(List<Category> categories)
        {
            calledShowCategoriesOnComboBox = true;
        }
    }

    public class TestPresenter()
    {
        
    }
}