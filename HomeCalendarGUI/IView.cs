using Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void DisplayErrorMessage(string msg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void DisplaySuccessfulMessage(string msg);

        /// <summary>
        /// 
        /// </summary>
        public void ShowInformationOnCmb(List<Category> categories);
    }
}
