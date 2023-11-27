
using camping.Core;
using camping.Database;
using camping.WPF;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Camping.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SshConnection connection;

        private RetrieveData retrieveData;
        private ReservationData resData;
        private SiteData siteData;

        public MainWindow()
        {
            InitializeComponent();
           connection = new SshConnection();
            siteData = new SiteData();
            resData = new ReservationData();
            retrieveData = new(siteData, resData);
            UpdateGrid();
            Closing += OnWindowClosing;
        }

        public void UpdateGrid()
        {
            // Removes all existing items in grid
            grid.RowDefinitions.Clear();
            grid.Children.Clear();

            for (int i = 0; i < retrieveData.GetCampSiteID().Count(); i++)
            {
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(50);
                grid.RowDefinitions.Add(rowDef);

                TextBlock campSiteIDText = new TextBlock();
                campSiteIDText.Text = $"{retrieveData.GetCampSiteID().ElementAt(i)}";
                Grid.SetColumn(campSiteIDText, 0);
                Grid.SetRow(campSiteIDText, i);
                campSiteIDText.HorizontalAlignment = HorizontalAlignment.Center;
                campSiteIDText.VerticalAlignment = VerticalAlignment.Center;

                TextBlock campSiteAvailabilityText = new TextBlock();
                if (retrieveData.GetDate(retrieveData.GetCampSiteID().ElementAt(i))){
                    campSiteAvailabilityText.Text = $"✓";
                }
                else
                {
                    campSiteAvailabilityText.Text = $"X";
                }

                Grid.SetColumn(campSiteAvailabilityText, 1);
                Grid.SetRow(campSiteAvailabilityText, i);
                campSiteAvailabilityText.HorizontalAlignment = HorizontalAlignment.Center;
                campSiteAvailabilityText.VerticalAlignment = VerticalAlignment.Center;

                Button moreInfoButton = new Button();
                moreInfoButton.Content = "Meer informatie";
                Grid.SetColumn(moreInfoButton, 2);
                Grid.SetRow(moreInfoButton, i);
                moreInfoButton.HorizontalAlignment = HorizontalAlignment.Center;
                moreInfoButton.VerticalAlignment = VerticalAlignment.Center;
                moreInfoButton.Click += (sender, e) => MoreInfoButton_Click(campSiteIDText);

                grid.Children.Add(campSiteIDText);
                grid.Children.Add(campSiteAvailabilityText);
                grid.Children.Add(moreInfoButton);
            }
        }

        private void MoreInfoButton_Click(TextBlock campSiteIDText)
        {
            int campsiteID = Convert.ToInt32( campSiteIDText.Text);

            int surfaceArea = GetSurfaceAreaForCampsiteID(campsiteID);

            SiteInfo siteInfoWindow = new SiteInfo(campsiteID);

            siteInfoWindow.SetCampsiteInfo(campsiteID, surfaceArea);

            siteInfoWindow.Show();
        }

        private int GetSurfaceAreaForCampsiteID(int campsiteID)
        {

            List<int> surfaceAreas = retrieveData.GetSurfaceArea();

            int index = retrieveData.GetCampSiteID().IndexOf(Convert.ToInt32(campsiteID));

            return surfaceAreas[index];
        }



        private void AddReservationButton_Click(object sender, RoutedEventArgs e)
        {
            ReservationDatesWindow rdw = new ReservationDatesWindow();

            rdw.ShowDialog();

            UpdateGrid();
        }
<<<<<<< HEAD
=======

        public void UpdateWindow(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            connection.BreakConnection();
        }

        private void viewReservationsButton_Click(object sender, RoutedEventArgs e)
        {
            reservationView resView = new reservationView(retrieveData);
            resView.ShowDialog();
        }
>>>>>>> f947ad9b2e0221f7fb4a58ea2e1ef1ec3fe1d1fb
    }
}
