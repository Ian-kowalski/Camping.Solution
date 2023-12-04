using camping.Core;
using camping.Database;
using Camping.WPF;
using DevExpress.Utils.About;
using DevExpress.XtraExport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static DevExpress.Data.Helpers.ExpressiveSortInfo;

namespace camping.WPF
{
    /// <summary>
    /// Interaction logic for Overview.xaml
    /// </summary>
    public partial class Overview : Window
    {
        private SshConnection connection { get; set; }
        private SiteData siteData { get; set; }
        private ReservationData resData { get; set; }
        private RetrieveData retrieveData { get; set; }

        private Location tempLocation;
        private Site currentSelected {  get; set; }
        private bool isUpdating { get; set; }
        private bool ReservationAanpassenButtonState { get; set; } = false; //true save : false aanpassen

        private int rowLength;

        private Area? SelectedArea;

        private Street? SelectedStreet;

        private Site? SelectedSite;


        private Reservation? selectedReservation = new Reservation();

        private List<Reservation> toBeCancel = new List<Reservation>();


        private Location selectedLocation;
        private Button changeFacilitiesButton;


        public Overview()
        {
            InitializeComponent();
            connection = new SshConnection();
            siteData = new SiteData();
            resData = new ReservationData();
            retrieveData = new RetrieveData(siteData, resData);

            StartDateButton.DisplayDateStart = DateTime.Today;
            EndDateButton.DisplayDateStart = DateTime.Today;

            displayAllLocations();
            displayAllReservations();



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
                button.Click += (sender, e) => { onSitePress(area); };

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
                    button.Click += (sender, e) => { onSitePress(street); };

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

                    button.Click += (sender, e) => { onSitePress(site); };

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
                selectedLocation = area;
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
                selectedLocation = street;
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
                selectedLocation = site;
                displayAllLocations();
            }
            displayInformation(location);
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

        private void displayInformation(Location location)
        {
            if (location is Area) {
                Area area = location as Area;
                Area tempArea = new(area.AreaID, area.OutletPresent, area.AtWater, area.PetsAllowed, area.HasShadow, area.HasWaterSupply);
                tempLocation = tempArea;
            }
            if (location is Street)
            {
                Street street = location as Street;
                Street tempStreet = new(street.StreetID, street.AreaID, street.OutletPresent, street.AtWater, street.PetsAllowed, street.HasShadow, street.HasWaterSupply);
                tempLocation = tempStreet;
            }
            if (location is Site)
            {
                Site site = location as Site;
                Site tempSite = new(site.CampSiteID, site.OutletPresent, site.AtWater, site.PetsAllowed, site.HasShadow, site.HasWaterSupply, site.Size, site.StreetID);
                tempLocation = tempSite;
            }
            LocationInfoGrid.Children.Clear();

            CreateAndAddLabel("Gebiedx/straatx/plekx", 16, 0, 0);
            CreateAndAddLabel("Faciliteiten", 24, 0, 2);
            CreateAndAddLabel("Overig", 24, 0, 3);

            if (location is Site)
            {
                CreateAndAddLabel("Oppervlak: ", 24, 0, 1);
                CreateAndAddLabel(Convert.ToString(((Site)location).Size), 24, 1, 1);
            }

            CreateAndAddFacility("HasWaterSupply", 60, 1, 2, location);
            CreateAndAddFacility("OutletPresent", 60, 2, 2, location);
            CreateAndAddFacility("HasShadow", 60, 1, 3, location);
            CreateAndAddFacility("AtWater", 60, 2, 3, location);
            CreateAndAddFacility("PetsAllowed", 60, 3, 3, location);

            isUpdating = false;
            Button ChangeFacilitiesButton = new Button();
            ChangeFacilitiesButton.Content = "Faciliteiten aanpassen";
            ChangeFacilitiesButton.HorizontalAlignment = HorizontalAlignment.Center;
            ChangeFacilitiesButton.VerticalAlignment = VerticalAlignment.Center;

            ChangeFacilitiesButton.Width = 180;
            ChangeFacilitiesButton.Height = 60;
            ChangeFacilitiesButton.BorderBrush = Brushes.Black;
            ChangeFacilitiesButton.BorderThickness = new Thickness(2);
            ChangeFacilitiesButton.FontSize = 16;

            ChangeFacilitiesButton.Click += (sender, e) => { ChangeFacilitiesButtonClick(ChangeFacilitiesButton); };

            Grid.SetRow(ChangeFacilitiesButton, 3);
            Grid.SetColumn(ChangeFacilitiesButton, 4);
            LocationInfoGrid.Children.Add(ChangeFacilitiesButton);
        }

