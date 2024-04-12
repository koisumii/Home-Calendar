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
        /// Initiates default settings
        /// </summary>
        /// <param name="v">IView interface</param>
        /// <param name="dbFilePath"></param>
        public Presenter(IView v) 
        {
            model = new HomeCalendar();
            view = v;            
        }

        public Presenter(IView v,string dbFile)
        {
            model = new HomeCalendar();
            view = v;
        }
    }
}
