using Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalendarGUI
{
    public interface ViewInterface
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        void DisplayErrorMessage(string msg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        void DisplaySuccessfulMessage(string msg);

        /// <summary>
        /// 
        /// </summary>
        void ShowInformationOnCmb(List<Category> categories);

    }
}
