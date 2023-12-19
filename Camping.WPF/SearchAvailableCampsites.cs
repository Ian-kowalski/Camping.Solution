using camping.Core;
using camping.Database;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace camping.WPF
{
    public class SearchAvailableCampsites
    {
        public bool ShowSites { get; set; }
        public bool DatesSelected { get; set; }
        Label StartDateLabel;
        Label EndDateLabel;
        public DatePicker StartDateButton;
        public DatePicker EndDateButton;
        public CheckBox ShadowCheckBox;
        public CheckBox WaterSupplyCheckBox;
        public CheckBox AtWaterCheckBox;
        public CheckBox PetCheckBox;
        public CheckBox PowerCheckBox;
        public bool HasShadow;
        public bool HasWaterSupply;
        public bool AtWater;
        public bool PetAllowed;
        public bool HasPower;
        Grid grid;
        Grid availableSitesGrid;

        SiteData siteData;
        IReservationData resData;

        public AvailableCampsites availableCampsites;


        public SearchAvailableCampsites(Grid dateGrid, SiteData siteData, IReservationData resData, Grid availableSitesGrid)
        {
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
            StartDateLabel.Margin = new Thickness(0, 0, 0, 0);
            StartDateLabel.HorizontalAlignment = HorizontalAlignment.Right;
            StartDateLabel.VerticalAlignment = VerticalAlignment.Top;


            EndDateLabel = new Label();
            EndDateLabel.Content = "Einddatum";
            Grid.SetColumn(EndDateLabel, 3);
            Grid.SetRow(EndDateLabel, 1);
            EndDateLabel.Width = 128;
            EndDateLabel.Height = 32;
            EndDateLabel.FontSize = 18;
            StartDateLabel.Margin = new Thickness(0, 0, 0, 0);
            EndDateLabel.HorizontalAlignment = HorizontalAlignment.Right;
            EndDateLabel.VerticalAlignment = VerticalAlignment.Top;

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
            StartDateButton.Focusable = false;


            EndDateButton = new DatePicker();
            Grid.SetColumn(EndDateButton, 4);
            Grid.SetRow(EndDateButton, 1);
            EndDateButton.Width = 150;
            EndDateButton.Height = 32;
            EndDateButton.FontSize = 12;
            EndDateButton.BorderBrush = Brushes.Black;
            EndDateButton.Background = Brushes.White;
            EndDateButton.BorderThickness = new Thickness(2);
            StartDateLabel.Margin = new Thickness(0, 0, 0, 0);
            EndDateButton.HorizontalAlignment = HorizontalAlignment.Left;
            EndDateButton.VerticalAlignment = VerticalAlignment.Top;
            EndDateButton.SelectedDateChanged += EndDateButton_SelectedDateChanged;
            EndDateButton.DisplayDateStart = DateTime.Today;
            EndDateButton.Focusable = false;

            ShadowCheckBox = new CheckBox();
            Grid.SetColumn(ShadowCheckBox, 1);
            Grid.SetRow(ShadowCheckBox, 2);
            ShadowCheckBox.Margin = new Thickness(130, 5, 0, 0);
            ShadowCheckBox.Checked += ShadowCheckBox_Checked;
            ShadowCheckBox.Unchecked += ShadowCheckBox_Checked;


            WaterSupplyCheckBox = new CheckBox();
            Grid.SetColumn(WaterSupplyCheckBox, 2);
            Grid.SetRow(WaterSupplyCheckBox, 2);
            WaterSupplyCheckBox.Margin = new Thickness(35,5,0,0);
            WaterSupplyCheckBox.Checked += HasWaterSupplyCheckBox_Checked;
            WaterSupplyCheckBox.Unchecked += HasWaterSupplyCheckBox_Checked;


            AtWaterCheckBox = new CheckBox();
            Grid.SetColumn(AtWaterCheckBox, 2);
            Grid.SetRow(AtWaterCheckBox, 2);
            AtWaterCheckBox.Margin = new Thickness(98,5,0,0);
            AtWaterCheckBox.Checked += AtWaterCheckBox_Checked;
            AtWaterCheckBox.Unchecked += AtWaterCheckBox_Checked;


            PetCheckBox = new CheckBox();
            Grid.SetColumn(PetCheckBox, 3);
            Grid.SetRow(PetCheckBox, 2);
            PetCheckBox.Margin = new Thickness(3,5,0,0);
            PetCheckBox.Checked += PetCheckBox_Checked;
            PetCheckBox.Unchecked += PetCheckBox_Checked;


            PowerCheckBox = new CheckBox();
            Grid.SetColumn(PowerCheckBox, 3);
            Grid.SetRow(PowerCheckBox, 2);
            PowerCheckBox.Margin = new Thickness(65,5,0,0);
            PowerCheckBox.Checked += PowerCheckBox_Checked;
            PowerCheckBox.Unchecked += PowerCheckBox_Checked;



            grid.Children.Add(StartDateLabel);
            grid.Children.Add(EndDateLabel);
            grid.Children.Add(StartDateButton);
            grid.Children.Add(EndDateButton);
            grid.Children.Add(ShadowCheckBox);
            grid.Children.Add(WaterSupplyCheckBox);
            grid.Children.Add(AtWaterCheckBox);
            grid.Children.Add(PetCheckBox);
            grid.Children.Add(PowerCheckBox);

            availableCampsites = new AvailableCampsites(availableSitesGrid, siteData, resData);
            availableCampsites.ReserveCampsite += ShowAddReservation;
            availableCampsites.AvailableCampsitesListRetrievedEventHandler += (sender, e) =>
            {
                AvailableCampsitesListEventHandler?.Invoke(sender, new AvailableCampsitesEventArgs(e.AvailableSites));
            };

            SearchSites?.Invoke(this, new EventArgs());
            availableCampsites.ShowAvailableCampSites(StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today), EndDateButton.SelectedDate.GetValueOrDefault(StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today)), HasShadow, HasWaterSupply, AtWater, PetAllowed, HasPower);


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
            availableCampsites.ShowAvailableCampSites(StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today), EndDateButton.SelectedDate.GetValueOrDefault(StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today)), HasShadow, HasWaterSupply, AtWater, PetAllowed, HasPower);
            
        }

        private void EndDateButton_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            StartDateButton.DisplayDateEnd = EndDateButton.SelectedDate;
            
            availableCampsites.ShowAvailableCampSites(StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today), EndDateButton.SelectedDate.GetValueOrDefault(StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today)), HasShadow, HasWaterSupply, AtWater, PetAllowed, HasPower);
            
        }

        private void ShadowCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            HasShadow = !HasShadow;

            availableCampsites.ShowAvailableCampSites(StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today), EndDateButton.SelectedDate.GetValueOrDefault(StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today)), HasShadow, HasWaterSupply, AtWater, PetAllowed, HasPower);

        }
        private void HasWaterSupplyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            HasWaterSupply = !HasWaterSupply;

            availableCampsites.ShowAvailableCampSites(StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today), EndDateButton.SelectedDate.GetValueOrDefault(StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today)), HasShadow, HasWaterSupply, AtWater, PetAllowed, HasPower);

        }
        private void AtWaterCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AtWater = !AtWater;

            availableCampsites.ShowAvailableCampSites(StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today), EndDateButton.SelectedDate.GetValueOrDefault(StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today)), HasShadow, HasWaterSupply, AtWater, PetAllowed, HasPower);

        }
        private void PetCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PetAllowed = !PetAllowed;

            availableCampsites.ShowAvailableCampSites(StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today), EndDateButton.SelectedDate.GetValueOrDefault(StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today)), HasShadow, HasWaterSupply, AtWater, PetAllowed, HasPower);

        }
        private void PowerCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            HasPower = !HasPower;

            availableCampsites.ShowAvailableCampSites(StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today), EndDateButton.SelectedDate.GetValueOrDefault(StartDateButton.SelectedDate.GetValueOrDefault(DateTime.Today)), HasShadow, HasWaterSupply, AtWater, PetAllowed, HasPower);

        }


        public void ShowAddReservation(object sender, AddReservationEventArgs e)
        {
            AddReservation?.Invoke(this, e);
        }

        public event EventHandler<EventArgs> SearchSites;
        public event EventHandler<AddReservationEventArgs> AddReservation;
        public event EventHandler<AvailableCampsitesEventArgs> AvailableCampsitesListEventHandler;
    }
}
