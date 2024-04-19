using Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HomeCalendarGUI
{
    /// <summary>
    /// Interaction logic for Events.xaml
    /// </summary>
    public partial class Events : Window, ViewInterface
    {
        private Presenter _presenter;
        public Events()
        {
            _presenter = new Presenter(this); 
            InitializeComponent();

            //also put this in presenter
            SetTodaysDateOnDatePicker();


            _presenter.GetCategoriesTypeInList();
        }

        public void DisplayErrorMessage(string msg)
        {
            throw new NotImplementedException();
        }

        public void DisplaySuccessfulMessage(string msg)
        {
            throw new NotImplementedException();
        }

        //this should be in view 
        public void SetTodaysDateOnDatePicker() 
        { 
            StartDate.DisplayDateStart = DateTime.Now;
            

            EndDate.DisplayDateStart = DateTime.Now;
        }

        public void ShowInformationOnCmb(List<Category> categories)
        {
            //List<Category> categories = _presenter.GetCategoriesList();
            for (int i = 0; i < categories.Count; i++)
            {
                cmbCategory.Items.Add(categories[i]);
            }
        }
    }
}
