using camping.Core;
using camping.Database;
using DevExpress.DirectX.Common;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace camping.WPF
{
    /// <summary>
    /// Interaction logic for map.xaml
    /// </summary>
    public partial class map : UserControl
    {

        private RetrieveData _retrieveData;
        public RetrieveData retrieveData { get; set; }

        public map()
        {

        }

        public map(RetrieveData retrieveData)
        {

            _retrieveData = retrieveData;
            InitializeComponent();
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

            if (retrieveData != null)
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
            campingmap.Children.Add(button);
        }

        public void drawMap()
        {

            if (retrieveData != null)
            {
                List<Street> streets = retrieveData.Streets;
                List<Site> sites = retrieveData.Sites;

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

        private Double drawStreet(Street street, Brush brush)
        {

            Line line = new Line();
            line.X1 = street.CoordinatesPairs._x1;
            line.Y1 = street.CoordinatesPairs._y1;
            line.X2 = street.CoordinatesPairs._x2;
            line.Y2 = street.CoordinatesPairs._y2;
            line.StrokeThickness = 4;
            line.Stroke = brush;
            campingmap.Children.Add(line);
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
