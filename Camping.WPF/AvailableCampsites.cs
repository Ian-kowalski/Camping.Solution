using camping.Core;
using camping.Database;
using Camping.Core;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace camping.WPF
{
    public class AvailableCampsites
    {
        public Grid grid;
        RowDefinition rowDef1;
        TextBlock campSiteIDText;
        TextBlock campSizeText;
        Rectangle campFacilityShadow;
        Rectangle campFacilityWater;
        Rectangle campFacilityAtWater;
        Rectangle campFacilityPets;
        Rectangle campFacilityOutlet;
        Button reserveButton;
        public AvailableCampsites(Grid grid, ISiteData siteData, IReservationData resData, DateTime startDate, DateTime endDate) {
            this.grid = grid;

            grid.Background = Brushes.White;
            

            grid.Children.Clear();
            grid.RowDefinitions.Clear();

            int rowNumber = 0;
            foreach (Site site in siteData.GetSiteInfo())
            {

                // if a spot isnt available on those selected dates, prevent it from showing up
                if (!resData.GetAvailableReservation(site.CampSiteID, startDate.ToString("MM-dd-yyyy"), endDate.ToString("MM-dd-yyyy"))) continue;

                rowDef1 = new RowDefinition();
                rowDef1.Height = new GridLength(50);
                grid.RowDefinitions.Add(rowDef1);

                // campSite ID
                campSiteIDText = new TextBlock();
                campSiteIDText.Text = $"{site.CampSiteID}"; // campSiteID
                campSiteIDText.FontSize = 20;
                campSiteIDText.FontWeight = FontWeights.Bold;
                Grid.SetColumn(campSiteIDText, 0);
                Grid.SetRow(campSiteIDText, rowNumber);
                campSiteIDText.HorizontalAlignment = HorizontalAlignment.Center;
                campSiteIDText.VerticalAlignment = VerticalAlignment.Center;

                // grootte
                campSizeText = new TextBlock();
                campSizeText.Text = $"{site.Size}"; // grootte
                campSizeText.FontSize = 16;
                Grid.SetColumn(campSizeText, 1);
                Grid.SetRow(campSizeText, rowNumber);
                campSizeText.HorizontalAlignment = HorizontalAlignment.Center;
                campSizeText.VerticalAlignment = VerticalAlignment.Center;

                // schaduw faliciteit
                campFacilityShadow = new Rectangle();
                createRectangle(site.HasShadow, campFacilityShadow);
                Grid.SetColumn(campFacilityShadow, 2);
                Grid.SetRow(campFacilityShadow, rowNumber);
                campFacilityShadow.HorizontalAlignment = HorizontalAlignment.Center;
                campFacilityShadow.VerticalAlignment = VerticalAlignment.Center;
                // water faliciteit
                campFacilityWater = new Rectangle();
                createRectangle(site.HasWaterSupply, campFacilityWater);
                Grid.SetColumn(campFacilityWater, 3);
                Grid.SetRow(campFacilityWater, rowNumber);
                campFacilityWater.HorizontalAlignment = HorizontalAlignment.Center;
                campFacilityWater.VerticalAlignment = VerticalAlignment.Center;
                // aan water faliciteit
                campFacilityAtWater = new Rectangle();
                createRectangle(site.AtWater, campFacilityAtWater);
                Grid.SetColumn(campFacilityAtWater, 4);
                Grid.SetRow(campFacilityAtWater, rowNumber);
                campFacilityAtWater.HorizontalAlignment = HorizontalAlignment.Center;
                campFacilityAtWater.VerticalAlignment = VerticalAlignment.Center;
                // dieren faliciteit
                campFacilityPets = new Rectangle();
                createRectangle(site.PetsAllowed, campFacilityPets);
                Grid.SetColumn(campFacilityPets, 5);
                Grid.SetRow(campFacilityPets, rowNumber);
                campFacilityPets.HorizontalAlignment = HorizontalAlignment.Center;
                campFacilityPets.VerticalAlignment = VerticalAlignment.Center;
                // stroom faliciteit
                campFacilityOutlet = new Rectangle();
                createRectangle(site.OutletPresent, campFacilityOutlet);
                Grid.SetColumn(campFacilityOutlet, 6);
                Grid.SetRow(campFacilityOutlet, rowNumber);
                campFacilityOutlet.HorizontalAlignment = HorizontalAlignment.Center;
                campFacilityOutlet.VerticalAlignment = VerticalAlignment.Center;





                //reserveren
                reserveButton = new Button();
                reserveButton.Background = new SolidColorBrush(Color.FromRgb(200, 200, 200));
                reserveButton.FontSize = 20;
                reserveButton.Content = "Reserveren";
                reserveButton.BorderBrush = Brushes.Black;
                reserveButton.BorderThickness = new Thickness(2);
                reserveButton.Height = 40;
                reserveButton.Width = 120;
                Grid.SetColumn(reserveButton, 7);
                Grid.SetRow(reserveButton, rowNumber);
                reserveButton.Click += (sender, RoutedEventArgs) => { ReserveButton_Click(sender, RoutedEventArgs, site.CampSiteID, startDate, endDate); };
                reserveButton.HorizontalAlignment = HorizontalAlignment.Center;
                reserveButton.VerticalAlignment = VerticalAlignment.Center;
                reserveButton.Margin = new Thickness(0, 0, 0, 0);

                grid.Children.Add(campSiteIDText);
                grid.Children.Add(campSizeText);
                grid.Children.Add(campFacilityShadow);
                grid.Children.Add(campFacilityWater);
                grid.Children.Add(campFacilityAtWater);
                grid.Children.Add(campFacilityPets);
                grid.Children.Add(campFacilityOutlet);
                grid.Children.Add(reserveButton);

                rowNumber++;
            }

            grid.Visibility = Visibility.Visible;

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

        private Rectangle createRectangle(int color, Rectangle rect)
        {
            rect.Height = 30;
            rect.Width = 30;

            rect.Stroke = Brushes.Black;
            rect.StrokeThickness = 2;

            if (color % 2 == 1)
            {
                rect.Fill = new SolidColorBrush(Color.FromRgb(100, 255, 100));
            }
            else
            {
                rect.Fill = new SolidColorBrush(Color.FromRgb(255, 100, 100));
            }

            return rect;
        }

        private void ReserveButton_Click(object sender, RoutedEventArgs e, int campSiteID, DateTime startDate, DateTime endDate)
        {
            AddReservationWindow ars = new(campSiteID, startDate.ToString("MM-dd-yyyy"), endDate.ToString("MM-dd-yyyy"));
            ars.ShowDialog();

        }
    }
}
