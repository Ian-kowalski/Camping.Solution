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

namespace Camping.WPF
{
    /// <summary>
    /// Interaction logic for Filterdialog.xaml
    /// </summary>
    public partial class Filterdialog : Window
    {
        public Filterdialog()
        {
            InitializeComponent();
        }



        private void zoeken(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this.Title);

        }

        private void DateChanged(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as DatePicker;
            DateTime? date = picker.SelectedDate;

            if (date == null)
            {
                this.Title = "No date";
            }
            else
            {
                this.Title = date.Value.ToShortDateString();
            }
        }
    }
}
