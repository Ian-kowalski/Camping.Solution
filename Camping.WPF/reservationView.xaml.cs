using camping.Core;
using camping.Database;
using camping.WPF;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Camping.WPF
{
    /// <summary>
    /// Interaction logic for reservationView.xaml
    /// </summary>
    public partial class reservationView : Window
    {
        private ReservationData rData;
        private SiteData sData;
        RetrieveData retrieveData;
        FilterDialog filterDialog;
        ChangeReservation changeReservationDialog;
        List<int> toBeCancel = new List<int>();

        DateTime date = DateTime.Now;
        public reservationView()
        {
            InitializeComponent();
            rData = new ReservationData();
            sData = new SiteData();
            retrieveData = new(sData,rData);
            InitializeGrid();

        }


        private void InitializeGrid()
        {
            Grid grid = new Grid();
            for (int i = 0; i < 8; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                if (i != 0)
                {
                    col.Width = new GridLength(2, GridUnitType.Star);
                }
                grid.ColumnDefinitions.Add(col);
            }
            List<Reservation> reservations = retrieveData.GetReservations(date);
            for (int i = 0; i < reservations.Count; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(50);
                grid.RowDefinitions.Add(row);

                AddCheckbox(grid, reservations, i);

                AddID(grid, reservations, i);
                AddSiteID(grid, reservations, i);
                AddLastName(grid, reservations, i);
                AddStartDate(grid, reservations, i);
                AddEndDate(grid, reservations, i);
                AddPhoneNr(grid, reservations, i);

                AddButton(grid, reservations, i);
            }
            grid.ShowGridLines = true;
            Viewer.Content = grid;
        }
        private void AddCheckbox(Grid grid, List<Reservation> reservations, int i)
        {
            CheckBox CB = new CheckBox();
            CB.Checked += CB_checkt;
            CB.Name = "CB"+reservations.ElementAt(i).ReservationID.ToString();
            Grid.SetColumn(CB, 0);
            Grid.SetRow(CB, i);
            CB.HorizontalAlignment = HorizontalAlignment.Center;
            CB.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(CB);
        }

        private void AddID(Grid grid, List<Reservation> reservations, int i)
        {
            TextBlock TB = new TextBlock();
            TB.Text = reservations.ElementAt(i).ReservationID.ToString();
            Grid.SetColumn(TB, 1);
            Grid.SetRow(TB, i);
            TB.HorizontalAlignment = HorizontalAlignment.Center;
            TB.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(TB);
        }

        private void AddSiteID(Grid grid, List<Reservation> reservations, int i)
        {
            TextBlock TB = new TextBlock();
            TB.Text = reservations.ElementAt(i).SiteID.ToString();
            Grid.SetColumn(TB, 2);
            Grid.SetRow(TB, i);
            TB.HorizontalAlignment = HorizontalAlignment.Center;
            TB.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(TB);
        }

        private void AddLastName(Grid grid, List<Reservation> reservations, int i)
        {
            TextBlock ID = new TextBlock();
            ID.Text = reservations.ElementAt(i).Guest.LastName.ToString();
            Grid.SetColumn(ID, 3);
            Grid.SetRow(ID, i);
            ID.HorizontalAlignment = HorizontalAlignment.Center;
            ID.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(ID);
        }

        private void AddStartDate(Grid grid, List<Reservation> reservations, int i)
        {
            TextBlock ID = new TextBlock();
            ID.Text = reservations.ElementAt(i).StartDate.ToShortDateString();
            Grid.SetColumn(ID, 4);
            Grid.SetRow(ID, i);
            ID.HorizontalAlignment = HorizontalAlignment.Center;
            ID.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(ID);
        }

        private void AddEndDate(Grid grid, List<Reservation> reservations, int i)
        {
            TextBlock ID = new TextBlock();
            ID.Text = reservations.ElementAt(i).EndDate.ToShortDateString();
            Grid.SetColumn(ID, 5);
            Grid.SetRow(ID, i);
            ID.HorizontalAlignment = HorizontalAlignment.Center;
            ID.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(ID);
        }

        private void AddPhoneNr(Grid grid, List<Reservation> reservations, int i)
        {
            TextBlock ID = new TextBlock();
            ID.Text = reservations.ElementAt(i).Guest.PhoneNumber.ToString();
            Grid.SetColumn(ID, 6);
            Grid.SetRow(ID, i);
            ID.HorizontalAlignment = HorizontalAlignment.Center;
            ID.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(ID);
        }

        private void AddButton(Grid grid, List<Reservation> reservations, int i)
        {
            Button B = new Button();
            B.Click += bewerkenButtonClick;
            B.Name = "BewerkButton" + reservations.ElementAt(i).ReservationID.ToString();
            B.Content = "bewerken";
            Grid.SetColumn(B, 7);
            Grid.SetRow(B, i);
            B.HorizontalAlignment = HorizontalAlignment.Center;
            B.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(B);
        }


        private void FilterClick(object sender, RoutedEventArgs e)
        {
            filterDialog = new FilterDialog();
            filterDialog.ShowDialog();
            date = filterDialog.Date;
            InitializeGrid();
        }

        private void bewerkenButtonClick(object sender, RoutedEventArgs e)
        {
            Button c = sender as Button;
            char last_char = c.Name[c.Name.Length - 1];
            changeReservationDialog = new ChangeReservation(last_char-'0');
            changeReservationDialog.ShowDialog();
            
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            string combinedString = string.Join(", ", toBeCancel);
            string messageBoxText = "Do you want to cansel these resevations: "+ combinedString;
            string caption = "Annuleren reservering(en)";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;

            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    // User pressed Yes button
                    foreach (var reservationNr in toBeCancel)
                    {
                        // ...Delete out of database
                    }
                    toBeCancel.Clear();
                    break;
                case MessageBoxResult.No:
                    // User pressed No button
                    // ...Nothing
                    toBeCancel.Clear();
                    break;
            }
            InitializeGrid();
        }

        private void CB_checkt(object sender, RoutedEventArgs e)
        {
            CheckBox c = sender as CheckBox;
            char last_char = c.Name[c.Name.Length - 1];
            toBeCancel.Add( last_char - '0');
        }
    }
}
