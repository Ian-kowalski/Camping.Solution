using camping.Core;
using camping.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace camping.WPF
{
    public class LocationInformation
    {
        public Grid LocationInfoGrid { get; set; }
        private SiteData siteData { get; set; }
        private RetrieveData retrieveData { get; set; }
        private Location tempLocation { get; set; }
        private Location location { get; set; }
        public Area SelectedArea { get; set; }
        public Street SelectedStreet { get; set; }
        public Site SelectedSite { get; set; }
        private bool isUpdating { get; set; }

        public LocationInformation(Grid locationInfoGrid, SiteData siteData, RetrieveData retrieveData, Location location, Area selectedArea, Street selectedStreet, Site selectedSite)
        {
            LocationInfoGrid = locationInfoGrid;
            this.siteData = siteData;
            this.retrieveData = retrieveData;
            this.location = location;
            SelectedArea = selectedArea;
            SelectedStreet = selectedStreet;
            SelectedSite = selectedSite;

            displayInformation(location);
        }

        private void displayInformation(Location location)
        {
            if (location is Area)
            {
                Area area = location as Area;
                Area tempArea = new(area.LocationID, area.OutletPresent, area.AtWater, area.PetsAllowed, area.HasShadow, area.HasWaterSupply);
                tempLocation = tempArea;
            }
            if (location is Street)
            {
                Street street = location as Street;
                Street tempStreet = new(street.LocationID, street.AreaID, street.OutletPresent, street.AtWater, street.PetsAllowed, street.HasShadow, street.HasWaterSupply);
                tempLocation = tempStreet;
            }
            if (location is Site)
            {
                Site site = location as Site;
                Site tempSite = new(site.LocationID, site.OutletPresent, site.AtWater, site.PetsAllowed, site.HasShadow, site.HasWaterSupply, site.Size, site.StreetID);
                tempLocation = tempSite;
            }
            LocationInfoGrid.Children.Clear();

            Label pathLabel = CreateAndAddLabel(getPathText(), 24, 0, 0);
            Grid.SetColumnSpan(pathLabel, 3);
            pathLabel.FontWeight = FontWeights.Bold;
            CreateAndAddLabel("Faciliteiten", 24, 0, 1);
            CreateAndAddLabel("Overig", 24, 0, 2);

            if (location is Site)
            {
                CreateAndAddLabel($"Oppervlak: {Convert.ToString(((Site)location).Size)}", 24, 4, 0);
            }

            CreateAndAddFacility("HasWaterSupply", 50, 1, 1, location);
            CreateAndAddFacility("OutletPresent", 50, 2, 1, location);
            CreateAndAddFacility("HasShadow", 50, 1, 2, location);
            CreateAndAddFacility("AtWater", 50, 2, 2, location);
            CreateAndAddFacility("PetsAllowed", 50, 3, 2, location);

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

        private string getPathText()
        {
            if (SelectedStreet == null) return $"Gebied: {SelectedArea.LocationID.ToString()}";
            if (SelectedSite == null) return $"Gebied: {SelectedArea.LocationID.ToString()}, Straat: {SelectedStreet.LocationID.ToString()}";
            return $"Gebied: {SelectedArea.LocationID.ToString()}, Straat: {SelectedStreet.LocationID.ToString()}, Plaats: {SelectedSite.LocationID.ToString()}";
        }

        //add label to LocationInfoGrid
        private Label CreateAndAddLabel(string content, int fontSize, int column, int row)
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
            return dynamicLabel;
        }

        //Add facility (elipse) to LocationInfoGrid
        private void CreateAndAddFacility(string name, int diameter, int column, int row, Location location)
        {
            Ellipse facility = new Ellipse
            {
                Name = name,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = diameter,
                Height = diameter,

            };
            Label label = CreateAndAddLabel(getFacilityName(facility.Name), 10, column, row);
            label.VerticalAlignment = VerticalAlignment.Bottom;
            label.HorizontalAlignment = HorizontalAlignment.Center;

            Grid.SetColumn(facility, column);
            Grid.SetRow(facility, row);

            var color = GetFacilityColor(facility);

            SolidColorBrush solidColorBrush = new SolidColorBrush(color);
            facility.Fill = solidColorBrush;
            facility.MouseLeftButtonDown += facilityClick;

            LocationInfoGrid.Children.Add(facility);
        }

        private string getFacilityName(string facility)
        {
            if (facility == "HasWaterSupply") return "Kraan";
            if (facility == "OutletPresent") return "Stroom";
            if (facility == "HasShadow") return "Schaduw";
            if (facility == "AtWater") return "Aan water";
            if (facility == "PetsAllowed") return "Huisdieren";
            return "";

        }

        //triggered when clicking a facility ellipse
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

        //retrieves the color of a specific facility
        private Color GetFacilityColor(Ellipse facility)
        {
            List<string> facilityNames = new List<string> { "HasWaterSupply", "OutletPresent", "PetsAllowed", "HasShadow", "AtWater" };
            Color color = Colors.OrangeRed;

            foreach (string facilityName in facilityNames)
            {
                if (facilityName == facility.Name)
                {
                    int value = (int)tempLocation.GetType().GetProperty(facilityName).GetValue(tempLocation);
                    if (value == 0) return Colors.OrangeRed;
                    if (value == 1) return Colors.LightGreen;
                    if (value == 2) return Colors.DarkRed;
                    if (value == 3) return Colors.DarkGreen;


                }
            }
            return color;
        }

        private void ChangeFacilityColor(Ellipse facility)
        {
            List<string> facilityNames = new List<string> { "HasWaterSupply", "OutletPresent", "PetsAllowed", "HasShadow", "AtWater" };

            foreach (string facilityName in facilityNames)
            {
                if (facilityName == facility.Name)
                {
                    int currentValue = (int)tempLocation.GetType().GetProperty(facilityName).GetValue(tempLocation);
                    ToggleFacilityValue(facilityName);
                }
            }
        }

        //toggles the temporary value of a facility
        private void ToggleFacilityValue(string facilityName)
        {
            var property = tempLocation.GetType().GetProperty(facilityName);
            if (property != null)
            {
                int currentValue = (int)property.GetValue(tempLocation);
                if (currentValue == 0) property.SetValue(tempLocation, 1);
                if (currentValue == 1)
                {
                    if (tempLocation is Area) property.SetValue(tempLocation, 0);
                    else
                    {
                        Location tempSelectedLocation = tempLocation is Site ? SelectedStreet : tempLocation is Street ? SelectedArea : null;
                        if ((int)tempSelectedLocation.GetType().GetProperty(facilityName).GetValue(tempSelectedLocation) % 2 == 0)
                        {
                            property.SetValue(tempLocation, 2);
                        }
                        else property.SetValue(tempLocation, 3);
                    }
                }
                if (currentValue >= 2)
                {
                    property.SetValue(tempLocation, 0);
                }
            }
        }

        //triggered when pressing the change facilities button
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
                UpdateLocation(tempLocation);
            }
        }

        private void UpdateLocation(Location location)
        {
            siteData.UpdateFacilities(location);

            if (location is Area)
            {
                Area tempArea = (Area)location;
                Area area = retrieveData.GetAreaFromID(tempArea.LocationID);
                area.AtWater = tempArea.AtWater;
                area.HasWaterSupply = tempArea.HasWaterSupply;
                area.OutletPresent = tempArea.OutletPresent;
                area.HasShadow = tempArea.HasShadow;
                area.PetsAllowed = tempArea.PetsAllowed;
                List<Street> streets = retrieveData.Streets.Where(i => i.AreaID == area.LocationID).Select(i => i).ToList();
                foreach (Street s in streets)
                {
                    if (s.HasWaterSupply >= 2) s.HasWaterSupply = area.HasWaterSupply % 2 + 2;
                    if (s.OutletPresent >= 2) s.OutletPresent = area.OutletPresent % 2 + 2;
                    if (s.HasShadow >= 2) s.HasShadow = area.HasShadow % 2 + 2;
                    if (s.PetsAllowed >= 2) s.PetsAllowed = area.PetsAllowed % 2 + 2;
                    if (s.AtWater >= 2) s.AtWater = area.AtWater % 2 + 2;

                    UpdateLocation(s);
                }
            }
            if (location is Street)
            {
                Street tempstreet = (Street)location;
                Street street = retrieveData.GetStreetFromID(tempstreet.LocationID);
                street.AtWater = tempstreet.AtWater;
                street.HasWaterSupply = tempstreet.HasWaterSupply;
                street.OutletPresent = tempstreet.OutletPresent;
                street.HasShadow = tempstreet.HasShadow;
                street.PetsAllowed = tempstreet.PetsAllowed;
                List<Site> sites = retrieveData.Sites.Where(i => i.StreetID == street.LocationID).Select(i => i).ToList();
                foreach (Site s in sites)
                {
                    if (s.HasWaterSupply >= 2) s.HasWaterSupply = street.HasWaterSupply % 2 + 2;
                    if (s.OutletPresent >= 2) s.OutletPresent = street.OutletPresent % 2 + 2;
                    if (s.HasShadow >= 2) s.HasShadow = street.HasShadow % 2 + 2;
                    if (s.PetsAllowed >= 2) s.PetsAllowed = street.PetsAllowed % 2 + 2;
                    if (s.AtWater >= 2) s.AtWater = street.AtWater % 2 + 2;

                    UpdateLocation(s);
                }
            }
            if (location is Site)
            {
                Site tempSite = (Site)location;
                Site site = retrieveData.GetSiteFromID(tempSite.LocationID);
                site.AtWater = tempSite.AtWater;
                site.HasWaterSupply = tempSite.HasWaterSupply;
                site.OutletPresent = tempSite.OutletPresent;
                site.HasShadow = tempSite.HasShadow;
                site.PetsAllowed = tempSite.PetsAllowed;
            }
        }
    }
}
