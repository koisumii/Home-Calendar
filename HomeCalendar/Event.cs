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
    // CLASS: Event
    //        - An individual event for calendar program
    // ====================================================================
    /// <summary>
    /// An occurance that will happen marked in the calendar. 
    /// </summary>
    public class Event
    {
        // ====================================================================
        // Properties
        // ====================================================================

        /// <summary>
        /// Gets the unique identifier for an activity.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the beginning of the activity.
        /// </summary>
        public DateTime StartDateTime { get; }

        /// <summary>
        /// Gets/sets the time that was set aside for this activity in minutes. 
        /// </summary>
        public Double DurationInMinutes { get; set; }

        /// <summary>
        /// Gets/sets the description of this activity.
        /// </summary>
        public String Details { get; set; }

        /// <summary>
        /// Gets/sets the identifier of the activitie's category. 
        /// </summary>
        public int Category { get; set; }

        // ====================================================================
        // Constructor
        //    NB: there is no verification the event category exists in the
        //        categories object
        // ====================================================================

        /// <summary>
        /// Initializes the created object with specific values. 
        /// </summary>
        /// <param name="id"> An unique number that represents an event. </param>
        /// <param name="date"> The beginning of the event. </param>
        /// <param name="category"> A name that is assocaited with a type of activity </param>
        /// <param name="duration"> The time that has been reserved for this activity. </param>
        /// <param name="details"> A description of the activity. </param>
        public Event(int id, DateTime date, int category, Double duration, String details)
        {
            this.Id = id;
            this.StartDateTime = date;
            this.Category = category;
            this.DurationInMinutes = duration;
            this.Details = details;
        }

        // ====================================================================
        // Copy constructor - does a deep copy
        // ====================================================================

        /// <summary>
        /// Initializes the created object with a especified object. 
        /// </summary>
        /// <param name="obj"> An ob ject of type Event ?? </param>
        public Event(Event obj)
        {
            this.Id = obj.Id;
            this.StartDateTime = obj.StartDateTime;
            this.Category = obj.Category;
            this.DurationInMinutes = obj.DurationInMinutes;
            this.Details = obj.Details;

        }
    }
}
