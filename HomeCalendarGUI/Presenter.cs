using Calendar;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using TeamHeavyWeight_HomeCalendarApp;
using static Calendar.Category;

namespace HomeCalendarGUI
{
    internal class Presenter
    {
        private HomeCalendar _model; 
        public Presenter()
        {
            _model = new HomeCalendar();
        }

        public void AddNewCategory(string desc, CategoryType type)
        {
            _model.categories.Add(desc, type);
        }
    }
}