        private void CreateAndAddLabel(string content, int fontSize, int column, int row)
        {
            Label dynamicLabel = new Label
            {
                Content = content,
                FontSize = fontSize,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetColumn(dynamicLabel, column);
            Grid.SetRow(dynamicLabel, row);

            LocationInfoGrid.Children.Add(dynamicLabel);
        }

        private void CreateAndAddFacility(string name, int diameter, int column, int row, Location location)
        {
            Ellipse facility = new Ellipse
            {
                Name = name,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = diameter,
                Height = diameter
            };
            

            Grid.SetColumn(facility, column);
            Grid.SetRow(facility, row);

            var color = GetFacilityColor(facility);

            SolidColorBrush solidColorBrush = new SolidColorBrush(color);
            facility.Fill = solidColorBrush;
            facility.MouseLeftButtonDown += facilityClick;

            LocationInfoGrid.Children.Add(facility);
        }

        private void facilityClick(object sender, MouseButtonEventArgs e)
        {
            if (isUpdating)
            {
                Ellipse clickedEllipse = (Ellipse)sender;
                SolidColorBrush solidColorBrush = new SolidColorBrush();
 
                ChangeFacilityColor(clickedEllipse);
                solidColorBrush.Color = GetFacilityColor(clickedEllipse);
                clickedEllipse.Fill = solidColorBrush;
            }
        }


        private Color GetFacilityColor(Ellipse facility)
        {
            Color color = Colors.Red;

            if (facility.Name == "HasWaterSupply" && tempLocation.HasWaterSupply) color = Colors.Green;
            else if (facility.Name == "OutletPresent" && tempLocation.OutletPresent) color = Colors.Green;
            else if (facility.Name == "PetsAllowed" && tempLocation.PetsAllowed) color = Colors.Green;
            else if (facility.Name == "HasShadow" && tempLocation.HasShadow) color = Colors.Green;
            else if (facility.Name == "AtWater" && tempLocation.AtWater) color = Colors.Green;

            return color;
        }

        private void ChangeFacilityColor(Ellipse facility)
        {
            if (facility.Name == "HasWaterSupply") tempLocation.HasWaterSupply = !tempLocation.HasWaterSupply;
            else if (facility.Name == "OutletPresent") tempLocation.OutletPresent = !tempLocation.OutletPresent;
            else if (facility.Name == "PetsAllowed") tempLocation.PetsAllowed = !tempLocation.PetsAllowed;
            else if (facility.Name == "HasShadow") tempLocation.HasShadow = !tempLocation.HasShadow;
            else if (facility.Name == "AtWater") tempLocation.AtWater = !tempLocation.AtWater;
        }

        private void ChangeFacilitiesButtonClick(Button button)
        {
            // Toggle the updating state
            isUpdating = !isUpdating;

            if (isUpdating)
            {
                button.Content = "Opslaan";
            }
            else
            {
                button.Content = "Faciliteiten aanpassen";
                siteData.UpdateFacilities(tempLocation);
                retrieveData.UpdateLocations();
                
            }          
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
            Grid grid = new Grid();
            for (int counter = 0; counter < 7; counter++)
            {
                ColumnDefinition col = new ColumnDefinition();
                if (counter > 2)
                {
                    col.Width = new GridLength(2, GridUnitType.Star);
                }
                grid.ColumnDefinitions.Add(col);
            }
            List<Reservation> reservations = retrieveData.Reservations;
            
            int i = 0;
            foreach (Reservation reservation in reservations)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(30);
                grid.RowDefinitions.Add(row);

                AddCheckbox(grid, reservation, i);

                AddID(grid, reservation, i);
                AddSiteID(grid, reservation, i);
                AddLastName(grid, reservation, i);
                AddStartDate(grid, reservation, i);
                AddEndDate(grid, reservation, i);
                AddEditReservationButton(grid, reservation, i);
                i++;
            }


            grid.ShowGridLines = false;
            ReservationListScrollViewer.Content = grid;
        }


        private void fillReservationInfoGrid(Reservation reservation)
        {
            selectedReservation = reservation;


            SiteIDBox.Text = reservation.ReservationID.ToString();
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

        private void addCancelCheckBoxColum(Grid grid, int i, Reservation reservation)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.Checked += (sender, e) => { Un_Checkt(reservation ,sender); };
            checkBox.Unchecked += (sender, e) => { Un_Checkt(reservation, sender); };

            Grid.SetColumn(checkBox, 0);
            Grid.SetRow(checkBox, i);
            checkBox.HorizontalAlignment = HorizontalAlignment.Center;
            checkBox.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(checkBox);
        }

