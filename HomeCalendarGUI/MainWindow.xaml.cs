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
        private readonly OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        private string saveFileToLocation;
        private string dbFile;
        private Category selectedCategoryObject;
        

        public MainWindow(bool useDefaultDb, string filePath = null)
        {
            InitializeComponent();
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog()
            {
                Filter = "Database files (*.db)|*.db|All Files|*.*",
                DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+"\\Calendar",
                RestoreDirectory = true,
                DefaultExt = "db"
            };

            saveFileToLocation = filePath;
            dbFile = System.IO.Path.GetFileName(filePath);


            if (useDefaultDb)
            {
                presenter = new Presenter(this);
            }
            else
            {
                presenter = new Presenter(this, filePath);
            }

            presenter.GetCategoriesForComboBox();
        }

        public void ShowCategoriesOnComboBox(List<Category> categories)
        {           
            const int DEFAULT = 0;                        
            categories.ForEach(c => {
                catsComboBox.Items.Add(c);                
            });
            catsComboBox.SelectedIndex = DEFAULT;
        }

        private void Btn_SaveCalendarFileTo(object sender, RoutedEventArgs e)
        {
            saveFileDialog.FileName = dbFile;
            if (saveFileDialog.ShowDialog() == true)
            {               
                saveFileToLocation = saveFileDialog.FileName;
                saveFileDialog.InitialDirectory = saveFileToLocation;
            }
        }
    }
}