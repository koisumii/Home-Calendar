
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;
using static System.Net.WebRequestMethods;
﻿using Calendar;
using System.Data.Entity.Migrations.Model;
using System.Printing;
using TeamHeavyWeight_HomeCalendarApp;
using static Calendar.Category;

namespace HomeCalendarGUI
{
    public class Presenter
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

        /// <summary>
        /// Gets all categories listed in the database
        /// </summary>
        public void GetCategoriesForComboBox()
        {
            List<Category> categories = model.categories.List();
            view.ShowCategoriesOnComboBox(categories);
        }

        public void AddNewCategory(string desc, CategoryType type)
        {
            if (desc == null || type == null)
            {
                view.DisplayErrorMessage("You can not leave any empty boxes."); 
            }
            else
            {
                model.categories.Add(desc, type);
                view.DisplaySuccessfulMessage("Category has been successfully added!");
            }
            
        }


        public void GetCategoriesTypeInList() 
        {
            view.ShowInformationOnCmb(_model.categories.List());
        }


    }
}
