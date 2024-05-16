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
    /// Interaction logic for UpdateEventsWindow.xaml
    /// </summary>
    public partial class UpdateEventsWindow : Window
    {
        private Presenter _presenter;
        public UpdateEventsWindow()
        {
            _presenter = new Presenter(this);
            InitializeComponent();
        }

        private void Btn_UpdateEvent(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
