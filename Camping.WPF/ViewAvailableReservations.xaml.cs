using camping.Core;
using camping.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for ViewAvailableReservations.xaml
    /// </summary>
    public partial class ViewAvailableReservations : Window
    {
        private ReservationData resData;
        private SiteData siteData;
        public ViewAvailableReservations(DateTime startDate, DateTime endDate)
        {
            InitializeComponent();
            siteData = new SiteData();
            resData = new ReservationData();
            initiateGrid(startDate, endDate);
        }

        RowDefinition rowDef1;
        TextBlock campSiteIDText;
        TextBlock campSizeText;
        TextBlock campFacilityText;
        Button reserveButton;
        private void initiateGrid(DateTime startDate, DateTime endDate)
        {
            int rowNumber = 0;
            foreach (Site site in siteData.GetSiteInfo()) {

                // if a spot isnt available on those selected dates, prevent it from showing up
                if (!resData.GetAvailableReservation(site.CampSiteID, startDate.ToString("MM-dd-yyyy"), endDate.ToString("MM-dd-yyyy"))) continue;

                rowDef1 = new RowDefinition();
                rowDef1.Height = new GridLength(50);
                grid.RowDefinitions.Add(rowDef1);

                // campSite ID
                campSiteIDText = new TextBlock();
                campSiteIDText.Text = $"{site.CampSiteID}"; // campSiteID
                Grid.SetColumn(campSiteIDText, 0);
                Grid.SetRow(campSiteIDText, rowNumber);
                campSiteIDText.HorizontalAlignment = HorizontalAlignment.Center;
                campSiteIDText.VerticalAlignment = VerticalAlignment.Center;

                // grootte
                campSizeText = new TextBlock();
                campSizeText.Text = $"{site.Size}"; // grootte
                Grid.SetColumn(campSizeText, 1);
                Grid.SetRow(campSizeText, rowNumber);
                campSizeText.HorizontalAlignment = HorizontalAlignment.Center;
                campSizeText.VerticalAlignment = VerticalAlignment.Center;

                // grootte
                campSizeText = new TextBlock();
                campSizeText.Text = $"{startDate}"; // grootte
                Grid.SetColumn(campSizeText, 1);
                Grid.SetRow(campSizeText, rowNumber);
                campSizeText.HorizontalAlignment = HorizontalAlignment.Center;
                campSizeText.VerticalAlignment = VerticalAlignment.Center;


                // grootte
                campFacilityText = new TextBlock();
                campFacilityText.Text = $"{site.HasShadow} {site.HasWaterSupply} {site.AtWater} {site.PetsAllowed}"; // faciliteiten
                Grid.SetColumn(campFacilityText, 3);
                Grid.SetRow(campFacilityText, rowNumber);
                campFacilityText.HorizontalAlignment = HorizontalAlignment.Center;
                campFacilityText.VerticalAlignment = VerticalAlignment.Center;


                //reserveren
                reserveButton = new Button();
                reserveButton.Content = "Reserveren";
                Grid.SetColumn(reserveButton, 4);
                Grid.SetRow(reserveButton, rowNumber);
                reserveButton.Click += (sender, RoutedEventArgs) => { ReserveButton_Click(sender, RoutedEventArgs, site.CampSiteID, startDate, endDate); };
                reserveButton.HorizontalAlignment = HorizontalAlignment.Center;
                reserveButton.VerticalAlignment = VerticalAlignment.Center;

                grid.Children.Add(campSiteIDText);
                grid.Children.Add(campSizeText);
                grid.Children.Add(campFacilityText);
                grid.Children.Add(reserveButton);

                rowNumber++;
            }
            
        }

        

        private void addRow(object sender, RoutedEventArgs e)
        {
            rowDef1 = new RowDefinition();
            grid.RowDefinitions.Add(rowDef1);
        }

        private void clearRow(object sender, RoutedEventArgs e)
        {
            grid.RowDefinitions.Clear();
        }

        private void ReserveButton_Click(object sender, RoutedEventArgs e, int campSiteID, DateTime startDate, DateTime endDate)
        {
            AddReservationWindow ars = new(campSiteID, startDate.ToString("MM-dd-yyyy"), endDate.ToString("MM-dd-yyyy"));
            Close();
            ars.ShowDialog();

        }
    }
}
