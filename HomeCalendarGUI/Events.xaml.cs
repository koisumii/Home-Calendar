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
    public partial class Events : Window
    {
        public Events()
        {
            InitializeComponent();
            ShowCategoriesOnCmb();
        }

        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        public void ShowCategoriesOnCmb()
        {
            HomeCalendar h1 = new HomeCalendar();
            List<Category> categories = h1.categories.List();

            for (int i = 0; i < categories.Count; i++)
            {
                cmbCategory.Items.Add(categories[i]);
            }
        }
    }
}
