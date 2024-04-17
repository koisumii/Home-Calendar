using Calendar;
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
using System.IO;
using Microsoft.Win32;


namespace HomeCalendarGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IView
    {
        private readonly Presenter presenter;
        private readonly OpenFileDialog fileDialog;
        private Category selectedCategoryObject;
        

        public MainWindow(bool useDefault, string filePath = null)
        {

            InitializeComponent();
            fileDialog = new OpenFileDialog();

            if (useDefault)
            {
                presenter = new Presenter(this);
            }
            else
            {
                presenter = new Presenter(this, filePath);
            }
        }

        public void ShowCategoriesOnComboBox(List<Category> categories)
        {           
            const int DEFAULT = 0;                        
            categories.ForEach(c => {
                catsComboBox.Items.Add(c);                
            });
            catsComboBox.SelectedIndex = DEFAULT;
        }

        private void btn_Specify_File(object sender, RoutedEventArgs e)
        {
            //https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.openfiledialog?view=windowsdesktop-8.0            
            bool wait = true;


            string defaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            defaultDirectory += "\\Calendar";
            
            if (!Directory.Exists(defaultDirectory))
            {
                Directory.CreateDirectory(defaultDirectory);
                MessageBox.Show("Calendar Directory Created","Notice");
            }

            fileDialog.InitialDirectory = defaultDirectory;
            fileDialog.Filter = "Database files (*.db)|";
            fileDialog.ShowDialog(); 


        }

        private void btn_TestComboBox(object sender, RoutedEventArgs e)
        {
            Category c = new Category(catsComboBox.SelectedItem as Category);

            if(c != null)
            {
                MessageBox.Show($"Category Id: {c.Id} \n Category Description: {c.Description} \n Category Type: {c.Type}", "Selected combo box item");
            }
            else if (catsComboBox.SelectedIndex > 0)
            {
                MessageBox.Show("You did not select a category","Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Unknown error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }   
    }
}