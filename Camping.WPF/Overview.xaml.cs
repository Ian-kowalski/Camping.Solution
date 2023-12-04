using camping.Core;
using camping.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
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
        private Ellipse[] facilityList {  get; set; }
        private Site currentSelected {  get; set; }
        private bool isUpdating { get; set; }
        private bool ReservationAanpassenButtonState { get; set; } = false; //true save : false aanpassen

        private int rowLength;

        private Area? SelectedArea;

        private Street? SelectedStreet;

        private Site? SelectedSite;

        private Location selectedLocation;
        private Button changeFacilitiesButton;

        public Overview()
        {
            InitializeComponent();
            connection = new SshConnection();
            siteData = new SiteData();
            resData = new ReservationData();
            retrieveData = new RetrieveData(siteData, resData);

            displayAllLocations();
            displayAlReservations();


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
                if (street.AreaID == areaID && street.Visible) {

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
        private void onSitePress(Location location) {
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


            foreach (Site site in retrieveData.Sites) {
                if (site.StreetID == street.StreetID) { 
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

        private void addNewRowDefinition() {
            RowDefinition rowDef = new RowDefinition();
            rowDef.Height = new GridLength(50);
            CampSiteList.RowDefinitions.Add(rowDef);
        }



        private Button createLocationButton(Site site) {
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

            LocationInfoGrid.Children.Clear();

            CreateAndAddLabel("Gebiedx/straatx/plekx", 16, 0, 0);
            CreateAndAddLabel("Faciliteiten", 24, 0, 2);
            CreateAndAddLabel("Overig", 24, 0, 3);

            if (location is Site) {
                CreateAndAddLabel("Oppervlak: ", 24, 0, 1);
                CreateAndAddLabel(Convert.ToString(((Site)location).Size), 24, 1, 1);
            }

            CreateAndAddFacility("HasWaterSupply", 60, 1, 2, location);
            CreateAndAddFacility("OutletPresent", 60, 2, 2, location);
            CreateAndAddFacility("HasShadow", 60, 1, 3, location);
            CreateAndAddFacility("AtWater", 60, 2, 3, location);
            CreateAndAddFacility("PetsAllowed", 60, 3, 3, location);

            Button ChangeFacilitiesButton = new Button();
            ChangeFacilitiesButton.Content = "Aanpassen";
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

            var color = GetFacilityColor(location, facility);

                SolidColorBrush solidColorBrush = new SolidColorBrush(color);
                facility.Fill = solidColorBrush;
                facility.MouseLeftButtonDown += facilityClick;

            LocationInfoGrid.Children.Add(facility);
        }

        private void facilityClick(object sender, MouseButtonEventArgs e)
        {
            if (isUpdating)
            {
                // Update the color of the clicked ellipse based on your logic
                Ellipse clickedEllipse = (Ellipse)sender;


                // Refresh the displayed information
                MessageBox.Show(clickedEllipse.Name);
            }
        }


        private Color GetFacilityColor(Location location, Ellipse facility)
        {
            Color color = Colors.Red; // Default color

            if (facility.Name == "HasWaterSupply" && location.HasWaterSupply) color = Colors.Green;
            else if (facility.Name == "OutletPresent" && location.OutletPresent) color = Colors.Green;
            else if (facility.Name == "PetsAllowed" && location.PetsAllowed) color = Colors.Green;
            else if (facility.Name == "HasShadow" && location.HasShadow) color = Colors.Green;
            else if (facility.Name == "AtWater" && location.AtWater) color = Colors.Green;
            
            return color;
        }

        private void ChangeFacilitiesButtonClick(Button button)
        {
            // Toggle the updating state
            isUpdating = !isUpdating;

            if (isUpdating)
            {
                changeFacilitiesButton.Content = "Opslaan";
            }
            else
            {
                changeFacilitiesButton.Content = "Aanpassen faciliteiten";
                saveColors();
            }
        }

        private void saveColors()
        {
            // Save the current colors or perform any other action
            // For example, you can save to a file or a data structure
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

        private void displayAlReservations()
        {
            Grid grid = new Grid();
            for (int counter = 0; counter < 6; counter++)
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
                i++;
            }


            grid.ShowGridLines = true;
            ReservationListScrollViewer.Content = grid;
        }

        private void EditReservationButtonClick(object sender, RoutedEventArgs e)
        {
            if (ReservationAanpassenButtonState)
            {
                //TODO: check data in text fields and send to database
                Checkfields();
                saveReservation();
            }

            chanceAanpassenOrSaveButtonContent(sender);
            enabledReservationInfoTextBoxes(new[] { SiteIDBox, FirstNameBox, PrepositionBox, LastNameBox, PhoneNumberBox, CityBox, AdressBox, HouseNumberBox, PostalCodeBox });
            enabledReservationInfodatePicker(new[] { StartDateDatePicker, EndDatedatePicker });
        }

        private void saveReservation()
        {
            throw new NotImplementedException();
        }

        private void Checkfields()
        {
            throw new NotImplementedException();
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
