using camping.Core;
using camping.Database;
using DevExpress.Utils.CommonDialogs.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
    /// Interaction logic for ChangeReservation.xaml
    /// </summary>
    public partial class ChangeReservation : Window
    {
        RetrieveData retrieveData;
        ReservationData reservationData;
        SiteData siteData;
        Reservation res;

        public ChangeReservation(Reservation reservation)
        {
            InitializeComponent();

            reservationData = new();
            siteData = new();

            retrieveData = new(siteData, reservationData);

            res = reservation;

            FirstName.Text = Convert.ToString(reservation.Guest.FirstName);
            Preposition.Text = Convert.ToString(reservation.Guest.Preposition);
            LastName.Text = Convert.ToString(reservation.Guest.LastName);
            City.Text = Convert.ToString(reservation.Guest.City);
            Adress.Text = Convert.ToString(reservation.Guest.Adress);
            Phonenumber.Text = Convert.ToString(reservation.Guest.PhoneNumber);
            HouseNumber.Text = Convert.ToString(reservation.Guest.HouseNumber);
            PostalCode.Text = Convert.ToString( reservation.Guest.PostalCode);
            StartDate.Text = Convert.ToString(reservation.StartDate);
            EndDate.Text = Convert.ToString(reservation.EndDate);
            
            ReservationID.Content = Convert.ToString(reservation.ReservationID);
        }

        private void Aanpassen_Click(object sender, RoutedEventArgs e)
        {
            ///Als de messageBox niet werkt zorg er dan voor dat de 'DevExpress.Data' nuGet package is geinstalleerd 
        
            var result = MessageBox.Show("Weet je zeker dat je de reservatie gegevens wilt aanpassen?", "Confirm", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    res.Guest.PhoneNumber = Int32.Parse(Phonenumber.Text);
                }
                catch
                {
                    MessageBox.Show("Verkeerde waarde ingevuld bij 'Telefoonnummer'.\nMoet een getal zijn");
                    return;
                }
                try
                {
                    res.Guest.PhoneNumber = Int32.Parse(HouseNumber.Text);
                }
                catch
                {
                    MessageBox.Show("Verkeerde waarde ingevuld bij 'Huisnummer'.\nMoet een getal zijn");
                    return;
                }

                Regex regex = new("[1-9][0-9]{3}[A-Z]{2}");
                if (regex.IsMatch(PostalCode.Text) && PostalCode.Text.Length <= 6)
                {
                    res.Guest.PostalCode = PostalCode.Text;
                }
                else
                {
                    MessageBox.Show("Onjuiste formaat bij postcode ingevuld. Moet vier getallen en twee letters zijn, bijv: '1234AB'");
                    return;
                }

                res.Guest.FirstName = FirstName.Text;
                res.Guest.Preposition = Preposition.Text;
                res.Guest.LastName = LastName.Text;
                res.Guest.City = City.Text;
                res.Guest.Adress = Adress.Text;


                if (Convert.ToDateTime(EndDate.Text) < DateTime.Today)
                {
                    MessageBox.Show("Einddatum kan niet in het verleden zijn");
                    return;
                } else if (Convert.ToDateTime(StartDate.Text) <= Convert.ToDateTime(EndDate.Text))
                {
                    res.StartDate = Convert.ToDateTime(StartDate.Text);
                    res.EndDate = Convert.ToDateTime(EndDate.Text);
                } else
                {
                    MessageBox.Show("Begindatum kan niet voor de einddatum komen");
                    return;
                }

                retrieveData.UpdateReservation(res.ReservationID, res.StartDate, res.Guest, res.EndDate);
            }
        }
    }
}
