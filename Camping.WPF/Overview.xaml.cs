using camping.Core;
using camping.Database;
using Camping.WPF;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        public Overview()
        {
            InitializeComponent();
            connection = new SshConnection();
            siteData = new SiteData();
            resData = new ReservationData();
            retrieveData = new RetrieveData(siteData, resData);

            displayAreas();
            displayStreets();
            displaySites();

            Closing += OnWindowClosing;
        }

        private void displayAreas() {
            int rowNumber = Grid.GetRowSpan(CampSiteList);
            foreach (Area area in retrieveData.Areas)
            {
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(50);
                CampSiteList.RowDefinitions.Add(rowDef);

                Button button = createSiteButton(area);

                Grid.SetRow(button, rowNumber);
                CampSiteList.Children.Add(button);
                rowNumber++;
            }
        }
        private void displayStreets()
        {
            int rowNumber = Grid.GetRowSpan(CampSiteList);
            foreach (Street street in retrieveData.Streets)
            {
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(50);
                CampSiteList.RowDefinitions.Add(rowDef);

                Button button = createSiteButton(street);

                Grid.SetRow(button, rowNumber);
                CampSiteList.Children.Add(button);
                rowNumber++;
            }
        }

        private void displaySites() {

            int rowNumber = Grid.GetRowSpan(CampSiteList);
            foreach (Site site in retrieveData.Sites)
            {
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(50);
                CampSiteList.RowDefinitions.Add(rowDef);

                Button button = new Button();
                button.Content = $"Plek {site.CampSiteID}";
                button.Margin = new Thickness(128, 4, 4, 4);

                
                //geeft site mee voor de info
                button.Click += (sender, e) => displaySiteInformation(site);
                Button createSiteButton = createSite(site);

                Grid.SetRow(button, rowNumber);
                CampSiteList.Children.Add(button);
                rowNumber++;
            }
        }

        private Button createSite(Site site) {
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

        private void displaySiteInformation(Site site)
        {
            SizeTextField.Content = site.Size;
            var colors = GetFacilityColors(site);
            facilityList = new Ellipse[]{ HasWaterSupply, OutletPresent, PetsAllowed, HasShadow, AtWater };
            for (int i = 0; i < colors.Count && i < facilityList.Length; i++)
            {
                
                SolidColorBrush solidColorBrush = new SolidColorBrush(colors[i]);
                facilityList[i].Fill = solidColorBrush;

                facilityList[i].MouseLeftButtonDown += Facility_Click;
            }
        }

        private void Facility_Click(object sender, MouseButtonEventArgs e)
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


        private List<Color> GetFacilityColors(Site site)
        {
            List<Color> colors = new List<Color>();

            colors.Add(site.HasWaterSupply ? Colors.Green : Colors.Red);
            colors.Add(site.OutletPresent ? Colors.Green : Colors.Red);
            colors.Add(site.PetsAllowed ? Colors.Green : Colors.Red);
            colors.Add(site.HasShadow ? Colors.Green : Colors.Red);
            colors.Add(site.AtWater ? Colors.Green : Colors.Red);
            
            return colors;
        }

        private void ChangeFacilitiesButton_Click(object sender, RoutedEventArgs e)
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
                SaveColors();
            }
        }

        private void SaveColors()
        {
            // Save the current colors or perform any other action
            // For example, you can save to a file or a data structure
        }
    }
}
