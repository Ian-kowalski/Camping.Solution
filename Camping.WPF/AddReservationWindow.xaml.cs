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
            if (!int.TryParse(HouseNumberBox.Text, out houseNumber)) {
                MessageBox.Show("Het huisnummer is geen nummer!");
                return;
            }
            if (houseNumber < 0 || houseNumber > 9999) {
                MessageBox.Show("Het huisnummer moet tussen 0 en 9999 zijn!");
                return;
            }

            int phoneNumber = 0;
            if (!int.TryParse(PhoneNumberBox.Text, out phoneNumber))
            {
                MessageBox.Show("Het telefoonnummer is geen nummer!");
                return;
            }

            string postalCode = PostalcodeBox.Text;
            if (postalCode.Length > 6)
            {
                MessageBox.Show("De postcode mag niet langer zijn dan 6 tekens!");
                return;
            }

            string preposition = PrepositionBox.Text;
            if (preposition.Length > 10)
            {
                MessageBox.Show("De tussenvoegsel mag niet langer zijn dan 10 tekens!");
                return;
            }


            if (resData.addReservation(CampSiteID, StartDate, EndDate,
                NameBox.Text, PrepositionBox.Text, SurnameBox.Text,
                AdressBox.Text, CityBox.Text, postalCode,
                houseNumber, phoneNumber))
            {
                MessageBox.Show("Reservering is toegevoegd!");
                ReservationAdded?.Invoke(this, new EventArgs());
                Close();
            }
            else {
                MessageBox.Show("Reservering kon niet worden toegevoegd. Incorrecte veldwaarde?");
            }
        }

        public event EventHandler<EventArgs> ReservationAdded;
    }
}
