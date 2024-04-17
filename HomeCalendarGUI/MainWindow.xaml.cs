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
    public partial class MainWindow : Window
    {
        private Presenter _presenter; 
        public MainWindow()
        {
            InitializeComponent();
            ShowEventsTypeOnCmb();
            _presenter = new Presenter();
            //Events events = new Events();
            //events.Show();
            //this.Hide();
        }

        public void ShowEventsTypeOnCmb()
        {
            cmbEventTypes.Items.Add(Category.CategoryType.Event);
            cmbEventTypes.Items.Add(Category.CategoryType.AllDayEvent);
            cmbEventTypes.Items.Add(Category.CategoryType.Holiday);
            cmbEventTypes.Items.Add(Category.CategoryType.Availability);
        }


        private void Button_ClickAddCategory(object sender, RoutedEventArgs e)
        {
            var eventTypeChoice = cmbEventTypes.SelectedItem;
            string desc = DescriptionBox.Text;

            if (eventTypeChoice != null || desc != null) 
            {
                CategoryType type = (CategoryType)eventTypeChoice;
                _presenter.AddNewCategory(desc, type);
                DisplaySuccessfulMessage("Category has been successfully added!");
            }
            else
            {
                DisplayErrorMessage("You can not leave any empty boxes."); 
            }
            
        }

        public Category.CategoryType GetEnumFromString(string str)
        {
            foreach (CategoryType types in Enum.GetValues(typeof(Category.CategoryType))) 
            { 
                if(types.ToString() == str)
                {
                    return types; 
                }
            }

            // safety net 
            return CategoryType.Event; 
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