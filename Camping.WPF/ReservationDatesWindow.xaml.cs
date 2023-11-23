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

namespace camping.WPF
{
    /// <summary>
    /// Interaction logic for ReservationDatesWindow.xaml
    /// </summary>
    public partial class ReservationDatesWindow : Window
    {
        public ReservationDatesWindow()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ViewAvailableReservations var = new(StartDateButton.DisplayDate.Date, EndDateButton.DisplayDate.Date);
            var.ShowDialog();
        }
    }
}
