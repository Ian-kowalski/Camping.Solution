using camping.Core;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace camping.WPF
{
    public class ChangeReservation
    {
        private RetrieveData retrieveData;

        public bool ReservationAanpassenButtonState { get; set; } //true save : false aanpassen

        private TextBox SiteIDBox;
        private DatePicker StartDateDatePicker;
        private DatePicker EndDateDatePicker;
        private TextBox FirstNameBox;
        private TextBox PrepositionBox;
        private TextBox LastNameBox;
        private TextBox PhoneNumberBox;
        private TextBox CityBox;
        private TextBox StreetBox;
        private TextBox HouseNumberBox;
        private TextBox PostalCodeBox;

        private Label SiteIDLabel;
        private Label StartDateLabel;
        private Label EndDateLabel;
        private Label FirstNameLabel;
        private Label LastNameLabel;
        private Label PhoneNumberLabel;
        private Label CityLabel;
        private Label StreetLabel;
        private Label HouseNumberLabel;
        private Label PostalCodeLabel;

        private Button EditReservationButton;

        public ChangeReservation(RetrieveData retrieve, TextBox siteIDBox, DatePicker startDateDatePicker, DatePicker endDateDatePicker, TextBox firstNameBox, TextBox prepositionBox, TextBox lastNameBox, TextBox phoneNumberBox, TextBox cityBox, TextBox streetBox, TextBox houseNumberBox, TextBox postalCodeBox, Label siteIDLabel, Label startDateLabel, Label endDateLabel, Label firstNameLabel, Label lastNameLabel, Label phoneNumberLabel, Label cityLabel, Label streetLabel, Label houseNumberLabel, Label postalCodeLabel, Button editReservationButton)
        {
            retrieveData = retrieve;

            SiteIDBox = siteIDBox;
            StartDateDatePicker = startDateDatePicker;
            EndDateDatePicker = endDateDatePicker;
            FirstNameBox = firstNameBox;
            PrepositionBox = prepositionBox;
            LastNameBox = lastNameBox;
            PhoneNumberBox = phoneNumberBox;
            CityBox = cityBox;
            StreetBox = streetBox;
            HouseNumberBox = houseNumberBox;
            PostalCodeBox = postalCodeBox;

            SiteIDLabel = siteIDLabel;
            StartDateLabel = startDateLabel;
            EndDateLabel = endDateLabel;
            FirstNameLabel = firstNameLabel;
            LastNameLabel = lastNameLabel;
            PhoneNumberLabel = phoneNumberLabel;
            CityLabel = cityLabel;
            StreetLabel = streetLabel;
            HouseNumberLabel = houseNumberLabel;
            PostalCodeLabel = postalCodeLabel;

            EditReservationButton = editReservationButton;
        }

        public void fillReservationInfoGrid(Reservation reservation)
        {
            SiteIDBox.Text = reservation.SiteID.ToString();
            StartDateDatePicker.Text = reservation.StartDate.ToShortDateString();
            EndDateDatePicker.Text = reservation.EndDate.ToShortDateString();

            FirstNameBox.Text = reservation.Visitor.FirstName;
            PrepositionBox.Text = reservation.Visitor.Preposition == string.Empty ? "" : reservation.Visitor.Preposition;
            LastNameBox.Text = reservation.Visitor.LastName;
            PhoneNumberBox.Text = reservation.Visitor.PhoneNumber.ToString();
            CityBox.Text = reservation.Visitor.City.ToString();
            StreetBox.Text = reservation.Visitor.Adress;

            HouseNumberBox.Text = reservation.Visitor.HouseNumber.ToString();
            PostalCodeBox.Text = reservation.Visitor.PostalCode;
        }

        public bool saveReservation(Reservation reservation)
        {

            var result = MessageBox.Show("Weet je zeker dat je de reservering gegevens wilt aanpassen?", "Confirm", MessageBoxButton.YesNo);

            bool errorsFound = false;

            SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.Red);

            if (result == MessageBoxResult.Yes)
            {
                int siteID;
                if (!int.TryParse(SiteIDBox.Text, out siteID))
                {
                    SiteIDLabel.Visibility = Visibility.Visible;
                    SiteIDLabel.Content = "Moet een getal zijn.";

                    SiteIDLabel.Foreground = solidColorBrush;
                    errorsFound = true;
                }
                else if (siteID > retrieveData.GetCampSiteID().Count() || siteID < 1)
                {
                    SiteIDLabel.Visibility = Visibility.Visible;
                    SiteIDLabel.Content = "Deze plek bestaat niet.";
                    SiteIDLabel.Foreground = solidColorBrush;
                    errorsFound = true;
                }
                else
                {
                    reservation.SiteID = siteID;
                    SiteIDLabel.Visibility = Visibility.Hidden;
                }


                int phoneNumber;
                if (int.TryParse(PhoneNumberBox.Text, out phoneNumber))
                {
                    reservation.Visitor.PhoneNumber = phoneNumber;
                    PhoneNumberLabel.Visibility = Visibility.Hidden;
                }
                else
                {
                    PhoneNumberLabel.Visibility = Visibility.Visible;
                    PhoneNumberLabel.Content = "Moet een getal zijn";
                    PhoneNumberLabel.Foreground = solidColorBrush;
                    errorsFound = true;
                }


                Regex reg = new Regex("^[1-9]*[a-z]{0,2}$");
                if (reg.IsMatch(HouseNumberBox.Text))
                {
                    reservation.Visitor.HouseNumber = HouseNumberBox.Text;
                    HouseNumberLabel.Visibility = Visibility.Hidden;

                }
                else
                {
                    HouseNumberLabel.Visibility = Visibility.Visible;
                    HouseNumberLabel.Content = "2 letters max.";
                    HouseNumberLabel.Foreground = solidColorBrush;
                    errorsFound = true;
                }


                Regex regex = new("^[1-9][0-9]{3}\\s?[a-zA-Z]{2}$");

                if (regex.IsMatch(PostalCodeBox.Text) && PostalCodeBox.Text.Length <= 6)
                {
                    reservation.Visitor.PostalCode = PostalCodeBox.Text.ToUpper();
                    PostalCodeLabel.Visibility = Visibility.Hidden;
                }
                else
                {
                    PostalCodeLabel.Visibility = Visibility.Visible;
                    PostalCodeLabel.Content = "Onjuiste formaat.";
                    PostalCodeLabel.Foreground = solidColorBrush;
                    errorsFound = true;
                }


                if (FirstNameBox.Text.IsNullOrEmpty())
                {
                    FirstNameLabel.Visibility = Visibility.Visible;
                    FirstNameLabel.Content = "Mag niet leeg zijn.";
                    FirstNameLabel.Foreground = solidColorBrush;
                    errorsFound = true;
                }
                else
                {
                    reservation.Visitor.FirstName = FirstNameBox.Text;
                    FirstNameLabel.Visibility = Visibility.Hidden;
                }


                if (LastNameBox.Text.IsNullOrEmpty())
                {
                    LastNameLabel.Visibility = Visibility.Visible;
                    LastNameLabel.Content = "Mag niet leeg zijn";
                    LastNameLabel.Foreground = solidColorBrush;
                    errorsFound = true;
                }
                else
                {
                    reservation.Visitor.LastName = LastNameBox.Text;
                    LastNameLabel.Visibility = Visibility.Hidden;
                }

                if (CityBox.Text.IsNullOrEmpty())
                {
                    CityLabel.Visibility = Visibility.Visible;
                    CityLabel.Content = "Mag niet leeg zijn";
                    CityLabel.Foreground = solidColorBrush;
                    errorsFound = true;
                }
                else
                {
                    reservation.Visitor.City = CityBox.Text;
                    CityLabel.Visibility = Visibility.Hidden;
                }
                if (StreetBox.Text.IsNullOrEmpty())
                {
                    StreetLabel.Visibility = Visibility.Visible;
                    StreetLabel.Content = "Mag niet leeg zijn";
                    StreetLabel.Foreground = solidColorBrush;
                    errorsFound = true;
                }
                else
                {
                    reservation.Visitor.Adress = StreetBox.Text;
                    StreetLabel.Visibility = Visibility.Hidden;
                }
                reservation.Visitor.Preposition = PrepositionBox.Text;

                if (!retrieveData.GetOtherAvailableReservations(reservation.SiteID, StartDateDatePicker.SelectedDate.GetValueOrDefault().ToString("MM-dd-yyyy"), EndDateDatePicker.SelectedDate.GetValueOrDefault().ToString("MM-dd-yyyy"), reservation.ReservationID))
                {
                    StartDateLabel.Visibility = Visibility.Visible;
                    StartDateLabel.Content = "De ingevulde datums zijn niet beschikbaar.";
                    StartDateLabel.Foreground = solidColorBrush;
                    errorsFound = true;
                }
                else if (Convert.ToDateTime(EndDateDatePicker.Text) < DateTime.Today)
                {
                    EndDateLabel.Visibility = Visibility.Visible;
                    EndDateLabel.Content = "Einddatum kan niet in het verleden zijn.";
                    EndDateLabel.Foreground = solidColorBrush;
                    errorsFound = true;
                }
                else if (Convert.ToDateTime(EndDateDatePicker.Text) < Convert.ToDateTime(StartDateDatePicker.Text))
                {
                    EndDateLabel.Visibility = Visibility.Visible;
                    EndDateLabel.Content = "Einddatum kan niet voor de begindatum.";
                    EndDateLabel.Foreground = solidColorBrush;
                    errorsFound = true;
                }
                else
                {
                    reservation.StartDate = Convert.ToDateTime(StartDateDatePicker.Text);
                    reservation.EndDate = Convert.ToDateTime(EndDateDatePicker.Text);

                    StartDateLabel.Visibility = Visibility.Hidden;
                    EndDateLabel.Visibility = Visibility.Hidden;
                }



                if (!errorsFound)
                {
                    retrieveData.UpdateReservation(reservation.ReservationID, reservation.StartDate, reservation.Visitor, reservation.EndDate, reservation.SiteID);
                    return true;
                }
            }
            return false;
        }

        public void hideErrors()
        {
            SiteIDLabel.Visibility = Visibility.Hidden;
            StartDateLabel.Visibility = Visibility.Hidden;
            EndDateLabel.Visibility = Visibility.Hidden;
            PhoneNumberLabel.Visibility = Visibility.Hidden;
            HouseNumberLabel.Visibility = Visibility.Hidden;
            PostalCodeLabel.Visibility = Visibility.Hidden;
        }

        public void chanceAanpassenOrSaveButtonContent(bool buttonState)
        {
            ReservationAanpassenButtonState = buttonState;
            EditReservationButton.Content = ReservationAanpassenButtonState ? "Opslaan" : "Aanpassen";
        }

        public void enabledReservationInfoTextBoxes(TextBox[] TextBoxElements, bool isVisible)
        {
            foreach (UIElement element in TextBoxElements)
            {
                element.IsEnabled = isVisible;
            }
        }

        public void enabledReservationInfodatePicker(DatePicker[] TextBoxElements, bool isVisible)
        {
            foreach (UIElement element in TextBoxElements)
            {
                element.IsEnabled = isVisible;
            }
        }
    }
}