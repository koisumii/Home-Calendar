using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;

namespace HomeCalendarGUI
{
    internal class Presenter
    {
        private readonly HomeCalendar model;
        private IView view;

        public Presenter(IView v) 
        {
            model = new HomeCalendar();
            view = v;
        }

    }
}
