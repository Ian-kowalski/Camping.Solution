using camping.Core;
using camping.Database;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace camping.WPF
{
    /// <summary>
    /// Interaction logic for Overview.xaml
    /// </summary>
    public partial class Overview : Window
    {
        private SshConnection connection { get; set; }
        private SiteData siteData { get; set; }
        private ReservationRepository resData { get; set; }
        private RetrieveData retrieveData { get; set; }
        private Location tempLocation { get; set; }

        private bool isUpdating { get; set; }
        private bool resInfoVisible { get; set; } = false;

        private bool ReservationAanpassenButtonState { get; set; } = false; //true save : false aanpassen

        private int rowLength;

        private Area? SelectedArea;

        private List<Reservation> toBeCancel = new List<Reservation>();

        private Reservation selectedReservation;

        private Street? SelectedStreet;

        private Site? SelectedSite;

/*        private Location selectedLocation;
*/        private Button changeFacilitiesButton;

        private SearchAvailableCampsites addReservation;

        public Overview()
        {
            InitializeComponent();
            connection = new SshConnection();
            siteData = new SiteData();
            resData = new ReservationRepository();
            retrieveData = new RetrieveData(siteData, resData);

            displayAllLocations();


            displayAllReservations();

            addReservation = new SearchAvailableCampsites(AddReservationGrid, siteData, resData, AddReservationGridList);

            Closing += onWindowClosing;
        }


        private void displayAllLocations()
        {
            CampSiteList.Children.Clear();
            CampSiteList.RowDefinitions.Clear();
            rowLength = 0;
            displayAreas();
        }


        private void displayAreas()
        {
            foreach (Area area in retrieveData.Areas)
            {


                addNewRowDefinition();

                Button button = createLocationButton(area);
                if (SelectedArea == area) button.Background = new SolidColorBrush(Color.FromArgb(185, 150, 190, 250));
                else button.Background = new SolidColorBrush(Color.FromRgb(190, 190, 190));
                button.BorderBrush = Brushes.Black;
                button.BorderThickness = new Thickness(2);
                button.FontSize = 16;
                button.MouseDoubleClick += (sender, e) => { onSitePress(area); };
                button.Click += (sender, e) => { onSiteSelect(area); };

                Grid.SetRow(button, rowLength);
                CampSiteList.Children.Add(button);
                rowLength++;

                displayStreets(area.AreaID);
            }
        }

        // laat de straten zien van de area
        private void displayStreets(int areaID)
        {
            foreach (Street street in retrieveData.Streets)
            {
                if (street.AreaID == areaID && street.Visible)
                {

                    addNewRowDefinition();

                    Button button = createLocationButton(street);
                    if (SelectedStreet == street) button.Background = new SolidColorBrush(Color.FromArgb(185, 160, 200, 240));
                    else button.Background = new SolidColorBrush(Color.FromRgb(210, 210, 210));
                    button.BorderBrush = Brushes.Black;
                    button.BorderThickness = new Thickness(2);
                    button.FontSize = 16;
                    button.MouseDoubleClick += (sender, e) => { onSitePress(street); };
                    button.Click += (sender, e) => { onSiteSelect(street); };

                    Grid.SetRow(button, rowLength);
                    CampSiteList.Children.Add(button);

                    rowLength++;

                    displaySites(street.StreetID);
                }


            }
        }

        // laat de sites zien van de straat
        private void displaySites(int streetID)
        {
            foreach (Site site in retrieveData.Sites)
            {
                if (site.StreetID == streetID && site.Visible)
                {

                    addNewRowDefinition();


                    Button button = createLocationButton(site);

                    if (SelectedSite == site) button.Background = new SolidColorBrush(Color.FromArgb(185, 170, 210, 230));
                    else button.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                    button.BorderBrush = Brushes.Black;
                    button.BorderThickness = new Thickness(2);
                    button.FontSize = 16;

                    button.MouseDoubleClick += (sender, e) => { onSitePress(site); };
                    button.Click += (sender, e) => { onSiteSelect(site); };

                    Grid.SetRow(button, rowLength);
                    CampSiteList.Children.Add(button);
                    rowLength++;
                }
            }
        }

        // highlight de geselecteerde site
        private void onSitePress(Location location)
        {
            if (location is Area && location is not null)
            {
                Area area = location as Area;
                SelectedSite = null;
                SelectedStreet = null;
                SelectedArea = area;
/*                selectedLocation = area;
*/                toggleChildrenVisibility(area);
                displayAllLocations();
            }
            else
            if (location is Street && location is not null)
            {
                Street street = location as Street;
                SelectedSite = null;
                SelectedStreet = street;
                SelectedArea = retrieveData.GetAreaFromID(SelectedStreet.AreaID);
/*                selectedLocation = street;
*/                toggleChildrenVisibility(street);
                displayAllLocations();
            }
            else
            if (location is Site && location is not null)
            {
                Site site = location as Site;
                SelectedSite = site;
                SelectedStreet = retrieveData.GetStreetFromID(site.StreetID);
                SelectedArea = retrieveData.GetAreaFromID(SelectedStreet.AreaID);
/*                selectedLocation = site;
*/                displayAllLocations();
            }
            /*            displayInformation(location); 
            */
            
        }
        public void onSiteSelect(Location location)
        {
            if (location is Area && location is not null)
            {
                Area area = location as Area;
                SelectedSite = null;
                SelectedStreet = null;
                SelectedArea = area;
                displayAllLocations();
            }
            else
            if (location is Street && location is not null)
            {
                Street street = location as Street;
                SelectedSite = null;
                SelectedStreet = street;
                SelectedArea = retrieveData.GetAreaFromID(SelectedStreet.AreaID);
                displayAllLocations();

            }
            else
            if (location is Site && location is not null)
            {
                Site site = location as Site;
                SelectedSite = site;
                SelectedStreet = retrieveData.GetStreetFromID(site.StreetID);
                SelectedArea = retrieveData.GetAreaFromID(SelectedStreet.AreaID);
                displayAllLocations();

            }
            LocationInformation locationInformation = new(LocationInfoGrid, siteData, retrieveData, location, SelectedArea, SelectedStreet, SelectedSite);
        }

        // toggled de visibility van de straat van een area
        private void toggleChildrenVisibility(Area area)
        {


            foreach (Street street in retrieveData.Streets)
            {
                if (street.AreaID == area.AreaID)
                {
                    street.Visible = !street.Visible;

                    // als de straat verborgen wordt, verberg ook de sites
                    if (!street.Visible)
                    {
                        hideChildren(street);
                    }
                }
            }
        }

        // toggled de visibility van de sites van een straat
        private void toggleChildrenVisibility(Street street)
        {


            foreach (Site site in retrieveData.Sites)
            {
                if (site.StreetID == street.StreetID)
                {
                    site.Visible = !site.Visible;
                }

            }


        }

        // verbergt alle sites van de straat
        private void hideChildren(Street street)
        {
            foreach (Site site in retrieveData.Sites)
            {
                if (site.StreetID == street.StreetID)
                {
                    site.Visible = false;
                }
            }
        }

        private void addNewRowDefinition()
        {
            RowDefinition rowDef = new RowDefinition();
            rowDef.Height = new GridLength(50);
            CampSiteList.RowDefinitions.Add(rowDef);
        }

        private Button createLocationButton(Site site)
        {
            Button button = new Button();
            button.Content = $"Plek {site.CampSiteID}";
            button.Margin = new Thickness(272, 4, 4, 4);

            // De volledige campsite wordt meegegeven aan de button.
            // De tag kan opgevraagd worden om informatie op het rechter scherm te tonen.
            button.Tag = site;

            return button;
        }

        private Button createLocationButton(Street street)
        {
            Button button = new Button();
            button.Content = $"Straat {street.StreetID}";
            button.Margin = new Thickness(144, 4, 4, 4);

            // De volledige campsite wordt meegegeven aan de button.
            // De tag kan opgevraagd worden om informatie op het rechter scherm te tonen.
            button.Tag = street;

            return button;
        }

        private Button createLocationButton(Area area)
        {
            Button button = new Button();
            button.Content = $"Gebied {area.AreaID}";
            button.Margin = new Thickness(16, 4, 4, 4);

            // De volledige campsite wordt meegegeven aan de button.
            // De tag kan opgevraagd worden om informatie op het rechter scherm te tonen.
            button.Tag = area;

            return button;
        }

        public void onWindowClosing(object sender, CancelEventArgs e)
        {
            connection.BreakConnection();
        }

        private void tabButtonClick(object sender, RoutedEventArgs e)
        {
            setTabButtonState((Button)sender,
                new[] { SiteOverview, LocationInfo, AddReservationList, AddReservationInfo, ReservationList, ReservationInfo },
                new[] { SiteControlButton, AddReservationButton, ReservationsButton }
                );
        }

        private void setTabButtonState(Button selectedButton, UIElement[] BorderElements, Button[] buttons)
        {

            foreach (var button in buttons)
            {
                button.IsEnabled = button != selectedButton;
            }
            for (int i = 0; i < BorderElements.Length; i += 2)
            {
                BorderElements[i].Visibility = i / 2 == Array.IndexOf(buttons, selectedButton) ? Visibility.Visible : Visibility.Hidden;
                BorderElements[i + 1].Visibility = BorderElements[i].Visibility;
            }

            AnnulerenButton.Visibility = selectedButton == ReservationsButton ? Visibility.Visible : Visibility.Hidden;

        }


        private void displayAllReservations()
        {
            // zorgt ervoor dat de annuleerlijst weer null wordt wanneer
            // er opnieuw een reservering geselecteerd wordt
            // (anders bevat de lisjt reserveringen die niet zijn aangeklikt!)

            if (reservationIDFilterBox.Text != string.Empty || LastNameFilterBox.Text != string.Empty)
            {
                int resID = reservationIDFilterBox.Text == string.Empty ? -1 : int.Parse(reservationIDFilterBox.Text);
                retrieveData.UpdateReservations(resID, LastNameFilterBox.Text.Trim()); ;
            }
            else
            {
                retrieveData.UpdateReservations();
            }

            List<Reservation> reservations = retrieveData.Reservations;

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8, GridUnitType.Star) });


            for (int i = 0; i < reservations.Count; i++)
            {

                Reservation reservation = reservations[i];
                RowDefinition row = new RowDefinition();

                row.Tag = reservation;
                row.Height = new GridLength(30);

                addCancelCheckBoxColum(grid, i, reservation);
                AddReservationInfoColum(grid, i, reservation);



                grid.RowDefinitions.Add(row);
            }
            ReservationListScrollViewer.Content = grid;
        }
        private void AddReservationInfoColum(Grid grid, int i, Reservation reservation)
        {
            Grid InfoGrid = GetGridOfReservationLine(reservation);
            Grid.SetColumn(InfoGrid, 1);
            Grid.SetRow(InfoGrid, i);
            InfoGrid.MouseDown += (sender, e) => { RowClick(reservation); };
            grid.Children.Add(InfoGrid);
        }
        private Grid GetGridOfReservationLine(Reservation reservation)
        {

            Grid grid = new Grid();
            grid.MouseDown += (sender, e) => { fillReservationInfoGrid(reservation); };

            if (selectedReservation is not null)
            {
                if (reservation.ReservationID == selectedReservation.ReservationID)
                {
                    grid.Background = new SolidColorBrush(Color.FromArgb(185, 150, 190, 250));
                }
                else { grid.Background = Brushes.Transparent; }
            }
            else { grid.Background = Brushes.Transparent; }


            for (int i = 0; i < 5; i++)
            {
                Label label = new Label();
                label.Margin = new Thickness(0);
                label.VerticalContentAlignment = VerticalAlignment.Center;
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                int info = i;
                switch (info)
                {
                    case 0:
                        label.Content = reservation.ReservationID.ToString();
                        grid.ColumnDefinitions.Add(new ColumnDefinition());
                        break;
                    case 1:
                        label.Content = reservation.SiteID.ToString();
                        grid.ColumnDefinitions.Add(new ColumnDefinition());
                        break;
                    case 2:
                        label.Content = reservation.Guest.LastName.ToString();
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                        break;
                    case 3:
                        label.Content = reservation.StartDate.ToShortDateString();
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                        break;
                    case 4:
                        label.Content = reservation.EndDate.ToShortDateString();
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                        break;
                }

                grid.Margin = new Thickness(0);
                Grid.SetRow(label, 0);
                Grid.SetColumn(label, i);
                grid.Children.Add(label);
            }
            return grid;
        }



        private void addCancelCheckBoxColum(Grid grid, int i, Reservation reservation)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.Checked += (sender, e) => { Un_Checkt(reservation, sender); };
            checkBox.Unchecked += (sender, e) => { Un_Checkt(reservation, sender); };

            if (toBeCancel.Contains(reservation))
            {
                checkBox.IsChecked = true;
            }

            Grid.SetColumn(checkBox, 0);
            Grid.SetRow(checkBox, i);
            checkBox.HorizontalAlignment = HorizontalAlignment.Center;
            checkBox.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(checkBox);
        }
        private void fillReservationInfoGrid(Reservation reservation)
        {
            selectedReservation = reservation;


            SiteIDBox.Text = reservation.SiteID.ToString();
            StartDateDatePicker.Text = reservation.StartDate.ToShortDateString();
            EndDatedatePicker.Text = reservation.EndDate.ToShortDateString();

            FirstNameBox.Text = reservation.Guest.FirstName;
            PrepositionBox.Text = reservation.Guest.Preposition == string.Empty ? "" : reservation.Guest.Preposition;
            LastNameBox.Text = reservation.Guest.LastName;
            PhoneNumberBox.Text = reservation.Guest.PhoneNumber.ToString();
            CityBox.Text = reservation.Guest.City.ToString();
            AdressBox.Text = reservation.Guest.Adress;

            HouseNumberBox.Text = reservation.Guest.HouseNumber.ToString();
            PostalCodeBox.Text = reservation.Guest.PostalCode;
        }
        private void RowClick(Reservation reservation)
        {
            resInfoVisible = false;

            selectedReservation = reservation;
            ReservationInfoGrid.Visibility = Visibility.Visible;

            displayAllReservations();


            chanceAanpassenOrSaveButtonContent(resInfoVisible);
            enabledReservationInfoTextBoxes(new[] { SiteIDBox, FirstNameBox, PrepositionBox, LastNameBox, PhoneNumberBox, CityBox, AdressBox, HouseNumberBox, PostalCodeBox }, resInfoVisible);
            enabledReservationInfodatePicker(new[] { StartDateDatePicker, EndDatedatePicker }, resInfoVisible);

            hideErrors();
        }

        private void Un_Checkt(Reservation reservation, object sender)
        {
            CheckBox c = (CheckBox)sender;
            if (c.IsChecked == true && !toBeCancel.Contains(reservation))
            {
                toBeCancel.Add(reservation);
            }
            else if(c.IsChecked == false && toBeCancel.Contains(reservation))
            {
                toBeCancel.Remove(reservation);
            }
            if (toBeCancel.Count != 0)
            {
                AnnulerenButton.IsEnabled = true;
            }
            else
            {
                AnnulerenButton.IsEnabled = false;
            }
        }



        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            string combinedString = "\n";

            foreach (var reservation in toBeCancel)
            {
                combinedString += $"{reservation.ReservationID}, ";
            }
            combinedString.Remove(combinedString.Length-2);
            string messageBoxText = "Weet je zeker dat je de volgende reservering(en) wil verwijderen: " + combinedString;
            string caption = "Annuleren reservering(en)";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;

            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    // User pressed Yes button
                    if (toBeCancel.Contains(selectedReservation))
                    {
                        ReservationInfoGrid.Visibility = Visibility.Hidden;
                    }
                    foreach (var reservation in toBeCancel)
                    {
                        // ...Delete out of database
                        retrieveData.DeleteReservation(reservation.ReservationID);
                        
                    }
                    toBeCancel.Clear();
                    displayAllReservations();
                    break;
                case MessageBoxResult.No:
                    // User pressed No button
                    // ...Nothing
                    break;
            }
        }

        private void EditReservationButtonClick(object sender, RoutedEventArgs e)
        {
            if (ReservationAanpassenButtonState)
            {
                if (!saveReservation(selectedReservation))
                {
                    return;
                }
                displayAllReservations();
            }

            resInfoVisible = !resInfoVisible;
            chanceAanpassenOrSaveButtonContent(resInfoVisible);
            enabledReservationInfoTextBoxes(new[] { SiteIDBox, FirstNameBox, PrepositionBox, LastNameBox, PhoneNumberBox, CityBox, AdressBox, HouseNumberBox, PostalCodeBox }, resInfoVisible);
            enabledReservationInfodatePicker(new[] { StartDateDatePicker, EndDatedatePicker }, resInfoVisible);
        }


        private bool saveReservation(Reservation reservation)
        {
            
            var result = MessageBox.Show("Weet je zeker dat je de reservering gegevens wilt aanpassen?", "Confirm", MessageBoxButton.YesNo);

            bool errorsFound = false;

            if (result == MessageBoxResult.Yes)
            {
                int siteID;
                if (!int.TryParse(SiteIDBox.Text, out siteID))
                {
                    SiteIDLabel.Visibility = Visibility.Visible;
                    SiteIDLabel.Content = "Moet een getal zijn.";
                    SiteIDLabel.Foreground = Brushes.Red;
                    errorsFound = true;
                } else if (siteID > retrieveData.GetCampSiteID().Count() || siteID < 1)
                {
                    SiteIDLabel.Visibility = Visibility.Visible;
                    SiteIDLabel.Content = "Deze plek bestaat niet.";
                    SiteIDLabel.Foreground = Brushes.Red;
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
                    reservation.Guest.PhoneNumber = phoneNumber;
                    PhoneNumberLabel.Visibility = Visibility.Hidden;
                }
                else
                {
                    PhoneNumberLabel.Visibility = Visibility.Visible;
                    PhoneNumberLabel.Content = "Moet een getal zijn";
                    PhoneNumberLabel.Foreground = Brushes.Red;
                    errorsFound = true;
                }
                

                Regex reg = new Regex("^[1-9]*[a-z]{0,2}$");
                if (reg.IsMatch(HouseNumberBox.Text))
                {
                    reservation.Guest.HouseNumber = HouseNumberBox.Text;
                    HouseNumberLabel.Visibility = Visibility.Hidden;

                }
                else
                {
                    HouseNumberLabel.Visibility = Visibility.Visible;
                    HouseNumberLabel.Content = "2 letters max.";
                    HouseNumberLabel.Foreground = Brushes.Red;
                    errorsFound = true;
                }

               
                Regex regex = new("^[1-9][0-9]{3}\\s?[a-zA-Z]{2}$");

                if (regex.IsMatch(PostalCodeBox.Text) && PostalCodeBox.Text.Length <= 6)
                {
                    reservation.Guest.PostalCode = PostalCodeBox.Text.ToUpper();
                    PostalCodeLabel.Visibility = Visibility.Hidden;
                }
                else
                {
                    PostalCodeLabel.Visibility= Visibility.Visible;
                    PostalCodeLabel.Content = "Onjuiste formaat.";
                    PostalCodeLabel.Foreground = Brushes.Red;
                    errorsFound = true;
                }


                if (FirstNameBox.Text.IsNullOrEmpty())
                {
                    FirstNameLabel.Visibility = Visibility.Visible;
                    FirstNameLabel.Content = "Mag niet leeg zijn.";
                    FirstNameLabel.Foreground = Brushes.Red;
                    errorsFound = true;
                }
                else
                {
                    reservation.Guest.FirstName = FirstNameBox.Text;
                    FirstNameLabel.Visibility = Visibility.Hidden;
                }
                

                if (LastNameBox.Text.IsNullOrEmpty())
                {
                    LastNameLabel.Visibility = Visibility.Visible;
                    LastNameLabel.Content = "Mag niet leeg zijn";
                    LastNameLabel.Foreground = Brushes.Red;
                    errorsFound = true;
                }
                else
                {
                    reservation.Guest.LastName = LastNameBox.Text;
                    LastNameLabel.Visibility = Visibility.Hidden;
                }

                if (CityBox.Text.IsNullOrEmpty())
                {
                    CityLabel.Visibility = Visibility.Visible;
                    CityLabel.Content = "Mag niet leeg zijn";
                    CityLabel.Foreground = Brushes.Red;
                    errorsFound = true;
                }
                else
                {
                    reservation.Guest.City = CityBox.Text;
                    CityLabel.Visibility = Visibility.Hidden;
                }
                if (AdressBox.Text.IsNullOrEmpty())
                {
                    AdressLabel.Visibility = Visibility.Visible;
                    AdressLabel.Content = "Mag niet leeg zijn";
                    AdressLabel.Foreground = Brushes.Red;
                    errorsFound = true;
                }
                else
                {
                    reservation.Guest.Adress = AdressBox.Text;
                    AdressLabel.Visibility = Visibility.Hidden;
                }

                reservation.Guest.Preposition = PrepositionBox.Text;

                if (!retrieveData.GetOtherAvailableReservations(reservation.SiteID, StartDateDatePicker.SelectedDate.GetValueOrDefault().ToString("MM-dd-yyyy"), EndDatedatePicker.SelectedDate.GetValueOrDefault().ToString("MM-dd-yyyy"), reservation.ReservationID))
                {
                    StartDateLabel.Visibility = Visibility.Visible;
                    StartDateLabel.Content = "De ingevulde datums zijn niet beschikbaar.";
                    StartDateLabel.Foreground = Brushes.Red;
                    errorsFound = true;
                }
                else if (Convert.ToDateTime(EndDatedatePicker.Text) < DateTime.Today)
                {
                    EndDateLabel.Visibility = Visibility.Visible;
                    EndDateLabel.Content = "Einddatum kan niet in het verleden zijn.";
                    EndDateLabel.Foreground = Brushes.Red;
                    errorsFound = true;
                }
                else if (Convert.ToDateTime(EndDatedatePicker.Text) < Convert.ToDateTime(StartDateDatePicker.Text))
                {
                    EndDateLabel.Visibility = Visibility.Visible;
                    EndDateLabel.Content = "Einddatum kan niet voor de begindatum.";
                    EndDateLabel.Foreground = Brushes.Red;
                    errorsFound = true;
                }
                else
                {
                    reservation.StartDate = Convert.ToDateTime(StartDateDatePicker.Text);
                    reservation.EndDate = Convert.ToDateTime(EndDatedatePicker.Text);

                    StartDateLabel.Visibility = Visibility.Hidden;
                    EndDateLabel.Visibility = Visibility.Hidden;
                }
                
                

                if (!errorsFound)
                {
                    retrieveData.UpdateReservation(reservation.ReservationID, reservation.StartDate, reservation.Guest, reservation.EndDate, reservation.SiteID);
                    return true;
                }
            }
            return false;
        }

        private void hideErrors()
        {
            SiteIDLabel.Visibility = Visibility.Hidden;
            StartDateLabel.Visibility = Visibility.Hidden;
            EndDateLabel.Visibility = Visibility.Hidden;
            PhoneNumberLabel.Visibility = Visibility.Hidden;
            HouseNumberLabel.Visibility = Visibility.Hidden;
            PostalCodeLabel.Visibility = Visibility.Hidden;
        }

        private void Checkfields()
        {
            throw new NotImplementedException();
        }

        private void chanceAanpassenOrSaveButtonContent(bool buttonState)
        {
            ReservationAanpassenButtonState = buttonState;
            EditReservationButton.Content = ReservationAanpassenButtonState ? "save" : "Aanpassen Resevering";
        }

        private void enabledReservationInfoTextBoxes(TextBox[] TextBoxElements, bool isVisible)
        {
            foreach (UIElement element in TextBoxElements)
            {
                element.IsEnabled = isVisible;
            }
        }

        private void enabledReservationInfodatePicker(DatePicker[] TextBoxElements, bool isVisible)
        {
            foreach (UIElement element in TextBoxElements)
            {
                element.IsEnabled = isVisible;
            }
        }

        

        

        private void FilterZoekenEnterPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                displayAllReservations();
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PostalCodeValidation(object sender, TextChangedEventArgs e)
        {
            string PostcodeHoofdletters = ((TextBox)sender).Text;
            if (Regex.IsMatch(PostcodeHoofdletters.Trim(), "^[1-9][0-9]{3}\\s?[a-zA-Z]{2}$"))
            {
                ((TextBox)sender).Foreground = Brushes.Black;
            }
            else { ((TextBox)sender).Foreground = Brushes.Red; }

        }

        private void HouseNumberValidation(object sender, TextChangedEventArgs e)
        {
            string houseNumber = ((TextBox)sender).Text;
            if (Regex.IsMatch(houseNumber.Trim(), "^[1-9]*[a-z]{0,2}$"))
            {
                ((TextBox)sender).Foreground = Brushes.Black;
            }
            else { ((TextBox)sender).Foreground = Brushes.Red; }
        }



        private void PhoneNumberValidation(object sender, TextChangedEventArgs e)
        {
            
                string houseNumber = ((TextBox)sender).Text;
                if (Regex.IsMatch(houseNumber.Trim(), "^[1-9]\\d{1,15}$"))
                {
                    ((TextBox)sender).Foreground = Brushes.Black;
                }
                else { ((TextBox)sender).Foreground = Brushes.Red; }
            
        }
    }
}
