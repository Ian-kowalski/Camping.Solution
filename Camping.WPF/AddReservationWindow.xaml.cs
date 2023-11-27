using camping.Database;
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
    /// Interaction logic for AddReservation.xaml
    /// </summary>
    public partial class AddReservationWindow : Window
    {

        public int CampSiteID;
        public string StartDate;
        public string EndDate;

        public AddReservationWindow(int campSiteID, string startDate, string endDate) {
            InitializeComponent();

            CampSiteID = campSiteID;
            StartDate = startDate;
            EndDate = endDate;

            campSiteIDLabel.Content = CampSiteID.ToString();
            startDateLabel.Content = StartDate.ToString();
            endDateLabel.Content = EndDate.ToString();

        }

        private void AddReservationButton_Click(object sender, RoutedEventArgs e)
        {
            ReservationData resData = new();

            int houseNumber = 0;
            try { int.TryParse(HouseNumberBox.Text, out houseNumber); }
            catch (FormatException) { MessageBox.Show("Het huisnummer is geen nummer!"); }

            int phoneNumber = 0;
            try { int.TryParse(PhoneNumberBox.Text, out phoneNumber); }
            catch (FormatException) { MessageBox.Show("Het telefoonnummer is geen nummer!"); }

            if (resData.addReservation(CampSiteID, StartDate, EndDate,
                NameBox.Text, PrepositionBox.Text, SurnameBox.Text,
                AdressBox.Text, CityBox.Text, PostalcodeBox.Text,
                houseNumber, phoneNumber))
            {
                MessageBox.Show("Reservering is toegevoegd!");
                Close();
            }
            else {
                MessageBox.Show("Reservering kon niet worden toegevoegd. Incorrecte veldwaarde?");
            }
        }
    }
}
