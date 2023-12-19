using camping.Core;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace camping.WPF
{
    public class ChangeReservation
    {

        private RetrieveData retrieveData;

        public bool ReservationAanpassenButtonState; //true save : false aanpassen
        private bool errorsFound;
        public bool isUpdating { get; set; }
        private string[] visitorData;

        SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.Red);

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

        public void editReservationButton(object sender, ChangeReservationEventArgs e)
        {
            saveReservation(e.Reservation);
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

        private void notEmpty(Reservation reservation, TextBox[] text, Label[] label)
        {
            visitorData = new string[4];
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i].Text.IsNullOrEmpty())
                {
                    label[i].Visibility = Visibility.Visible;
                    label[i].Content = "Mag niet leeg zijn.";
                    label[i].Foreground = solidColorBrush;
                    errorsFound = true;
                }
                else
                {
                    visitorData[i] = text[i].Text;
                    label[i].Visibility = Visibility.Hidden;
                }
            }
        }

        private void checkSiteID(Reservation reservation)
        {
            int siteID;
            if (!int.TryParse(SiteIDBox.Text, out siteID))
            {
                SiteIDLabel.Visibility = Visibility.Visible;
                SiteIDLabel.Content = "Moet een getal zijn.";

                SiteIDLabel.Foreground = solidColorBrush;
                errorsFound = true;
            }
            else if (retrieveData.GetCampSiteID().Contains(siteID) || siteID < 1)
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
        }

        private void checkPhoneNumber(Reservation reservation)
        {
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
        }

        private void checkHouseNumber(Reservation reservation)
        {
            Regex regex = new Regex("^[1-9][0-9]*[a-z]{0,2}$");
            if (regex.IsMatch(HouseNumberBox.Text))
            {
                reservation.Visitor.HouseNumber = HouseNumberBox.Text;
                HouseNumberLabel.Visibility = Visibility.Hidden;

            }
            else
            {
                HouseNumberLabel.Visibility = Visibility.Visible;
                HouseNumberLabel.Content = "Ongeldig huisnummer.";
                HouseNumberLabel.Foreground = solidColorBrush;
                errorsFound = true;
            }
        }

        private void checkPostalCode(Reservation reservation)
        {
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
        }

        private void checkDates(Reservation reservation)
        {
            StartDateLabel.Visibility = Visibility.Hidden;
            EndDateLabel.Visibility = Visibility.Hidden;
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
            }
        }

        public bool saveReservation(Reservation reservation)
        {
            checkSiteID(reservation);
            checkPhoneNumber(reservation);
            checkHouseNumber(reservation);
            checkPostalCode(reservation);

            notEmpty(reservation, new[] { FirstNameBox, LastNameBox, CityBox, StreetBox }, new[] { FirstNameLabel, LastNameLabel, CityLabel, StreetLabel });
            reservation.Visitor.FirstName = visitorData[0];
            reservation.Visitor.LastName = visitorData[1];
            reservation.Visitor.City = visitorData[2];
            reservation.Visitor.Adress = visitorData[3];


            reservation.Visitor.Preposition = PrepositionBox.Text;

            checkDates(reservation);
            if (!errorsFound) { 
            retrieveData.UpdateReservation(reservation.ReservationID, reservation.StartDate, reservation.Visitor, reservation.EndDate, reservation.SiteID);
            return true;
            }
            else {
            return false; 
            }
        }

        public void hideErrors()
        {
            SiteIDLabel.Visibility = Visibility.Hidden;
            StartDateLabel.Visibility = Visibility.Hidden;
            EndDateLabel.Visibility = Visibility.Hidden;
            FirstNameLabel.Visibility = Visibility.Hidden;
            LastNameLabel.Visibility = Visibility.Hidden;
            PhoneNumberLabel.Visibility = Visibility.Hidden;
            CityLabel.Visibility = Visibility.Hidden;
            StreetLabel.Visibility = Visibility.Hidden;
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