        private void Un_Checkt(Reservation reservation, object sender)
        {
            CheckBox c = (CheckBox)sender;
            if (c.IsChecked == true)
            {
                toBeCancel.Add(reservation);
            }
            else
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
            string combinedString = "";
            foreach (var reservation in toBeCancel)
            {
                combinedString += reservation.ReservationID.ToString();
            }
            string messageBoxText = "Do you want to cancel these reservation(s): " + combinedString;
            string caption = "Annuleren reservering(en)";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;

            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    // User pressed Yes button
                    foreach (var reservation in toBeCancel)
                    {
                        // ...Delete out of database
                        retrieveData.DeleteReservation(reservation.ReservationID);
                    }
                    toBeCancel.Clear();
                    break;
                case MessageBoxResult.No:
                    // User pressed No button
                    // ...Nothing
                    toBeCancel.Clear();
                    break;
            }
            displayAllReservations();
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

            chanceAanpassenOrSaveButtonContent(sender);
            enabledReservationInfoTextBoxes(new[] { SiteIDBox, FirstNameBox, PrepositionBox, LastNameBox, PhoneNumberBox, CityBox, AdressBox, HouseNumberBox, PostalCodeBox });
            enabledReservationInfodatePicker(new[] { StartDateDatePicker, EndDatedatePicker });
        }

