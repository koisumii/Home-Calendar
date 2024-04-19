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
    public class Presenter
    {
        private HomeCalendar _model;
        private ViewInterface _view;
        public Presenter(ViewInterface v)
        {
            _model = new HomeCalendar();
            _view = v;
        }

        public void AddNewCategory(string desc, CategoryType type)
        {
            _model.categories.Add(desc, type);
        }

        //should call view over here 
        //public void GetCategoriesList()
        //{
        //    _model.categories.List();
        //}

        public void GetCategoriesTypeInList() 
        {
            _view.ShowInformationOnCmb(_model.categories.List());
        }

    }
}
