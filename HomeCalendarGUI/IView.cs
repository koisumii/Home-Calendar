using Calendar;

namespace HomeCalendarGUI
{
    public interface IView
    {
        /// <summary>
        /// Displays all categories currently in the database on the drop-down button
        /// </summary>
        /// <param name="categories">A list of current categories</param>
        public void PopulateCategoriesComboBox(List<Category> categories);

        /// <summary>
        /// Displays a message letting the user know the operation failed.
        /// </summary>
        public void DisplayErrorMessage(string msg);

        /// <summary>
        /// Displays a message letting the user know the operation was successful.
        /// </summary>
        public void DisplaySuccessfulMessage(string msg);

        /// <summary>
        /// Populates category types in combobox
        /// </summary>
        public void PopulateCategoryTypesComboBox(List<Category> categories);

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
        public void ShowCalendarItemsWithCategoryFiltersOn(List<CalendarItem> calendarItems);

        /// <summary>
        /// Displays calendar items on datagrid
        /// </summary>
        /// <param name="items">Calendar Items from the database</param>
        public void ShowCalendarItems(List<CalendarItem> items);

        public void ShowTotalBusyTimeByMonth(List<CalendarItemsByMonth> itemsByMonth);

        public void ShowCalendarItemsByACategory(List<CalendarItemsByCategory> itemsByCategory);

        public void ShowTotalBusyTimeByMonthAndCategory(List<Dictionary<string, object>> itemsByCategoryAndMonth);

        public void ShowTotalBusyTimeByCategory(List<CalendarItemsByCategory> itemsByCategory);
    }
}
