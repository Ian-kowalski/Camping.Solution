using camping.Core;
using camping.Database;
using DevExpress.Utils.CommonDialogs.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        List<Reservation> res;
        int index = 4; /// index van de gekozen reservering meegeven vanaf het reserveringen scherm
        public ChangeReservation()
        {
            InitializeComponent();

            reservationData = new();
            siteData = new();

            retrieveData = new(siteData, reservationData);
            res = reservationData.GetReservationInfo();


            FirstName.Text = Convert.ToString(res.ElementAt(index).Guest.FirstName);
            Preposition.Text = Convert.ToString(res.ElementAt(index).Guest.Preposition);
            LastName.Text = Convert.ToString(res.ElementAt(index).Guest.LastName);
            City.Text = Convert.ToString(res.ElementAt(index).Guest.City);
            Adress.Text = Convert.ToString(res.ElementAt(index).Guest.Adress);
            Phonenumber.Text = Convert.ToString(res.ElementAt(index).Guest.PhoneNumber);
            HouseNumber.Text = Convert.ToString(res.ElementAt(index).Guest.HouseNumber);
            PostalCode.Text = Convert.ToString(res.ElementAt(index).Guest.PostalCode);
            StartDate.Text = Convert.ToString(res.ElementAt(index).StartDate);
            EndDate.Text = Convert.ToString(res.ElementAt(index).EndDate);
            
            ReservationID.Content = Convert.ToString(res.ElementAt(index).ReservationID);
        }

        private void Aanpassen_Click(object sender, RoutedEventArgs e)
        {
            ///Als de messageBox niet werkt zorg er dan voor dat de 'DevExpress.Data' nuGet package is geinstalleerd 
        
            var result = MessageBox.Show("weet je zeker dat je de reservatie gegevens wilt aanpassen?", "Confirm", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                res.ElementAt(index).Guest.FirstName = FirstName.Text;
                res.ElementAt(index).Guest.Preposition = Preposition.Text;
                res.ElementAt(index).Guest.LastName = LastName.Text;
                res.ElementAt(index).Guest.City = City.Text;
                res.ElementAt(index).Guest.Adress = Adress.Text;
                res.ElementAt(index).Guest.PhoneNumber = Int32.Parse(Phonenumber.Text);
                res.ElementAt(index).Guest.HouseNumber = Int32.Parse(HouseNumber.Text);
                res.ElementAt(index).Guest.PostalCode = PostalCode.Text;

                res.ElementAt(index).StartDate = Convert.ToDateTime(StartDate.Text);
                res.ElementAt(index).EndDate = Convert.ToDateTime(EndDate.Text);

                retrieveData.UpdateReservation(res.ElementAt(index).ReservationID, res.ElementAt(index).StartDate, res.ElementAt(index).Guest, res.ElementAt(index).EndDate);
            }
        }
    }
}
