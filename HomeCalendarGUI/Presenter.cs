using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;
using static System.Net.WebRequestMethods;

namespace HomeCalendarGUI
{
    internal class Presenter
    {
        private readonly HomeCalendar model;
        private readonly IView view;
        

        /// <summary>
        /// Initiates presenter with default settings
        /// </summary>
        /// <param name="v">IView interface implemented class</param>
        public Presenter(IView v) 
        {
            model = new HomeCalendar();            
            view = v;            
        }

        /// <summary>
        /// Instantiates presenter with an existing database
        /// </summary>
        /// <param name="v">IView interface implemented class</param>
        /// <param name="dbFile">File path to the database</param>
        public Presenter(IView v,string dbFile)
        {
            model = new HomeCalendar(dbFile,false);
            view = v;
        }

        public void GetCategoriesForComboBox()
        {
            List<Category> categories = model.categories.List();
            view.ShowCategoriesOnComboBox(categories);
        }
    }
}
