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

        private SiteData siteData { get; set; }
        private SshConnection connection { get; set; }
        private ReservationRepository resData { get; set; }
        private RetrieveData retrieveData { get; set; }


        public map()
        {
            connection = new SshConnection();
            siteData = new SiteData();
            resData = new ReservationRepository();
            retrieveData = new RetrieveData(siteData, resData);
            InitializeComponent();
            drawStreets();


        }

        private Brush PickBrush(int i)
        {
            Brush result = Brushes.Transparent;

            Type brushesType = typeof(Brushes);

            PropertyInfo[] properties = brushesType.GetProperties();

            result = (Brush)properties[i].GetValue(null, null);

            return result;
        }

        public void drawStreets()
        {
            if (retrieveData != null)
            {
                List<Street>  streets = siteData.GetStreetInfo();

                foreach (var street in streets)
                {
                    Line line = new Line();
                    line.X1 = street.CoordinatesPairs._x1;
                    line.Y1 = street.CoordinatesPairs._y1;
                    line.X2 = street.CoordinatesPairs._x2;
                    line.Y2 = street.CoordinatesPairs._y2;
                    line.StrokeThickness = 2;
                    line.Stroke = PickBrush(street.AreaID);

                    campingmap.Children.Add(line);

                }
            }
        }
    }
}
