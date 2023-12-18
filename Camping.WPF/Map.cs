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

namespace camping.WPF
{
    public class Map
    {

        private RetrieveData _retrieveData;
        private Grid _campingmap;

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
        }

        public void drawMap()
        {
            Button button = new Button();
            button.Height = 20;
            button.Width = 20;
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.VerticalAlignment = VerticalAlignment.Top;
            _campingmap.Children.Add(button);

            if (_retrieveData != null)
            {
                Debug.WriteLine("RetrieveData is niet null");
                List<Street> streets = _retrieveData.Streets;
                List<Site> sites = _retrieveData.Sites;

                foreach (var street in streets)
                {
                    Debug.WriteLine("Straat tekenen...");
                    Brush AreaColor = PickBrush(street.AreaID);
                    List<Site> sitesOnStreet =
                        (from site in sites
                         where site.StreetID == street.LocationID
                         select site).ToList();
                    drawSites(sitesOnStreet, AreaColor, drawStreet(street, AreaColor));

                }
            }
            else
            {
                Debug.WriteLine("RetrieveData in map is null");
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
    }
}
