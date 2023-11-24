﻿
using camping.Core;
using camping.Database;
using camping.WPF;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Camping.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RetrieveData retrieveData;
        private ReservationData resData;
        private SiteData siteData;
        public MainWindow()
        {
            InitializeComponent();
            siteData = new SiteData();
            resData = new ReservationData();
            retrieveData = new(siteData, resData);
            initiateGrid();
        }

        RowDefinition rowDef1;
        TextBlock campSiteIDText;
        Button moreInfoButton;
        private void initiateGrid()
        {
            int numberOfRows = retrieveData.GetCampSiteID().Count();
            for (int i = 0; i < numberOfRows; i++)
            {
                rowDef1 = new RowDefinition();
                rowDef1.Height = new GridLength(50);
                grid.RowDefinitions.Add(rowDef1);

                campSiteIDText = new TextBlock();
                campSiteIDText.Text = $"{retrieveData.GetCampSiteID().ElementAt(i)}";
                Grid.SetColumn(campSiteIDText, 0);
                Grid.SetRow(campSiteIDText, i);
                campSiteIDText.HorizontalAlignment = HorizontalAlignment.Center;
                campSiteIDText.VerticalAlignment = VerticalAlignment.Center;

                //beschikbaarheid

                moreInfoButton = new Button();
                moreInfoButton.Content = "Meer informatie";
                Grid.SetColumn(moreInfoButton, 2);
                Grid.SetRow(moreInfoButton, i);
                moreInfoButton.HorizontalAlignment = HorizontalAlignment.Center;
                moreInfoButton.VerticalAlignment = VerticalAlignment.Center;
                moreInfoButton.Click += (sender, e) => MoreInfoButton_Click(campSiteIDText);

                grid.Children.Add(campSiteIDText);
                //beschikbaarheid
                grid.Children.Add(moreInfoButton);

            }
        }

        private void MoreInfoButton_Click(TextBlock campSiteIDText)
        {
            string campsiteID = campSiteIDText.Text;

            int surfaceArea = GetSurfaceAreaForCampsiteID(campsiteID);

            SiteInfo siteInfoWindow = new SiteInfo(campsiteID);

            siteInfoWindow.SetCampsiteInfo(campsiteID, surfaceArea);

            siteInfoWindow.Show();
        }

        private int GetSurfaceAreaForCampsiteID(string campsiteID)
        {
           
            List<int> surfaceAreas = retrieveData.GetSurfaceArea();

            int index = retrieveData.GetCampSiteID().IndexOf(Convert.ToInt32(campsiteID));

            return surfaceAreas[index];
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

        

        private void AddReservationButton_Click(object sender, RoutedEventArgs e)
        {
            ReservationDatesWindow rdw = new ReservationDatesWindow();

            rdw.ShowDialog();
        }
    }
}