        private bool saveReservation(Reservation reservation)
        {
            var result = MessageBox.Show("Weet je zeker dat je de reservatie gegevens wilt aanpassen?", "Confirm", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (int.Parse(SiteIDBox.Text) > retrieveData.GetCampSiteID().Count())
                    {
                        MessageBox.Show("Deze plek bestaat niet.\nKies een ID tussen 1 en " + retrieveData.GetCampSiteID().Count() + ".");
                        return false;
                    }
                    else reservation.SiteID = int.Parse(SiteIDBox.Text);
                } catch
                {
                    MessageBox.Show("Verkeerde waarde ingevuld bij 'Plaats nr'.\nMoet een getal zijn.");
                }
                try
                {
                    reservation.Guest.PhoneNumber = Int32.Parse(PhoneNumberBox.Text);
                }
                catch
                {
                    MessageBox.Show("Verkeerde waarde ingevuld bij 'Telefoonnummer'.\nMoet een getal zijn.");
                    return false;
                }
                try
                {
                    reservation.Guest.PhoneNumber = Int32.Parse(HouseNumberBox.Text);
                }
                catch
                {
                    MessageBox.Show("Verkeerde waarde ingevuld bij 'Huisnummer'.\nMoet een getal zijn.");
                    return false;
                }

                Regex regex = new("[1-9][0-9]{3}[A-Z]{2}");
                if (regex.IsMatch(PostalCodeBox.Text) && PostalCodeBox.Text.Length <= 6)
                {
                    reservation.Guest.PostalCode = PostalCodeBox.Text;
                }
                else
                {
                    MessageBox.Show("Onjuiste formaat bij postcode ingevuld. Moet vier getallen en twee letters zijn, bijv: '1234AB'.");
                    return false;
                }

                reservation.Guest.FirstName = FirstNameBox.Text;
                reservation.Guest.Preposition = PrepositionBox.Text;
                reservation.Guest.LastName = LastNameBox.Text;
                reservation.Guest.City = CityBox.Text;
                reservation.Guest.Adress = AdressBox.Text;


                if (!retrieveData.GetOtherAvailableReservations(reservation.SiteID, StartDateDatePicker.SelectedDate.GetValueOrDefault().ToString("MM-dd-yyyy"), EndDatedatePicker.SelectedDate.GetValueOrDefault().ToString("MM-dd-yyyy"), reservation.ReservationID))
                {
                    MessageBox.Show("De ingevulde datums zijn niet beschikbaar.");
                    return false ;
                }
                else if (Convert.ToDateTime(EndDatedatePicker.Text) < DateTime.Today)
                {
                    MessageBox.Show("Einddatum kan niet in het verleden zijn.");
                    return false;
                }
                else if (Convert.ToDateTime(StartDateDatePicker.Text) <= Convert.ToDateTime(EndDatedatePicker.Text))
                {
                    reservation.StartDate = Convert.ToDateTime(StartDateDatePicker.Text);
                    reservation.EndDate = Convert.ToDateTime(EndDatedatePicker.Text);
                }
                else
                {
                    MessageBox.Show("Begindatum kan niet voor de einddatum komen.");
                    return false;
                }

                retrieveData.UpdateReservation(reservation.ReservationID, reservation.StartDate, reservation.Guest, reservation.EndDate, reservation.SiteID);
                return true;
            }
            return false;
        }


        private void chanceAanpassenOrSaveButtonContent(object sender)
        {
            ReservationAanpassenButtonState = !ReservationAanpassenButtonState;
            ((Button)sender).Content = ReservationAanpassenButtonState ? "save" : "Aanpassen Resevering";
        }

        private void enabledReservationInfoTextBoxes(TextBox[] TextBoxElements)
        {
            foreach (UIElement element in TextBoxElements)
            {
                element.IsEnabled = !element.IsEnabled;
            }
        }

        private void enabledReservationInfodatePicker(DatePicker[] TextBoxElements)
        {
            foreach (UIElement element in TextBoxElements)
            {
                element.IsEnabled = !element.IsEnabled;
            }
        }

        private void RowClick(Reservation reservation)
        {
            selectedReservation = reservation;

            //MessageBox.Show( reservation.ReservationID.ToString());
            ReservationInfoGrid.Visibility = Visibility.Visible;
            fillReservationInfoGrid(reservation);

            displayAllReservations();
           
        }

        private void StartDateButton_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            EndDateButton.DisplayDateStart = StartDateButton.SelectedDate;
        }

        private void EndDateButton_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            StartDateButton.DisplayDateEnd = EndDateButton.SelectedDate;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            AvailableCampsites availableCampsites = new AvailableCampsites(AddReservationGridList, siteData, resData, StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today), EndDateButton.SelectedDate.GetValueOrDefault(DateTime.Today));
            //MessageBox.Show($"zoeken voor \n{StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today)} tot {EndDateButton.SelectedDate.GetValueOrDefault(DateTime.Today)}");
        }
        
        private void AddCheckbox(Grid grid, Reservation reservation, int i)
        {
            CheckBox CB = new CheckBox();
            CB.Checked += CB_checkt;
            CB.Unchecked += CB_checkt;
            CB.Name = "CB" + reservation.ReservationID.ToString();
            Grid.SetColumn(CB, 0);
            Grid.SetRow(CB, i);
            CB.HorizontalAlignment = HorizontalAlignment.Center;
            CB.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(CB);
        }

        private void AddID(Grid grid, Reservation reservation, int i)
        {
            TextBlock TB = new TextBlock();
            TB.Text = reservation.ReservationID.ToString();
            Grid.SetColumn(TB, 1);
            Grid.SetRow(TB, i);
            TB.HorizontalAlignment = HorizontalAlignment.Center;
            TB.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(TB);
        }

        private void AddSiteID(Grid grid, Reservation reservation, int i)
        {
            TextBlock TB = new TextBlock();
            TB.Text = reservation.SiteID.ToString();

            Grid.SetColumn(TB, 2);
            Grid.SetRow(TB, i);
            TB.HorizontalAlignment = HorizontalAlignment.Center;
            TB.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(TB);
        }

        private void AddLastName(Grid grid, Reservation reservation, int i)
        {
            TextBlock TB = new TextBlock();
            TB.Text = reservation.Guest.LastName.ToString();
            Grid.SetColumn(TB, 3);
            Grid.SetRow(TB, i);
            TB.HorizontalAlignment = HorizontalAlignment.Center;
            TB.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(TB);
        }

        private void AddStartDate(Grid grid, Reservation reservation, int i)
        {
            TextBlock TB = new TextBlock();
            TB.Text = reservation.StartDate.ToShortDateString();
            Grid.SetColumn(TB, 4);
            Grid.SetRow(TB, i);
            TB.HorizontalAlignment = HorizontalAlignment.Center;
            TB.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(TB);
        }

        private void AddEndDate(Grid grid, Reservation reservation, int i)
        {
            TextBlock TB = new TextBlock();
            TB.Text = reservation.EndDate.ToShortDateString();
            Grid.SetColumn(TB, 5);
            Grid.SetRow(TB, i);
            TB.HorizontalAlignment = HorizontalAlignment.Center;
            TB.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(TB);
        }
        private void AddEditReservationButton(Grid grid, Reservation reservation, int i)
        {
            Button TB = new();
            TB.Content = "Bewerken";
            Grid.SetColumn(TB, 6);
            Grid.SetRow(TB, i);
            TB.Click += (sender, e) => { fillReservationInfoGrid(reservation); };
            TB.HorizontalAlignment = HorizontalAlignment.Center;
            TB.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(TB);
        }
        private void CB_checkt(object sender, RoutedEventArgs e)
        {
            CheckBox c = sender as CheckBox;
            int last_part = int.Parse(c.Name.Remove(0, 2));
            if (c.IsChecked == true)
            {
                //toBeCancel.Add(last_part);
            }
            else
            {
              //  toBeCancel.Remove(last_part);
            }
           // if (toBeCancel.Count != 0)
           // {
          //      CancelButton.IsEnabled = true;
          //  }
          //  else
          //  {
         //       CancelButton.IsEnabled = false;
         //   }
        }
    }
}
