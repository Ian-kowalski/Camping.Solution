using camping.Core;
using camping.Database;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using Renci.SshNet.Common;

namespace camping.WPF
{
    public class SearchAvailableCampsites
    {
        Label StartDateLabel;
        Label EndDateLabel;
        DatePicker StartDateButton;
        DatePicker EndDateButton;
        Button SearchButton;
        Grid grid;
        Grid availableSitesGrid;

        SiteData siteData;
        IReservationData resData;


        public SearchAvailableCampsites(Grid dateGrid, SiteData siteData, IReservationData resData, Grid availableSitesGrid) {
            grid = dateGrid;
            this.siteData = siteData;
            this.resData = resData;
            this.availableSitesGrid = availableSitesGrid;
            grid.Background = Brushes.LightGray;

            StartDateLabel = new Label();
            StartDateLabel.Content = "Begindatum";
            Grid.SetColumn(StartDateLabel, 1);
            Grid.SetRow(StartDateLabel, 1);
            StartDateLabel.Width = 128;
            StartDateLabel.Height = 32;
            StartDateLabel.FontSize = 18;
            StartDateLabel.Margin = new Thickness(0,0,0,0);
            StartDateLabel.HorizontalAlignment = HorizontalAlignment.Right;
            StartDateLabel.VerticalAlignment = VerticalAlignment.Top;
            
            
            EndDateLabel = new Label();
            EndDateLabel.Content = "Einddatum";
            Grid.SetColumn(EndDateLabel, 1);
            Grid.SetRow(EndDateLabel, 1);
            EndDateLabel.Width = 128;
            EndDateLabel.Height = 32;
            EndDateLabel.FontSize = 18;
            StartDateLabel.Margin = new Thickness(0, 0, 0, 0);
            EndDateLabel.HorizontalAlignment = HorizontalAlignment.Right;
            EndDateLabel.VerticalAlignment = VerticalAlignment.Bottom;

            StartDateButton = new DatePicker();
            Grid.SetColumn(StartDateButton, 2);
            Grid.SetRow(StartDateButton, 1);
            StartDateButton.Width = 150;
            StartDateButton.Height = 32;
            StartDateButton.FontSize = 12;
            StartDateButton.BorderBrush = Brushes.Black;
            StartDateButton.Background = Brushes.White;
            StartDateButton.BorderThickness = new Thickness(2);
            StartDateLabel.Margin = new Thickness(0, 0, 0, 0);
            StartDateButton.HorizontalAlignment = HorizontalAlignment.Left;
            StartDateButton.VerticalAlignment = VerticalAlignment.Top;
            StartDateButton.SelectedDateChanged += StartDateButton_SelectedDateChanged;
            StartDateButton.DisplayDateStart = DateTime.Today;

            EndDateButton = new DatePicker();
            Grid.SetColumn(EndDateButton, 2);
            Grid.SetRow(EndDateButton, 1);
            EndDateButton.Width = 150;
            EndDateButton.Height = 32;
            EndDateButton.FontSize = 12;
            EndDateButton.BorderBrush = Brushes.Black;
            EndDateButton.Background = Brushes.White;
            EndDateButton.BorderThickness = new Thickness(2);
            StartDateLabel.Margin = new Thickness(0, 0, 0, 0);
            EndDateButton.HorizontalAlignment = HorizontalAlignment.Left;
            EndDateButton.VerticalAlignment = VerticalAlignment.Bottom;
            EndDateButton.SelectedDateChanged += EndDateButton_SelectedDateChanged;
            EndDateButton.DisplayDateStart = DateTime.Today;

            SearchButton = new Button();
            SearchButton.Content = "Zoeken";
            SearchButton.FontSize = 22;
            SearchButton.BorderBrush = Brushes.Black;
            SearchButton.BorderThickness = new Thickness(2);
            SearchButton.Background = new SolidColorBrush(Color.FromRgb(153, 255, 153));
            SearchButton.Click += SearchButton_Click;
            Grid.SetColumn(SearchButton, 4);
            Grid.SetRow(SearchButton, 1);

            grid.Children.Add(SearchButton);
            grid.Children.Add(StartDateLabel);
            grid.Children.Add(EndDateLabel);
            grid.Children.Add(StartDateButton);
            grid.Children.Add(EndDateButton);




            /*
                            < Label Content = "Begindatum" Grid.Column = "0" Grid.Row = "0" HorizontalAlignment = "Right" Width = "74" Height = "24" VerticalAlignment = "Top" Margin = "0,24,24,0" />
                            < DatePicker x: Name = "StartDateButton" Grid.Column = "1" Grid.Row = "0" HorizontalAlignment = "Left" Width = "102" Height = "24" VerticalAlignment = "Top" Margin = "24,24,0,0"  SelectedDateChanged = "StartDateButton_SelectedDateChanged" />

                            < Label Content = "Einddatum" Grid.Column = "0" Grid.Row = "0" HorizontalAlignment = "Right" Width = "67" Height = "24" VerticalAlignment = "Bottom" Margin = "0,0,24,24" />
                            < DatePicker x: Name = "EndDateButton"  Grid.Column = "1" Grid.Row = "0" Height = "24" VerticalAlignment = "Bottom" HorizontalAlignment = "Left" Width = "102" Margin = "24,0,0,24"  SelectedDateChanged = "EndDateButton_SelectedDateChanged" />

                            < Button x: Name = "SearchButton" Content = "Zoek" Grid.Column = "3" Grid.ColumnSpan = "2" FontSize = "32"  Background = "#99FF99" Margin = "24,24,24,24" Click = "SearchButton_Click" />
            */
        }

        

        private void StartDateButton_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            EndDateButton.DisplayDateStart = StartDateButton.SelectedDate;
        }

        private void EndDateButton_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            StartDateButton.DisplayDateEnd = EndDateButton.SelectedDate;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchSites?.Invoke(this, new EventArgs());
            AvailableCampsites availableCampsites = new AvailableCampsites(availableSitesGrid, siteData, resData, StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today), EndDateButton.SelectedDate.GetValueOrDefault(DateTime.Today));
        }

        public event EventHandler<EventArgs> SearchSites;
    }
}
