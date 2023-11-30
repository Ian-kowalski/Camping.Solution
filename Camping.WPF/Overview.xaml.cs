using camping.Core;
using camping.Database;
using Camping.WPF;
using Renci.SshNet.Common;
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
using System.Windows.Shapes;

namespace camping.WPF
{
    /// <summary>
    /// Interaction logic for Overview.xaml
    /// </summary>
    public partial class Overview : Window
    {
        private SshConnection connection { get; set; }
        private SiteData siteData { get; set; }
        private ReservationData resData { get; set; }
        private RetrieveData retrieveData { get; set; }

        public Overview()
        {
            InitializeComponent();
            connection = new SshConnection();
            siteData = new SiteData();
            resData = new ReservationData();
            retrieveData = new RetrieveData(siteData, resData);

            displaySites();

            Closing += OnWindowClosing;
        }

        private void displaySites() {

            int rowNumber = 0;
            foreach (Site site in retrieveData.Sites)
            {
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(50);
                CampSiteList.RowDefinitions.Add(rowDef);

                Button button = new Button();
                button.Content = $"Plek {site.CampSiteID}";
                button.Margin = new Thickness(128, 4, 4, 4);

                // De volledige campsite wordt meegegeven aan de button.
                // De tag kan opgevraagd worden om informatie op het rechter scherm te tonen.
                button.Tag = site;
                
                button.Click += (sender, e) => displaySiteInformation(site);


                Grid.SetRow(button, rowNumber);
                CampSiteList.Children.Add(button);
                rowNumber++;
            }
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            connection.BreakConnection();
        }

        public void displaySiteInformation (Site site)
        {
            SizeTextField.Content = site.Size;
        }

        
    }
}
