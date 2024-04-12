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
    public partial class MainWindow : Window,IView
    {
        private readonly Presenter presenter;
        private readonly OpenFileDialog fileDialog;        

        public MainWindow()
        {
            InitializeComponent();
            fileDialog = new OpenFileDialog();
            presenter = new Presenter(this); 
        }

        public void ShowOpenFileDialog()
        {
            //https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.openfiledialog?view=windowsdesktop-8.0
            fileDialog.InitialDirectory = System.IO.Path.GetDirectoryName("Documents");
            fileDialog.Filter = "All files (*.*)|";
            fileDialog.ShowDialog();            
        }
    }
}