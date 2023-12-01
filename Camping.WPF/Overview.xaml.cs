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

        public Overview()
        {
            InitializeComponent();
            connection = new SshConnection();
            siteData = new SiteData();
            resData = new ReservationData();
            retrieveData = new RetrieveData(siteData, resData);

            displayAllSites();
            displayAlReservations();

            Closing += onWindowClosing;
        }

        // Laat alleen de areas zien
        private void displayAllSites()
        {
            CampSiteList.Children.Clear();
            CampSiteList.RowDefinitions.Clear();
            rowLength = 0;
            displayAreas();
        }

        // laat de areas zien
        private void displayAreas()
        {
            foreach (Area area in retrieveData.Areas)
            {


                addNewRowDefinition();

                Button button = createSiteButton(area);
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

                    Button button = createSiteButton(street);
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
                    

                    Button button = createSiteButton(site);

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
        private void onSitePress(object o) {
            if (o is Area && o is not null)
            {
                Area area = o as Area;
                SelectedSite = null;
                SelectedStreet = null;
                SelectedArea = area;
                toggleChildrenVisibility(area);
                displayAllSites();
            }
            else
            if (o is Street && o is not null)
            {
                Street street = o as Street;
                SelectedSite = null;
                SelectedStreet = street;
                SelectedArea = retrieveData.GetAreaFromID(SelectedStreet.AreaID);
                toggleChildrenVisibility(street);
                displayAllSites();
            }
            else
            if (o is Site && o is not null)
            {
                Site site = o as Site;
                SelectedSite = site;
                SelectedStreet = retrieveData.GetStreetFromID(site.StreetID);
                SelectedArea = retrieveData.GetAreaFromID(SelectedStreet.AreaID);
                displayAllSites();
            }
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

        private Button createSiteButton(Site site) {
            Button button = new Button();
            button.Content = $"Plek {site.CampSiteID}";
            button.Margin = new Thickness(272, 4, 4, 4);

            // De volledige campsite wordt meegegeven aan de button.
            // De tag kan opgevraagd worden om informatie op het rechter scherm te tonen.
            button.Tag = site;

            return button;
        }

        private Button createSiteButton(Street street)
        {
            Button button = new Button();
            button.Content = $"Straat {street.StreetID}";
            button.Margin = new Thickness(144, 4, 4, 4);

            // De volledige campsite wordt meegegeven aan de button.
            // De tag kan opgevraagd worden om informatie op het rechter scherm te tonen.
            button.Tag = street;

            return button;
        }

        private Button createSiteButton(Area area)
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

        private void displaySiteInformation(Site site)
        {
            SizeTextField.Content = site.Size;
            var colors = getFacilityColors(site);
            facilityList = new Ellipse[]{ HasWaterSupply, OutletPresent, PetsAllowed, HasShadow, AtWater };
            for (int i = 0; i < colors.Count && i < facilityList.Length; i++)
            {
                
                SolidColorBrush solidColorBrush = new SolidColorBrush(colors[i]);
                facilityList[i].Fill = solidColorBrush;

                facilityList[i].MouseLeftButtonDown += facilityClick;
            }
        }

        private void facilityClick(object sender, MouseButtonEventArgs e)
        {
            if (isUpdating)
            {
                // Update the color of the clicked ellipse based on your logic
                Ellipse clickedEllipse = (Ellipse)sender;
                int index = Array.IndexOf(facilityList, clickedEllipse);

                // Refresh the displayed information
                MessageBox.Show("clicked facility");
            }
        }

        private List<Color> getFacilityColors(Site site)
        {
            List<Color> colors = new List<Color>();

            colors.Add(site.HasWaterSupply ? Colors.Green : Colors.Red);
            colors.Add(site.OutletPresent ? Colors.Green : Colors.Red);
            colors.Add(site.PetsAllowed ? Colors.Green : Colors.Red);
            colors.Add(site.HasShadow ? Colors.Green : Colors.Red);
            colors.Add(site.AtWater ? Colors.Green : Colors.Red);
            
            return colors;
        }

        private void ChangeFacilitiesButtonClick(object sender, RoutedEventArgs e)
        {
            // Toggle the updating state
            isUpdating = !isUpdating;

            if (isUpdating)
            {
                ChangeFacilitiesButton.Content = "Opslaan";
            }
            else
            {
                ChangeFacilitiesButton.Content = "Aanpassen faciliteiten";
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
            List<Reservation> reservations = retrieveData.GetReservations();

            int i = 0;
            foreach (Reservation reservation in reservations)
            {
                
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(50);
                grid.RowDefinitions.Add(row);
                i++;
            }


            grid.ShowGridLines = true;
            ReservationListScrollViewer.Content = grid;
        }

        private void aanpassenOrSaveButtonClick(object sender, RoutedEventArgs e)
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
    }
}
