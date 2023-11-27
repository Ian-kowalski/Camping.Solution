using camping.Database;
using Camping.Core;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;
using camping.Core;

namespace Camping.WPF
{
   
    public partial class SiteInfo : Window
    {
        private SiteData siteData;
        private ReservationData reservationData;
        private RetrieveData retrieveData;
        private bool checkboxesEnabled = false;
        private int campsiteID;
        public SiteInfo(int campsiteID)
        {
            InitializeComponent();
            siteData = new SiteData();
            reservationData = new ReservationData();
            retrieveData = new RetrieveData(siteData, reservationData);
            this.campsiteID = campsiteID;
            DisableCheckboxes();
            LoadSiteInfo(campsiteID);
        }
        public void SetCampsiteInfo(int campsiteID, int surfaceArea)
        {
            // Assuming campsiteIDLabel is a Label in your SiteInfo window
            campsiteIDLabel.Content = campsiteID;

            TextBlock campSiteAvailabilityText = new TextBlock();
            if (retrieveData.GetDate(campsiteID))
            {
                availability.Content = $"✓";
            }
            else
            {
                availability.Content = $"X";
            }

            // Assuming surfaceAreaLabel is a Label with Tag="6" in your SiteInfo window
            surfaceAreaLabel.Content = surfaceArea.ToString();
        }
        private void LoadSiteInfo(int campsiteID)
        {
            List<Site> sites = siteData.GetSiteInfo();

            foreach (Site site in sites)
            {
                // Check if the site.Number matches the campsiteID
                if (site.CampSiteID == campsiteID)
                {
                    SetCheckboxState(site.CampSiteID, site);
                }
            }
        }

        private void SetCheckboxState(int number, Site site)
        {
            CheckBox craneCheckbox = FindCheckBoxByNumberAndFacility(number, "Crane");
            CheckBox powerCheckbox = FindCheckBoxByNumberAndFacility(number, "Power");
            CheckBox animalCheckbox = FindCheckBoxByNumberAndFacility(number, "Animal");
            CheckBox shadowCheckbox = FindCheckBoxByNumberAndFacility(number, "Shadow");
            CheckBox waterCheckbox = FindCheckBoxByNumberAndFacility(number, "Water");

            if (site.HasWaterSupply)
            {
                craneCheckbox.IsChecked = site.HasWaterSupply;
            }

            if (site.OutletPresent)
            {
                powerCheckbox.IsChecked = site.OutletPresent;
            }
            if (site.PetsAllowed)
            {
                animalCheckbox.IsChecked = site.PetsAllowed;
            }
            if (site.HasShadow)
            {
                shadowCheckbox.IsChecked = site.HasShadow;
            }
            if (site.AtWater)
            {
                waterCheckbox.IsChecked = site.AtWater;
            }
            SetCheckboxBackground(craneCheckbox);
            SetCheckboxBackground(powerCheckbox);
            SetCheckboxBackground(animalCheckbox);
            SetCheckboxBackground(shadowCheckbox);
            SetCheckboxBackground(waterCheckbox);
        }
        private CheckBox FindCheckBoxByNumberAndFacility(int number, string facility)
        {
            foreach (var checkbox in YourGrid.Children.OfType<CheckBox>())
            {
                if (Convert.ToInt32(campsiteID) == number && checkbox.Name.Contains(facility))
                {
                    return checkbox;
                }
            }
            return null;
        }
        private void SetCheckboxBackground(CheckBox checkbox)
        {
            if (checkbox != null)
            {
                Border border = (Border)checkbox.Template.FindName("border", checkbox);
                if (border != null)
                {
                    border.Background = checkbox.IsChecked == true ? Brushes.Green : Brushes.Red;
                }
            }
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {   
            UpdateDatabase((CheckBox)sender, isChecked: true);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateDatabase((CheckBox)sender, isChecked: false);
        }
        private void UpdateDatabase(CheckBox checkbox, bool isChecked)
        {
            string columnName = GetColumnNameForCheckbox(checkbox.Name);
            int siteNumber = Convert.ToInt32(checkbox.Tag); 

         string connectionString = "Data Source=127.0.0.1;Initial Catalog=Camping;Persist Security Info=True;User ID=sa;Password=r2Njj8#4;Trust Server Certificate=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"UPDATE campSite SET {columnName} = @isChecked WHERE campSiteID = {campsiteID}", connection))
                {
                    command.Parameters.AddWithValue("@isChecked", isChecked ? 1 : 0);
                    command.Parameters.AddWithValue("@siteNumber", siteNumber);
                    command.ExecuteNonQuery();
                }
            }
        }
        private string GetColumnNameForCheckbox(string checkboxName)
        {
            switch (checkboxName)
            {
                case "CraneCheckbox":
                    return "waterSupply";
                case "PowerCheckbox":
                    return "powerSupply";
                case "AnimalCheckbox":
                    return "pets";
                case "ShadowCheckbox":
                    return "shadow";
                case "WaterCheckbox":
                    return "waterFront";
                default:
                    throw new ArgumentException("Unknown checkbox name", nameof(checkboxName));

            }
        }
       
        private void DisableCheckboxes()
        {
            CraneCheckbox.IsEnabled = false;
            PowerCheckbox.IsEnabled = false;
            AnimalCheckbox.IsEnabled = false;
            ShadowCheckbox.IsEnabled = false;
            WaterCheckbox.IsEnabled = false;
        }
    
        private void EnableCheckboxes()
        {
            CraneCheckbox.IsEnabled = true;
            PowerCheckbox.IsEnabled = true;
            AnimalCheckbox.IsEnabled = true;
            ShadowCheckbox.IsEnabled = true;
            WaterCheckbox.IsEnabled = true;
        }
        private void ToggleCheckboxState()
        {
            if (checkboxesEnabled)
            {
                DisableCheckboxes();
                EditFacilityButton.Content = "Aanpassen Faciliteiten";
            }
            else
            {
                EnableCheckboxes();
                EditFacilityButton.Content = "Opslaan";
            }

            checkboxesEnabled = !checkboxesEnabled;
        }
        private void EditFacilityButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleCheckboxState();

        }

    }

}