using camping.Core;
using camping.Database;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

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

        private int rowLength;

        public Overview()
        {
            InitializeComponent();
            connection = new SshConnection();
            siteData = new SiteData();
            resData = new ReservationData();
            retrieveData = new RetrieveData(siteData, resData);

            displayAllSites();

            Closing += OnWindowClosing;
        }

        // Laat alleen de areas zien
        private void displayAllSites()
        {
            CampSiteList.Children.Clear();
            rowLength = 0;
            displayAreas();
        }

        // laat de areas zien
        private void displayAreas()
        {
            foreach (Area area in retrieveData.Areas)
            {

                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(50);
                CampSiteList.RowDefinitions.Add(rowDef);

                Button button = createSiteButton(area);
                button.Click += (sender, e) => { toggleChildrenVisibility(area); };

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
                    RowDefinition rowDef = new RowDefinition();
                    rowDef.Height = new GridLength(50);
                    CampSiteList.RowDefinitions.Add(rowDef);

                    Button button = createSiteButton(street);
                    button.Click += (sender, e) => { toggleChildrenVisibility(street); };

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


                    RowDefinition rowDef = new RowDefinition();
                    rowDef.Height = new GridLength(50);
                    CampSiteList.RowDefinitions.Add(rowDef);

                    Button button = createSiteButton(site);


                    Grid.SetRow(button, rowLength);
                    CampSiteList.Children.Add(button);
                    rowLength++;
                }
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

                    if (street.Visible) continue;
                    // als de straat verborgen wordt, verberg ook de sites
                    hideChildren(street);
                }

            }

            displayAllSites();
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

            displayAllSites();
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


        private Button createSiteButton(Site site)
        {
            Button button = new Button();
            button.Content = $"Plek {site.CampSiteID}";
            button.Margin = new Thickness(142, 4, 4, 4);

            // De volledige campsite wordt meegegeven aan de button.
            // De tag kan opgevraagd worden om informatie op het rechter scherm te tonen.
            button.Tag = site;

            return button;
        }
        private Button createSiteButton(Street street)
        {
            Button button = new Button();
            button.Content = $"Straat {street.StreetID}";
            button.Margin = new Thickness(80, 4, 4, 4);

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

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            connection.BreakConnection();
        }

        private void tabButtonClick(object sender, RoutedEventArgs e)
        {
            SetButtonState((Button)sender,
                new[] { SiteOverview, SiteInfo, AddReservationList, AddReservationInfo, ReservationList, ReservationInfo },
                new[] { SiteControlButton, AddReservationButton, ReservationsButton }
                );
        }

        private void SetButtonState(Button selectedButton, UIElement[] BorderElements, Button[] buttons)
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

        private void AanpassenOrSaveButtonClick(object sender, RoutedEventArgs e)
        {
            EnabledReservationInfoTextBoxes(new[] { SiteIDBox, FirstNameBox, PrepositionBox, LastNameBox, PhoneNumberBox, CityBox, AdressBox, HouseNumberBox, PostalCodeBox });
            EnabledReservationInfodatePicker(new[] { StartDateDatePicker, EndDatedatePicker });
        }

        private void EnabledReservationInfoTextBoxes(TextBox[] TextBoxElements)
        {
            foreach (UIElement element in TextBoxElements)
            {
                element.IsEnabled = !element.IsEnabled;
            }
        }

        private void EnabledReservationInfodatePicker(DatePicker[] TextBoxElements)
        {
            foreach (UIElement element in TextBoxElements)
            {
                element.IsEnabled = !element.IsEnabled;
            }
        }
    }
}
