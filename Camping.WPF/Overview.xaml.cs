using camping.Core;
using camping.Database;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public event EventHandler<ChangeReservationEventArgs> EditReservationClick;

        private SshConnection connection { get; set; }
        private SiteData siteData { get; set; }
        private ReservationRepository resData { get; set; }
        private RetrieveData retrieveData { get; set; }
        private Location tempLocation { get; set; }
        private ChangeReservation changeReservation { get; set; }

        private bool resInfoVisible { get; set; } = false;

        private int rowLength;

        private Area? SelectedArea;

        private List<Reservation> toBeCancel = new List<Reservation>();

        private Reservation selectedReservation;
        private LocationInformation locationInformation;

        private Street? SelectedStreet;

        private Site? SelectedSite;

        /*        private Location selectedLocation;
        */
        private Map map;
        private Button changeFacilitiesButton;

        private SearchAvailableCampsites SearchCampsites;
        private const int siteButtonMarginSize = 128;
        private Point pos = new();

        private Map AvailableCampSitesMap;

        public Overview()
        {
            InitializeComponent();
            connection = new SshConnection();
            siteData = new SiteData();
            resData = new ReservationRepository();
            retrieveData = new RetrieveData(siteData, resData);

            map = new Map(retrieveData, campingmap);

            // als een campsite verwijderd wordt, update plekbeheer.
            retrieveData.SiteDeleted += (sender, e) => { 
                displayAllLocations();
                map.drawMap();
                AddReservationInfoGrid.Visibility = Visibility.Hidden;
                AvailableCampsitesScrollViewer.Visibility = Visibility.Hidden;


            };
            
            displayAllLocations();
            displayAllReservations();

            ChangeReservation changeRes = new(retrieveData, SiteIDBox, StartDateDatePicker, EndDatedatePicker, FirstNameBox, PrepositionBox, LastNameBox, PhoneNumberBox, CityBox, AdressBox, HouseNumberBox, PostalCodeBox, SiteIDLabel, StartDateLabel, EndDateLabel, FirstNameLabel, LastNameLabel, PhoneNumberLabel, CityLabel, AdressLabel, HouseNumberLabel, PostalCodeLabel, EditReservationButton);
            changeReservation = changeRes;

            SearchCampsites = new SearchAvailableCampsites(SearchCampsiteGrid, siteData, resData, AvailableCampsitesGridList);
            AvailableCampSitesMap = new Map(retrieveData, AvailableCampsitesMap);
            SearchCampsites.AvailableCampsitesListEventHandler += (sender, e) =>
            {
                AvailableCampsitesMap.Visibility = Visibility.Visible;
                AddReservationInfoGrid.Visibility = Visibility.Collapsed;
                AvailableCampSitesMap.ShowAvailableCampsites(e.AvailableSites);
            };

            AvailableCampSitesMap.SiteSelected += (sender, e) => {
                AvailableCampsitesMap.Visibility = Visibility.Collapsed;
                AddReservationInfoGrid.Visibility = Visibility.Visible;
                SearchCampsites.ShowAddReservation(sender, new AddReservationEventArgs(e.Site.LocationID, SearchCampsites.StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today), SearchCampsites.EndDateButton.SelectedDate.GetValueOrDefault(SearchCampsites.StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today))));
            };


            SearchCampsites.AddReservation += (sender, e) =>
            {
                AvailableCampsitesMap.Visibility = Visibility.Collapsed;
                AddReservationInfoGrid.Visibility = Visibility.Visible;
                fillAddReservationInfoGrid(e.CampSiteID, e.StartDate, e.EndDate);
            };


            map.SiteSelected += (sender, e) => 
            { 
                onSiteSelect(e.Site);
                enableChildrenVisibility(retrieveData.GetAreaFromID(retrieveData.GetStreetFromID(e.Site.StreetID).AreaID));
                enableChildrenVisibility(retrieveData.GetStreetFromID(e.Site.StreetID));
                
                displayAllLocations();
            };
            map.StreetSelected += (sender, e) =>
            {
                onSiteSelect(e.Street);
                enableChildrenVisibility(retrieveData.GetAreaFromID(e.Street.AreaID));
                displayAllLocations();
            };

            EditReservationClick += changeReservation.editReservationButton;
            Closing += onWindowClosing;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            Grid campingmap = sender as Grid;


            pos = e.GetPosition(campingmap);
            preview.Margin = new Thickness(pos.X, pos.Y, 0, 0);
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
                button.Click += (sender, e) => 
                {
                    onSiteSelect(area);
                    map.ShowSelectedAreaOnMap(area);
                };

                Grid.SetRow(button, rowLength);
                CampSiteList.Children.Add(button);
                rowLength++;

                displayStreets(area.LocationID);
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
                    button.Click += (sender, e) => 
                    {
                        onSiteSelect(street);
                        map.ShowSelectedStreetOnMap(street, true);
                    };

                    Grid.SetRow(button, rowLength);
                    CampSiteList.Children.Add(button);

                    rowLength++;

                    displaySites(street);
                }


            }
        }

        // laat de sites zien van de straat
        private void displaySites(Street street)
        {
            bool visible = false;
            int siteCount = 0;
            foreach (Site site in retrieveData.Sites)
            {
                if (site.StreetID == street.LocationID) { siteCount++; }
                if (site.StreetID == street.LocationID && site.Visible)
                {

                    addNewRowDefinition();


                    Button button = createLocationButton(site);

                    if (SelectedSite == site) { button.Background = new SolidColorBrush(Color.FromArgb(185, 170, 210, 230)); }
                    else button.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                    button.BorderBrush = Brushes.Black;
                    button.BorderThickness = new Thickness(2);
                    button.FontSize = 16;

                    button.MouseDoubleClick += (sender, e) => { onSitePress(site); };
                    button.Click += (sender, e) => 
                    {
                        onSiteSelect(site);
                        map.ShowSelectedSiteOnMap(site);
                        map.ShowSelectedStreetOnMap(street, false);
                    };

                    Grid.SetRow(button, rowLength);
                    CampSiteList.Children.Add(button);
                    rowLength++;
                    visible = true;
                }
            }
            if ( (siteCount == 0  || visible) && SelectedStreet is not null && SelectedStreet.LocationID == street.LocationID)
            {
                addNewRowDefinition();
                Button button = createLocationButton(siteButtonMarginSize);
                button.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                button.BorderBrush = Brushes.Black;
                button.BorderThickness = new Thickness(2);
                button.FontSize = 16;

                button.Click += (sender, e) => addSitePreview();

                Grid.SetRow(button, rowLength);
                CampSiteList.Children.Add(button);
                rowLength++;
                
            }
        }
        private void addSitePreview()
        {
            preview.Visibility = Visibility.Visible;
            preview.RenderTransform = new RotateTransform { Angle = map.calculateStreetAngle(SelectedStreet) };
        }

        private void TextBlockClick(object sender, MouseButtonEventArgs e) //add site
        {
            preview.Visibility = Visibility.Hidden;
            int tempSiteID = siteData.AddLocation(SelectedStreet, Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y));            
            retrieveData.UpdateLocations();
            Site site = retrieveData.GetSiteFromID(tempSiteID);
            Street street = retrieveData.GetStreetFromID(site.StreetID);
            Area area = retrieveData.GetAreaFromID(street.AreaID);

            toggleChildrenVisibility(area);
            toggleChildrenVisibility(street);

            if (map is not null)
            {
                map.drawMap();
            }
            
            displayAllLocations();
        }

        // highlight de geselecteerde site
        private void onSitePress(Location location)
        {
            preview.Visibility = Visibility.Hidden;
            if (location is Area && location is not null)
            {
                Area area = location as Area;
                SelectedSite = null;
                SelectedStreet = null;
                SelectedArea = area;
                toggleChildrenVisibility(area);
                displayAllLocations();
            }
            else
            if (location is Street && location is not null)
            {
                Street street = location as Street;
                SelectedSite = null;
                SelectedStreet = street;
                SelectedArea = retrieveData.GetAreaFromID(SelectedStreet.AreaID);
                toggleChildrenVisibility(street);
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
        }
        public void onSiteSelect(Location location)
        {
            preview.Visibility = Visibility.Hidden;
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
            locationInformation = new(LocationInfoGrid, siteData, retrieveData, location, SelectedArea, SelectedStreet, SelectedSite);
        }



        // toggled de visibility van de straat van een area
        private void toggleChildrenVisibility(Area area)
        {


            foreach (Street street in retrieveData.Streets)
            {
                if (street.AreaID == area.LocationID)
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
        private void enableChildrenVisibility(Area area)
        {


            foreach (Street street in retrieveData.Streets)
            {
                if (street.AreaID == area.LocationID)
                {
                    street.Visible = true;
                }
            }
        }

        private void enableAllVisibility(object sender, RoutedEventArgs e) {
            foreach (Site site in retrieveData.Sites) {
                site.Visible = true;
            }
            foreach (Street street in retrieveData.Streets)
            {
                street.Visible = true;
            }
            displayAllLocations();
        }

        // toggled de visibility van de sites van een straat
        private void toggleChildrenVisibility(Street street)
        {


            foreach (Site site in retrieveData.Sites)
            {
                if (site.StreetID == street.LocationID)
                {
                    site.Visible = !site.Visible;
                }

            }
        }

        private void enableChildrenVisibility(Street street)
        {


            foreach (Site site in retrieveData.Sites)
            {
                if (site.StreetID == street.LocationID)
                {
                    site.Visible = true;
                }

            }
        }

        // verbergt alle sites van de straat
        private void hideChildren(Street street)
        {
            foreach (Site site in retrieveData.Sites)
            {
                if (site.StreetID == street.LocationID)
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

        private Button createLocationButton(int size)
        {
            Button button = new Button();
            button.Content = $"+";
            button.Margin = new Thickness(size, 4, 4, 4);
            return button;
        }

        private Button createLocationButton(Site site)
        {
            Button button = new Button();
            button.Content = $"Plek {site.LocationID}";
            button.Margin = new Thickness(siteButtonMarginSize, 4, 4, 4);

            // De volledige campsite wordt meegegeven aan de button.
            // De tag kan opgevraagd worden om informatie op het rechter scherm te tonen.
            button.Tag = site;

            return button;
        }

        private Button createLocationButton(Street street)  
        {
            Button button = new Button();
            button.Content = $"Straat {street.LocationID}";
            button.Margin = new Thickness(64, 4, 4, 4);

            // De volledige campsite wordt meegegeven aan de button.
            // De tag kan opgevraagd worden om informatie op het rechter scherm te tonen.
            button.Tag = street;

            return button;
        }

        private Button createLocationButton(Area area)
        {
            Button button = new Button();
            button.Content = $"Gebied {area.LocationID}";
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
                new[] { SiteOverview, LocationInfo, SearchCampsiteList, AddReservationInfo, ReservationList, ReservationInfo },
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
            if (selectedButton == ReservationsButton) displayAllReservations();
        }


        private void displayAllReservations()
        {
            // zorgt ervoor dat de annuleerlijst weer null wordt wanneer
            // er opnieuw een reservering geselecteerd wordt
            // (anders bevat de lisjt reserveringen die niet zijn aangeklikt!)

            if (reservationIDFilterBox.Text != string.Empty || LastNameFilterBox.Text != string.Empty)
            {
                int resID;
                if (!int.TryParse(reservationIDFilterBox.Text, out resID))
                {
                    reservationIDFilterBox.Text = string.Empty;
                    resID = -1;
                }
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
            grid.MouseDown += (sender, e) => { changeReservation.fillReservationInfoGrid(reservation); };

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
                        label.Content = reservation.Visitor.LastName.ToString();
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

        private void RowClick(Reservation reservation)
        {
            changeReservation.isUpdating = true;
            selectedReservation = reservation;


            ReservationInfoGrid.Visibility = Visibility.Visible;

            displayAllReservations();


            changeReservation.enabledReservationInfoTextBoxes(new[] { SiteIDBox, FirstNameBox, PrepositionBox, LastNameBox, PhoneNumberBox, CityBox, AdressBox, HouseNumberBox, PostalCodeBox }, changeReservation.isUpdating);
            changeReservation.enabledReservationInfodatePicker(new[] { StartDateDatePicker, EndDatedatePicker }, changeReservation.isUpdating);

            changeReservation.hideErrors();
        }

        private void Un_Checkt(Reservation reservation, object sender)
        {
            CheckBox c = (CheckBox)sender;
            if (c.IsChecked == true && !toBeCancel.Contains(reservation))
            {
                toBeCancel.Add(reservation);
            }
            else if (c.IsChecked == false && toBeCancel.Contains(reservation))
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
            combinedString.Remove(combinedString.Length - 2);
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
            EditReservationClick?.Invoke(sender, new ChangeReservationEventArgs(selectedReservation));
        }
        private void CancelEditReservationButtonClick(object sender, RoutedEventArgs e)
        {
            changeReservation.fillReservationInfoGrid(selectedReservation);
        }

        private void Checkfields()
        {
            throw new NotImplementedException();
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

        private void NumberValidationTextBox(object sender, TextChangedEventArgs e)
        {
            string PostcodeHoofdletters = ((TextBox)sender).Text;
            if (!Regex.IsMatch(PostcodeHoofdletters.Trim(), "[0-9]+"))
            {
                ((TextBox)sender).Text = "";
            }

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
            if (Regex.IsMatch(houseNumber.Trim(), "^[1-9][0-9]*[a-z]{0,2}$"))
            {
                ((TextBox)sender).Foreground = Brushes.Black;
            }
            else { ((TextBox)sender).Foreground = Brushes.Red; }
        }


        private void PhoneNumberValidation(object sender, TextChangedEventArgs e)
        {

            string houseNumber = ((TextBox)sender).Text;
            if (Regex.IsMatch(houseNumber.Trim(), "^[0-9]\\d{1,15}$"))
            {
                ((TextBox)sender).Foreground = Brushes.Black;
            }
            else { ((TextBox)sender).Foreground = Brushes.Red; }

        }



        private void StringBox(object sender, TextChangedEventArgs e)
        {
            string LastNameFilterBox = ((TextBox)sender).Text;
            if (!Regex.IsMatch(LastNameFilterBox.Trim(), "[0-9a-zA-Z]"))
            {
                ((TextBox)sender).Text = "";
            }
        }


        private void fillAddReservationInfoGrid(int campSiteID, DateTime startDate, DateTime endDate)
        {
            AddReservationInfoGrid.Visibility = Visibility.Visible;

            AddResSiteIDBox.Content = campSiteID.ToString();
            AddResStartDateDatePicker.Content = startDate.ToString("MM-dd-yyyy");
            AddResEndDateDatePicker.Content = endDate.ToString("MM-dd-yyyy");

            AddResFirstNameBox.Text = "";
            AddResPrepositionBox.Text = "";
            AddResLastNameBox.Text = "";
            AddResPhoneNumberBox.Text = "";
            AddResCityBox.Text = "";
            AddResAdressBox.Text = "";

            AddResHouseNumberBox.Text = "";
            AddResPostalCodeBox.Text = "";
        }

        private void CancelReservationButtonClick(object sender, EventArgs e) {
            AddReservationInfoGrid.Visibility = Visibility.Hidden;
            AvailableCampsitesMap.Visibility = Visibility.Visible;
        }

        private void AddReservationButtonClick(object sender, EventArgs e)
        {
            bool errorsFound = false;

            if (!int.TryParse(AddResSiteIDBox.Content.ToString(), out int siteID))
            {
                AddResSiteIDBox.Visibility = Visibility.Visible;
                AddResSiteIDBox.Content = "Moet een getal zijn";
                AddResSiteIDBox.Foreground = Brushes.Red;
                errorsFound = true;
            }


            if (int.TryParse(AddResPhoneNumberBox.Text, out int phoneNumber))
            {
                AddResPhoneNumberLabel.Visibility = Visibility.Hidden;
            }
            else
            {
                AddResPhoneNumberLabel.Visibility = Visibility.Visible;
                AddResPhoneNumberLabel.Content = "Moet een getal zijn";
                AddResPhoneNumberLabel.Foreground = Brushes.Red;
                errorsFound = true;
            }


            Regex reg = new Regex("^[1-9][0-9]*[a-z]{0,2}$");
            if (reg.IsMatch(AddResHouseNumberBox.Text))
            {
                AddResHouseNumberLabel.Visibility = Visibility.Hidden;

            }
            else
            {
                AddResHouseNumberLabel.Visibility = Visibility.Visible;
                AddResHouseNumberLabel.Content = "2 letters max.";
                AddResHouseNumberLabel.Foreground = Brushes.Red;
                errorsFound = true;
            }


            Regex regex = new("^[1-9][0-9]{3}\\s?[a-zA-Z]{2}$");

            if (regex.IsMatch(AddResPostalCodeBox.Text) && AddResPostalCodeBox.Text.Length <= 6)
            {
                AddResPostalCodeLabel.Visibility = Visibility.Hidden;
            }
            else
            {
                AddResPostalCodeLabel.Visibility = Visibility.Visible;
                AddResPostalCodeLabel.Content = "Onjuiste formaat.";
                AddResPostalCodeLabel.Foreground = Brushes.Red;
                errorsFound = true;
            }


            if (AddResFirstNameBox.Text.Trim().IsNullOrEmpty())
            {
                AddResFirstNameLabel.Visibility = Visibility.Visible;
                AddResFirstNameLabel.Content = "Mag niet leeg zijn.";
                AddResFirstNameLabel.Foreground = Brushes.Red;
                errorsFound = true;
            }
            else
            {
                AddResFirstNameLabel.Visibility = Visibility.Hidden;
            }


            if (AddResLastNameBox.Text.Trim().IsNullOrEmpty())
            {
                AddResLastNameLabel.Visibility = Visibility.Visible;
                AddResLastNameLabel.Content = "Mag niet leeg zijn";
                AddResLastNameLabel.Foreground = Brushes.Red;
                errorsFound = true;
            }
            else
            {
                AddResLastNameLabel.Visibility = Visibility.Hidden;
            }

            if (AddResCityBox.Text.Trim().IsNullOrEmpty())
            {
                AddResCityLabel.Visibility = Visibility.Visible;
                AddResCityLabel.Content = "Mag niet leeg zijn";
                AddResCityLabel.Foreground = Brushes.Red;
                errorsFound = true;
            }
            else
            {
                AddResCityLabel.Visibility = Visibility.Hidden;
            }
            if (AddResAdressBox.Text.Trim().IsNullOrEmpty())
            {
                AddResAdressLabel.Visibility = Visibility.Visible;
                AddResAdressLabel.Content = "Mag niet leeg zijn";
                AddResAdressLabel.Foreground = Brushes.Red;
                errorsFound = true;
            }
            else
            {
                AddResAdressLabel.Visibility = Visibility.Hidden;
            }




            if (!errorsFound)
            {
                if (resData.addReservation(siteID, AddResStartDateDatePicker.Content.ToString(), AddResEndDateDatePicker.Content.ToString(),
                    AddResFirstNameBox.Text, AddResPrepositionBox.Text, AddResLastNameBox.Text,
                    AddResAdressBox.Text, AddResCityBox.Text, AddResPostalCodeBox.Text,
                    AddResHouseNumberBox.Text, phoneNumber))
                {
                    SearchCampsites.StartDateButton.SelectedDate = null;
                    SearchCampsites.EndDateButton.SelectedDate = null;
                    SearchCampsites.ShowSites = false;

                    AddReservationInfoGrid.Visibility = Visibility.Hidden;
                }
                
            }

        }


    }
}
