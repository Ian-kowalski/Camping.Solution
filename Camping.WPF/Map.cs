using camping.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Input;

// DIT MOET NIET
using camping.Database;
using System.IO;

namespace camping.WPF
{
    public class Map
    {

        private RetrieveData _retrieveData;
        private Grid _campingmap;
        private List<Button> siteButtons = new();
        private Button siteButton;
        private List<Line> streetLines = new();
        private Line streetLine;

        public Map(RetrieveData retrieveData, Grid campingmap)
        {
            _retrieveData = retrieveData;
            _campingmap = campingmap;
            drawMap();
            
        }


        public void drawSites(List<Site> sites, Brush areaColor, Double angle)
        {
            if (_retrieveData != null)
            {
                foreach (var site in sites)
                {
                    drawSite(areaColor, angle, site, true);
                }
            }
        }
        public void drawSites(List<Site> sites, Brush areaColor, Double angle, List<Site> availableSitesList)
        {
            if (_retrieveData != null)
            {
                //var ids = list1.Select(x => x.Id).Intersect(list2.Select(x => x.Id));
                //var result = list1.Where(x => ids.Contains(x.Id));
                foreach(var site in sites)
                {
                    if (availableSitesList.Contains(site))
                    {
                        drawSite(areaColor, angle, site, true);
                    }
                    else
                    {
                        drawSite(areaColor, angle, site, false);
                    }
                }
            }
        }

        private void drawSite(Brush areaColor, double angle, Site site, bool available)
        {
            Button button = new Button();
            
            
            button.Content = site.LocationID.ToString();
            button.Background = areaColor;
            button.Height = 30;
            button.Width = 30;
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.VerticalAlignment = VerticalAlignment.Top;
            button.Margin = new Thickness(site.CoordinatesPairs._x1, site.CoordinatesPairs._y1, 0, 0);
            button.RenderTransform = new RotateTransform { Angle = angle };

            button.BorderThickness = new Thickness(1);
            button.BorderBrush = Brushes.Black;

            button.IsEnabled = available;
            button.Tag = site;

            button.Click += (sender, e) => 
            {
                if (_campingmap.Name == "AvailableCampsitesMap") {
                    _campingmap.Visibility = Visibility.Hidden;
                    SiteSelected?.Invoke(sender, new SiteSelectedOnMapEventArgs(site));
                } else {
                    displayLocation(sender, new SiteSelectedOnMapEventArgs(site));

                    button.BorderThickness = new Thickness(4);
                    button.BorderBrush = Brushes.White;

                    ShowSelectedStreetOnMap(_retrieveData.GetStreetFromID(site.StreetID), false);
                    foreach (Button b in siteButtons)
                    {
                        if (b != button)
                        {
                            b.BorderThickness = new Thickness(1);
                            b.BorderBrush = Brushes.Black;
                        }
                    }
                }
            };

            siteButtons.Add(button);
            _campingmap.Children.Add(button);
        }

        
        public void ShowSelectedSiteOnMap(Site site)
        {
            foreach (Button button in siteButtons)
            {
                if (button.Tag == site)
                {
                    button.BorderThickness = new Thickness(2);
                    button.BorderBrush = Brushes.White;
                } else
                {
                    button.BorderThickness = new Thickness(1);
                    button.BorderBrush = Brushes.Black;
                }
            }
        }


        public void drawMap()
        {
            siteButtons.Clear();
            clearMap();

            if (_retrieveData != null)
            {

                List<Area> areas = _retrieveData.Areas;
                List<Street> streets = _retrieveData.Streets;
                List<Site> sites = _retrieveData.Sites;

                List<Site> sitesOnStreet = new List<Site>();

                foreach (Street street in streets)
                {

                    drawHighlightedStreet(street);
                }

                foreach (var street in streets)
                {

                    Brush AreaColor = (SolidColorBrush)new BrushConverter().ConvertFrom(string.Join(",", (from area in areas where area.LocationID == street.AreaID select area.AreaColor)));

                    sitesOnStreet =
                        (from site in sites
                         where site.StreetID == street.LocationID
                         select site).ToList();

                    drawSites(sitesOnStreet, AreaColor, drawStreet(street, AreaColor, true));

                }
            }
        }

        private void clearMap()
        {
            for (int i = _campingmap.Children.Count-1; i >= 0; i--)
            {
                if (typeof(Button) == _campingmap.Children[i].GetType() || typeof(Line) == _campingmap.Children[i].GetType())
                {
                    _campingmap.Children.RemoveAt(i);
                }
            }
        }

        public void drawMap(List<Site> availableCampsites, List<Street> availableStreets)
        {
            clearMap();
            if (_retrieveData != null)
            {
                List<Area> areas = _retrieveData.Areas;
                List<Street> streets = _retrieveData.Streets;
                List<Site> sites = _retrieveData.Sites;

                List<Site> availableSitesOnStreet = new List<Site>();

                foreach (Street street in streets) {
                    drawHighlightedStreet(street);
                }

                foreach (var street in streets)
                {
                    bool available = false;
                    if (availableStreets .Contains(street)) { available = true; }
                    Brush AreaColor = (SolidColorBrush)new BrushConverter().ConvertFrom(string.Join(",", (from area in areas where area.LocationID == street.AreaID select area.AreaColor)));

                    availableSitesOnStreet =
                            (from site in sites
                             where site.StreetID == street.LocationID
                             select site).ToList();
                    
                    drawSites(availableSitesOnStreet, AreaColor, drawStreet(street, AreaColor, available), availableCampsites);

                }

            }
        }

