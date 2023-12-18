﻿using camping.Core;
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
using camping.Database;

namespace camping.WPF
{
    public class Map
    {

        private RetrieveData _retrieveData;
        private Grid _campingmap;
        private List<Button> siteButtons = new List<Button>();

        public Map(RetrieveData retrieveData, Grid campingmap)
        {
            _retrieveData = retrieveData;
            _campingmap = campingmap;
            drawMap();
        }

        private Brush PickBrush(int i)
        {
            Brush result = Brushes.Transparent;

            Type brushesType = typeof(Brushes);

            PropertyInfo[] properties = brushesType.GetProperties();

            result = (Brush)properties[i].GetValue(null, null);

            return result;
        }

        public void drawSites(List<Site> sites, Brush areaColor, Double angle)
        {
            if (_retrieveData != null)
            {
                foreach (var site in sites)
                {
                    drawSite(areaColor, angle, site);
                }
            }
        }
        public void drawSites(List<Site> sites, Brush areaColor, Double angle, List<Site> availableSitesList)
        {
            if (_retrieveData != null)
            {
                //var ids = list1.Select(x => x.Id).Intersect(list2.Select(x => x.Id));
                //var result = list1.Where(x => ids.Contains(x.Id));


                List<Site> availableSites = new List<Site>();
                foreach (Site site in sites) { 
                    availableSites.Add(site);
                }

                List<Site> unavailableSites = new List<Site>();
                foreach (Site site in sites)
                {
                    unavailableSites.Add(site);
                }

                foreach (var site in sites.Where(s => sites.Select(s => s.LocationID).Intersect(availableSitesList.Select(s => s.LocationID)).Contains(s.LocationID)))
                {
                    drawSite(areaColor, angle, site, true);
                };

                foreach (var site in sites.Where(s => sites.Select(s => s.LocationID).Except(availableSitesList.Select(s => s.LocationID)).Contains(s.LocationID)))
                {
                    drawSite(areaColor, angle, site, false);
                };


            }
        }

        private void drawSite(Brush areaColor, double angle, Site site)
        {
            Button button = new Button();
            
            
            button.Content = site.LocationID.ToString();
            button.Background = areaColor;
            button.Height = 20;
            button.Width = 20;
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.VerticalAlignment = VerticalAlignment.Top;
            button.Margin = new Thickness(site.CoordinatesPairs._x1, site.CoordinatesPairs._y1, 0, 0);
            button.RenderTransformOrigin = new Point(0.5, 0.5);
            button.RenderTransform = new RotateTransform { Angle = angle };
            _campingmap.Children.Add(button);

            siteButtons.Add(button);

            button.Click += (sender, e) => 
            {
                displayLocation(sender, new SiteSelectedOnMapEventArgs(site));
                button.BorderThickness = new Thickness(2);
                button.BorderBrush = Brushes.Blue;
                foreach(Button b in siteButtons)
                {
                    if (b != button)
                    {
                        b.BorderThickness = new Thickness(1);
                        b.BorderBrush = Brushes.Black;
                    }
                }
            };     
        }

        private void drawSite(Brush areaColor, double angle, Site site, bool available)
        {
            Button button = new Button();

            button.Content = site.LocationID.ToString();
            button.Background = areaColor;
            button.Height = 20;
            button.Width = 20;
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.VerticalAlignment = VerticalAlignment.Top;
            button.Margin = new Thickness(site.CoordinatesPairs._x1, site.CoordinatesPairs._y1, 0, 0);
            button.RenderTransformOrigin = new Point(0.5, 0.5);
            button.RenderTransform = new RotateTransform { Angle = angle };
            button.IsEnabled = available;
            _campingmap.Children.Add(button);
        }

        public void drawMap()
        {
            if (_retrieveData != null)
            {
                List<Street> streets = _retrieveData.Streets;
                List<Site> sites = _retrieveData.Sites;
                siteButtons.Clear();
                foreach (var street in streets)
                {

                    Brush AreaColor = PickBrush(street.AreaID);
                    List<Site> sitesOnStreet =
                        (from site in sites
                         where site.StreetID == street.LocationID
                         select site).ToList();
                    drawSites(sitesOnStreet, AreaColor, drawStreet(street, AreaColor));

                }
            }
        }
        public void drawMap(List<Site> availableCampsites)
        {
            _campingmap.Children.Clear();

            if (_retrieveData != null)
            {
                List<Street> streets = _retrieveData.Streets;
                List<Site> sites = _retrieveData.Sites;

                foreach (var street in streets)
                {

                    Brush AreaColor = PickBrush(street.AreaID);
                    List<Site> availableSitesOnStreet =
                        (from site in sites
                         where site.StreetID == street.LocationID
                         select site).ToList();
                    drawSites(availableSitesOnStreet, AreaColor, drawStreet(street, AreaColor), availableCampsites);

                }
            }
        }

        private Double drawStreet(Street street, Brush brush)
        {

            Line line = new Line();
            line.X1 = street.CoordinatesPairs._x1;
            line.Y1 = street.CoordinatesPairs._y1;
            line.X2 = street.CoordinatesPairs._x2;
            line.Y2 = street.CoordinatesPairs._y2;
            line.StrokeThickness = 4;
            line.Stroke = brush;
            _campingmap.Children.Add(line);
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


        public void ShowAvailableCampsites(List<Site> availableSites) {
            drawMap(availableSites);
        }

      
        private void displayLocation(object sender, SiteSelectedOnMapEventArgs e)
        {
            SiteSelected?.Invoke(sender, e);
            
        }

        public event EventHandler<SiteSelectedOnMapEventArgs> SiteSelected;

    }
}