using Calendar;

namespace HomeCalendarGUI
{
    public interface IView
    {
        /// <summary>
        /// Displays all categories currently in the database on the drop-down button
        /// </summary>
        /// <param name="categories">A list of current categories</param>
        public void PopulateAllCategoriesComboBox(List<Category> categories);

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

        public void ShowTotalBusyTimeByMonth(List<CalendarItemsByMonth> itemsByMonth);

        public void ShowTotalBusyTimeByMonthAndCategory(List<Dictionary<string, object>> itemsByCategoryAndMonth);

        public void ShowTotalBusyTimeByCategory(List<CalendarItemsByCategory> itemsByCategory);
    }
}
