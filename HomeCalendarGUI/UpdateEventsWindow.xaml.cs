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
using Xceed.Wpf.Toolkit;

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
            //_presenter = new Presenter(this);
            InitializeComponent();

            CreateTimePicker();
        }

        /// <summary>
        /// Initializes the timepicker and adds it to the window
        /// </summary>
        public void CreateTimePicker()
        {
            TimePicker startTimePicker = new TimePicker();
            startTimePicker.AllowTextInput = false;
            startTimePicker.Name = "startTime";
            startTimePicker.Margin = new Thickness(0, 5, 0, 0);

            StartTimeGrid.Children.Add(startTimePicker);

            TimePicker EndTimePicker = new TimePicker();
            EndTimePicker.AllowTextInput = false;
            EndTimePicker.Name = "endTime";
            EndTimePicker.Margin = new Thickness(0, 5, 0, 0);

            EndTimeGrid.Children.Add(EndTimePicker);
        }

        private void Btn_UpdateEvent(object sender, RoutedEventArgs e)
        {
            

        }
    }
}
