using Calendar;

namespace HomeCalendarGUI
{
    public interface IView
    {
        /// <summary>
        /// Displays all categories currently in the database on the drop-down button
        /// </summary>
        /// <param name="categories">A list of current categories</param>
        public void PopulateCategoriesInAllCatsComboBox(List<Category> categories);

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
        /// Displays calendar items on datagrid
        /// </summary>
        /// <param name="items">Calendar Items from the database</param>
        public void ShowCalendarItems(List<CalendarItem> items);

        /// <summary>
        /// Displays the total busy times of all calendar items by month
        /// </summary>
        /// <param name="itemsByMonth">Calendar Items from the database</param>
        public void ShowTotalBusyTimeByMonth(List<CalendarItemsByMonth> itemsByMonth);

        /// <summary>
        /// Displays the total busy time of all calendar items by category
        /// </summary>
        /// <param name="itemsByCategory"></param>
        public void ShowTotalBusyTimeByCategory(List<CalendarItemsByCategory> itemsByCategory);
        
        /// <summary>
        /// Displays the total busy times of all calendar items by category and month
        /// </summary>
        /// <param name="itemsByCategoryAndMonth">Calendar items from the database</param>
        public void ShowTotalBusyTimeByMonthAndCategory(List<Dictionary<string, object>> itemsByCategoryAndMonth);
    }
}
