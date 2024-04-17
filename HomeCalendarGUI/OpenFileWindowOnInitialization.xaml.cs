using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for OpenFileWindowOnInitialization.xaml
    /// </summary>
    public partial class OpenFileWindowOnInitialization : Window
    {
        private MainWindow main;
        private OpenFileDialog fileDialog;
        public OpenFileWindowOnInitialization()
        {
            InitializeComponent();  
               
        }

        private void Btn_Use_Default_File(object sender, RoutedEventArgs e)
        {
            main = new MainWindow(true);
            main.Show();
            Close();
        }

        private void Btn_Use_Specific_File(object sender, RoutedEventArgs e)
        {
            //https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.openfiledialog?view=windowsdesktop-8.0            
            //https://www.c-sharpcorner.com/UploadFile/mahesh/openfiledialog-in-C-Sharp/


            string defaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            defaultDirectory += "\\Calendar";

            if (!Directory.Exists(defaultDirectory))
            {
                Directory.CreateDirectory(defaultDirectory);
                MessageBox.Show("Calendar Directory Created", "Notice");
            }
            
            fileDialog = new OpenFileDialog()
            {
                InitialDirectory = defaultDirectory,
                Filter = "Database files (*.db)|",
                CheckFileExists = true,
                CheckPathExists = true,      
                RestoreDirectory = true,
                DefaultExt = "db"
            };           
            
           if(fileDialog.ShowDialog() == DialogResult)
           {

           }

        }
    }
}
