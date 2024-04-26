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
    public partial class Events : Window, IView
    {
        private Presenter _presenter;
        public Events()
        {
            _presenter = new Presenter(this); 
            InitializeComponent();

            //also put this in presenter
            ShowTodaysDate();


            _presenter.GetCategoriesTypeInList();
        }

        public void DisplayErrorMessage(string msg)
        {
            message.Foreground = Brushes.Red;
            message.Text = msg;
        }

        public void DisplaySuccessfulMessage(string msg)
        {
            message.Foreground = Brushes.Green;
            message.Text = msg;
        }

        public void ShowTodaysDate() 
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

        public void ShowCategoriesOnComboBox(List<Category> categories)
        {
            throw new NotImplementedException();
        }
    }
}
