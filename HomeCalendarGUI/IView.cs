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
        /// <param name="events">Current events from the database</param>
        public void ShowEventsOnDataGrid(List<Event> events);

        /// <summary>
        /// Displays events with specified filters
        /// </summary>
        /// <param name="events">List of events</param>
        public void ShowEventsWithFiltersOn(List<Event> events);
    }
}
