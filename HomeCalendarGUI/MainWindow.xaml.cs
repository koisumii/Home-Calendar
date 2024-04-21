using Calendar;
using System.Data.SQLite;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Calendar.Category;

namespace HomeCalendarGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ViewInterface
    {
        private Presenter _presenter; 
        public MainWindow()
        {
            InitializeComponent();
            _presenter = new Presenter(this);
            _presenter.GetCategoriesTypeInList(); 
        }

        public void ShowInformationOnCmb(List<Category> categories)
        {
            foreach (var category in categories) 
            {   
                if (cmbEventTypes.Items.Contains(category.Type))
                {
                    //ignoring event types that have already been added because we do not want duplicates
                    continue;
                }
                cmbEventTypes.Items.Add(category.Type);
            }

        }


        private void Button_ClickAddCategory(object sender, RoutedEventArgs e)
        {
            var eventTypeChoice = cmbEventTypes.SelectedItem;
            string desc = DescriptionBox.Text;

            if (eventTypeChoice != null) 
            {
                CategoryType type = (CategoryType)eventTypeChoice;
                _presenter.AddNewCategory(desc, type);
            }
            
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

    }
}