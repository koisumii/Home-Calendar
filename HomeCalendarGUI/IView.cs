using Calendar;

namespace HomeCalendarGUI
{
    public interface IView
    {
        /// <summary>
        /// Displays all categories currently in the database on the drop-down button
        /// </summary>
        /// <param name="categories">A list of current categories</param>
        public void ShowCategoriesOnComboBox(List<Category> categories);

        /// <summary>
        /// Displays a message letting the user know the operation was successful.
        /// </summary>
        public void DisplayErrorMessage(string msg);

        /// <summary>
        /// Displays a message letting the user know the operation failed.
        /// </summary>
        public void DisplaySuccessfulMessage(string msg);

        /// <summary>
        /// Shows the neccessary information on a combobox for example, types of events. 
        /// </summary>
        public void ShowInformationOnCmb(List<Category> categories);

        /// <summary>
        /// Displays a list of events on the main page
        /// </summary>
        /// <param name="calendarItems">Current events from the database</param>
        public void ShowCalendarItemsOnDataGrid(List<CalendarItem> calendarItems);

        /// <summary>
        /// Displays events with specified filters
        /// </summary>
        /// <param name="calendarItems">List of events</param>
        public void ShowCalendarItemsWithDateFiltersOn(List<CalendarItem> calendarItems);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemsByMonthAndTime"></param>
        public void ShowCalendarItemsFilteredByMonth(Dictionary<string, Double> itemsByMonthAndTime);

        /// <summary>
        /// Displays a list of calendar items that have been filtered based on specific category criteria.
        /// </summary>
        void ShowCalendarItemsWithCategoryFiltersOn(List<CalendarItem> calendarItems);
    }
}
