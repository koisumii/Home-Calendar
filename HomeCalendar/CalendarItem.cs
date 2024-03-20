using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{
    // ====================================================================
    // CLASS: CalendarItem
    //        A single calendar item, includes a Category and an Event
    // ====================================================================

    /// <summary>
    /// A single item marked in your calendar, includes a category and an event. 
    /// </summary>
    public class CalendarItem
    {
        /// <summary>
        /// Gets/sets the category ID of the calendar item.  
        /// </summary>
        /// <returns> An integer number that represents the CategoryID. </returns>
        public int CategoryID { get; set; }

        /// <summary>
        /// Gets/sets the event ID of the calendar item.  
        /// </summary>
        /// <returns> An integer number that represents the EventID. </returns>
        public int EventID { get; set; }

        /// <summary>
        /// Gets/sets the beginning date & time of the calendar item.   
        /// </summary>
        /// <returns> The chronology indicator of the beginning of the event. </returns>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// Gets/sets the name of the category such as work, vacation, fun...
        /// </summary>
        /// <returns> A string that represents the title of the category. </returns>
        public String? Category { get; set; }

        /// <summary>
        /// Gets/sets the explanation of the event (optional). 
        /// </summary>
        /// <returns> A string that espicifies details about the event. </returns>
        public String? ShortDescription { get; set; }

        /// <summary>
        /// Gets/sets how long the event will take (in minutes). 
        /// </summary>
        /// <returns> A double number that represents the full length of the event. </returns>
        public Double DurationInMinutes { get; set; }

        /// <summary>
        /// Gets/sets the sum of all the time that has been set aside for an item. 
        /// </summary>
        /// <returns> A double number that represents the whole time you were not available. </returns>
        public Double BusyTime { get; set; }

    }

    /// <summary>
    /// Many items (of the same moth) marked in your calendar, includes a category and an event. 
    /// </summary>
    public class CalendarItemsByMonth
    {
        /// <summary>
        /// Gets/sets the month the item took place in.
        /// </summary>
        /// <returns> A string that represents the name of the month. </returns>
        public String? Month { get; set; }

        /// <summary>
        /// Gets/sets a list of all items (of type CalendarItem) for a specific month.
        /// </summary>
        /// <returns> A list of type CalendarItem that holds all items for their specific month.</returns>
        public List<CalendarItem>? Items { get; set; }

        /// <summary>
        /// Gets/sets sum of all the time that has been set aside for a specific month.
        /// </summary>
        /// <returns> A double number that represents TotalBusyTime.</returns>
        public Double TotalBusyTime { get; set; }
    }

    /// <summary>
    /// Many items (of the same category) marked in your calendar, includes a category and an event 
    /// </summary>
    public class CalendarItemsByCategory
    {
        /// <summary>
        /// Gets/sets the name of the category of a calendar item. 
        /// </summary>
        /// <returns> a string that represent the name of the category. </returns>
        public String? Category { get; set; }

        /// <summary>
        /// Gets/sets a list of items (of type CalendarItem) for a specific category. 
        /// </summary>
        /// <returns> a list of type CalendarItem that holds all items of the same category. </returns>
        public List<CalendarItem>? Items { get; set; }

        /// <summary>
        /// Gets/sets all time that has been set aside for a specific category.
        /// </summary>
        /// <returns> a double number that represents the total of all the time that has been set aside in my calendar.</returns>
        public Double TotalBusyTime { get; set; }

    }


}
