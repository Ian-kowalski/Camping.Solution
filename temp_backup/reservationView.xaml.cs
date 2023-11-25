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
    /// Interaction logic for reservationView.xaml
    /// </summary>
    public partial class reservationView : Window
    {
        public reservationView()
        {
            InitializeComponent();
        }



        private void FilterClick(object sender, RoutedEventArgs e)
        { 
            Filterdialog dlg = new Filterdialog();
            dlg.ShowDialog();
        }
    }
}