        private void drawHighlightedStreet(Street street) {

            Line highlightedLine = new Line();
            highlightedLine.X1 = street.CoordinatesPairs._x1;
            highlightedLine.Y1 = street.CoordinatesPairs._y1;
            highlightedLine.X2 = street.CoordinatesPairs._x2;
            highlightedLine.Y2 = street.CoordinatesPairs._y2;
            highlightedLine.Stroke = Brushes.White;
            highlightedLine.StrokeThickness = 8;

            _campingmap.Children.Add(highlightedLine);
        }

        private Double drawStreet(Street street, Brush brush, bool available)
        {

            Line line = new Line();
            line.X1 = street.CoordinatesPairs._x1;
            line.Y1 = street.CoordinatesPairs._y1;
            line.X2 = street.CoordinatesPairs._x2;
            line.Y2 = street.CoordinatesPairs._y2;
            line.IsEnabled = available;
            if (!available)
            {
                line.Stroke = Brushes.White;
            }
            else
            {
                line.Stroke = brush;
            }
            
            line.StrokeThickness = 8;




            line.Tag = street;

            line.MouseDown += (sender, e) =>
            {
                onStreetClick(sender, new StreetSelectedOnMapEventArgs(street));
                line.StrokeThickness = 4;

                foreach (Line l in streetLines)
                {
                    if (l != line)
                    {
                        l.StrokeThickness = 8;
                    }
                }
                foreach (Button button in siteButtons)
                {
                    Site site = button.Tag as Site;
                    if (_retrieveData.GetStreetFromID(site.StreetID).LocationID == street.LocationID)
                    {
                        button.BorderThickness = new Thickness(2);
                        button.BorderBrush = Brushes.White;
                    }
                    else
                    {
                        button.BorderThickness = new Thickness(1);
                        button.BorderBrush = Brushes.Black;
                    }
                }
            };
            streetLines.Add(line);
            
            _campingmap.Children.Add(line);
            return calculateStreetAngle(street);

        }

        

        

        public void ShowSelectedStreetOnMap(Street street, bool onlyStreet)
        {
            foreach (Line line in streetLines)
            {
                if (line.Tag == street)
                {
                    line.StrokeThickness = 4;
                } else
                {
                    line.StrokeThickness = 8;
                }
            }
            if (onlyStreet)
            {
                foreach (Button button in siteButtons)
                {
                    Site site = button.Tag as Site;
                    if (_retrieveData.GetStreetFromID(site.StreetID).LocationID == street.LocationID)
                    {
                        button.BorderThickness = new Thickness(2);
                        button.BorderBrush = Brushes.White;
                    }
                    else
                    {
                        button.BorderThickness = new Thickness(1);
                        button.BorderBrush = Brushes.Black;
                    }
                }
            }
        }

        public void ShowSelectedAreaOnMap(Area area)
        {
            foreach (Line line in streetLines)
            {
                Street street = line.Tag as Street;
                if (street.AreaID == area.LocationID)
                {
                    line.StrokeThickness = 4;
                } else
                {
                    line.StrokeThickness = 8;
                }
                
            }
            foreach (Button button in siteButtons)
            {
                Site site = button.Tag as Site;
                if (_retrieveData.GetStreetFromID(site.StreetID).AreaID == area.LocationID)
                {
                    button.BorderThickness = new Thickness(2);
                    button.BorderBrush = Brushes.White;
                }
                else
                {
                    button.BorderThickness = new Thickness(1);
                    button.BorderBrush = Brushes.Black;
                }
            }
        }

        public Double calculateStreetAngle(Street street)
        {
            return CalcAngle(street.CoordinatesPairs._x1, street.CoordinatesPairs._y1, street.CoordinatesPairs._x2, street.CoordinatesPairs._y2);
        }

        private Double CalcAngle(int x1, int y1, int x2, int y2)
        {
            Double angle = 0;
            float xDiff = x2 - x1;
            float yDiff = y2 - y1;
            angle = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;

            while (angle < -45)
            {
                angle += 90;
            }
            while (angle >= 45)
            {
                angle -= 90;
            }

            return angle;
        }

        public void ShowAvailableCampsites(List<Site> availableSites, List<Street> availablestreets) {
            drawMap(availableSites,availablestreets);
        }

        private void displayLocation(object sender, SiteSelectedOnMapEventArgs e)
        {
            SiteSelected?.Invoke(sender, e);
        }

        private void onStreetClick(object sender, StreetSelectedOnMapEventArgs e)
        {
            StreetSelected?.Invoke(sender, e);
        }

        public event EventHandler<SiteSelectedOnMapEventArgs> SiteSelected;
        public event EventHandler<StreetSelectedOnMapEventArgs> StreetSelected;
    }
}
