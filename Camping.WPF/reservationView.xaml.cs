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
        RetrieveData retrieveData;
        FilterDialog filterDialog;
        ChangeReservation changeReservationDialog;
        List<int> toBeCancel = new List<int>();

        DateTime date = DateTime.Today;
        public reservationView(RetrieveData retrieveData)
        {
            InitializeComponent();
            this.retrieveData = retrieveData;
            InitializeGrid();

        }


        private void InitializeGrid()
        {
            Grid grid = new Grid();
            for (int i = 0; i < 2; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                if (i > 2)
                {
                    col.Width = new GridLength(2, GridUnitType.Star);
                }
                grid.ColumnDefinitions.Add(col);
            }
            List<Reservation> reservations = retrieveData.GetReservations(date);

            int i = 0;
            foreach (Reservation reservation in reservations) {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(50);
                grid.RowDefinitions.Add(row);

                AddCheckbox(grid, reservation, i);

                AddID(grid, reservation, i);
                AddSiteID(grid, reservation, i);
                AddLastName(grid, reservation, i);
                AddStartDate(grid, reservation, i);
                AddEndDate(grid, reservation, i);
                AddPhoneNr(grid, reservation, i);

                AddButton(grid, reservation, i);
                i++;
            }

            
            grid.ShowGridLines = true;
            Viewer.Content = grid;
        }
        private void AddCheckbox(Grid grid, Reservation reservation, int i)
        {
            CheckBox CB = new CheckBox();
            CB.Checked += CB_checkt;
            CB.Unchecked += CB_checkt;
            CB.Name = "CB" + reservation.ReservationID.ToString();
            Grid.SetColumn(CB, 0);
            Grid.SetRow(CB, i);
            CB.HorizontalAlignment = HorizontalAlignment.Center;
            CB.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(CB);
        }

        private void AddID(Grid grid, Reservation reservation, int i)
        {
            TextBlock TB = new TextBlock();
            TB.Text = reservation.ReservationID.ToString();
            Grid.SetColumn(TB, 1);
            Grid.SetRow(TB, i);
            TB.HorizontalAlignment = HorizontalAlignment.Center;
            TB.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(TB);
        }

        private void AddSiteID(Grid grid, Reservation reservation, int i)
        {
            TextBlock TB = new TextBlock();
            TB.Text = reservation.SiteID.ToString();

            Grid.SetColumn(TB, 2);
            Grid.SetRow(TB, i);
            TB.HorizontalAlignment = HorizontalAlignment.Center;
            TB.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(TB);
        }

        private void AddLastName(Grid grid, Reservation reservation, int i)
        {
            TextBlock TB = new TextBlock();
            TB.Text = reservation.Guest.LastName.ToString();
            Grid.SetColumn(TB, 3);
            Grid.SetRow(TB, i);
            TB.HorizontalAlignment = HorizontalAlignment.Center;
            TB.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(TB);
        }

        private void AddStartDate(Grid grid, Reservation reservation, int i)
        {
            TextBlock TB = new TextBlock();
            TB.Text = reservation.StartDate.ToShortDateString();
            Grid.SetColumn(TB, 4);
            Grid.SetRow(TB, i);
            TB.HorizontalAlignment = HorizontalAlignment.Center;
            TB.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(TB);
        }

        private void AddEndDate(Grid grid, Reservation reservation, int i)
        {
            TextBlock TB = new TextBlock();
            TB.Text = reservation.EndDate.ToShortDateString();
            Grid.SetColumn(TB, 5);
            Grid.SetRow(TB, i);
            TB.HorizontalAlignment = HorizontalAlignment.Center;
            TB.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(TB);
        }


        private void FilterClick(object sender, RoutedEventArgs e)
        {
            filterDialog = new FilterDialog();
            filterDialog.ShowDialog();
            date = filterDialog.Date;
            InitializeGrid();
        }

        private void bewerkenButtonClick(Reservation reservation)
        {
            changeReservationDialog = new ChangeReservation(reservation);
            changeReservationDialog.ShowDialog();
            
        }
        
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            string combinedString = string.Join(", ", toBeCancel);
            string messageBoxText = "Do you want to cancel these reservation(s): "+ combinedString;
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
                        retrieveData.DeleteReservation(reservationNr);
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
            int last_part = int.Parse(c.Name.Remove(0, 2));
            if (c.IsChecked == true)
            {
                toBeCancel.Add(last_part);
            }
            else
            {
                toBeCancel.Remove(last_part);
            }  
            if(toBeCancel.Count != 0)
            {
                CancelButton.IsEnabled = true;
            }
            else
            {
                CancelButton.IsEnabled = false;
            }
        }
    }
}
