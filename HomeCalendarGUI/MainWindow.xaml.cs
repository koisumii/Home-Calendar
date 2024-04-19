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
using System.Data.Entity.Core.Objects;
using System.Xml.Linq;


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
        private OpenFolderDialog openFolderDialog;
        private string fileDirectoryToStore;
        private string currentDbFileLocation;
        private string dbFileName;
        private Category selectedCategoryObject;
        

        public MainWindow(bool useDefaultDb, string filePath = null)
        {
            InitializeComponent();
            openFileDialog = new OpenFileDialog();

            //Create Calendar directory if it doesn't exist
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Calendar"))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Calendar");
            }

            //Db file information
            currentDbFileLocation = filePath;
            dbFileName = System.IO.Path.GetFileName(filePath);
            
            //Save file dialog properties
            saveFileDialog = new SaveFileDialog()
            {
                Filter = "Database files (*.db)|*.db|All Files|*.*",
                DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Calendar",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Calendar",
                RestoreDirectory = true,
                DefaultExt = "db",
                FileName = dbFileName
            };

            openFolderDialog = new OpenFolderDialog
            {
                DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Calendar",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Calendar",
            };

            //Validate if user is using the default database or specified database
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
            if (openFolderDialog.ShowDialog() == true)
            {
                fileDirectoryToStore = openFolderDialog.FolderName;
                openFolderDialog.InitialDirectory = fileDirectoryToStore;
                openFolderDialog.FolderName = System.IO.Path.GetDirectoryName(fileDirectoryToStore);                                             
            }


            //if (saveFileDialog.ShowDialog() == true)
            //{

            //    //If the file exists at specified directory, replace it, otherwise move the file
            //    if (!File.Exists(saveFileDialog.FileName))
            //    {
            //        Database.CloseDatabaseAndReleaseFile();
            //        File.Move(currentDbFileLocation, saveFileDialog.FileName);
            //        Database.existingDatabase(saveFileDialog.FileName);
            //    }
            //    else
            //    {
            //        Database.CloseDatabaseAndReleaseFile();
                    
            //        //Replace operation in 4 steps
            //        File.Copy(currentDbFileLocation, saveFileDialog.FileName +"- 1");                   
            //        File.Delete(saveFileDialog.FileName);
            //        File.Copy(saveFileDialog.FileName + "- 1", saveFileDialog.FileName);
            //        File.Delete(saveFileDialog.FileName + "- 1");
            //        Database.existingDatabase(saveFileDialog.FileName);
            //    }
            //    RefreshMainView();  
            //    currentDbFileLocation = saveFileDialog.FileName;
            //    saveFileDialog.InitialDirectory = saveFileDialog.FileName;
            //    saveFileDialog.FileName = dbFileName;
            //}
        }

        private void RefreshMainView()
        {
            presenter.GetCategoriesForComboBox();
        }
    }
}