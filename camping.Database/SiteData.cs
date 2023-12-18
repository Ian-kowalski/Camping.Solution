﻿using camping.Core;
using Camping.Core;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;

namespace camping.Database
{
    public class SiteData : ISiteData
    {

        private string connectionString = Constants.databaseConnectionString;

        public event PropertyChangedEventHandler? PropertyChanged;

        public List<Site> GetSiteInfo()
        {

            string sql = $"SELECT * FROM campSite ORDER BY campSiteID ASC";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlDataReader reader;
                List<Site> result = new List<Site>();

                using (var command = new SqlCommand(sql, connection))
                {
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    { /// 0 nummer, 1 power, 2 atwater, 3 pets, 4 shadow, 5 watersupply, 6 size, 7 streetID
                        result.Add(new Site(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6), reader.GetInt32(7)));
                    }
                }
                connection.Close();
                return result;
            }
        }

        public List<Street> GetStreetInfo()
        {

            string sql = $"SELECT * FROM street ORDER BY streetID ASC";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlDataReader reader;
                List<Street> result = new List<Street>();

                using (var command = new SqlCommand(sql, connection))
                {
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {                       // streetID
                        result.Add(new Street(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6)));
                    }
                }
                connection.Close();
                return result;
            }
        }

        public List<Area> GetAreaInfo()
        {

            string sql = $"SELECT * FROM area ORDER BY areaID ASC";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlDataReader reader;
                List<Area> result = new List<Area>();

                using (var command = new SqlCommand(sql, connection))
                {
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    { /// 0 nummer, 1 power, 2 atwater, 3 pets, 4 shadow, 5 watersupply, 6 size
                        result.Add(new Area(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5)));
                    }
                }
                connection.Close();
                return result;
            }
        }

        public List<int> GetCampSiteID(int reservationID)
        {
            string sql = $"SELECT campSiteID FROM reservationLines WHERE reservationID = {reservationID}";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlDataReader reader;
                List<int> result = new();

                using (var command = new SqlCommand(sql, connection))
                {
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(Convert.ToInt32(reader.GetInt32(0)));
                    }
                }
                connection.Close();
                return result;
            }
        }

        public List<ReservationDates> GetAvailability(int siteID)
        {
            List<ReservationDates> result = new();
            string sql = $"select startDate , endDate from reservation " +
                $"right join reservationLines on reservationLines.reservationID = reservation.reservationID " +
                $"WHERE campSiteID = @siteID";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataReader reader;


                using (var command = new SqlCommand(sql, connection))
                {
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        command.Parameters.AddWithValue("@siteID", siteID);

                        result.Add(new ReservationDates(reader.GetDateTime(0), reader.GetDateTime(1)));
                    }
                }
                connection.Close();
                return result;
            }
        }

        public void UpdateFacilities(Location location)
        {
            List<string> facilityNames = new List<string> { "HasWaterSupply", "OutletPresent", "PetsAllowed", "HasShadow", "AtWater" };
            string sql = "";
            if (location is Area)
            {
                Area area = location as Area;
                sql = $"UPDATE area " +
        $"SET powerSupply = @powerSupply, " +
        $"    waterFront = @waterFront, " +
        $"    pets = @pets, " +
        $"    shadow = @shadow, " +
        $"    waterSupply = @waterSupply " +
        $"WHERE areaID = {area.LocationID}";
            }
            if (location is Street)
            {
                Street street = location as Street;
                sql = $"UPDATE street " +
        $"SET powerSupply = @powerSupply, " +
        $"    waterFront = @waterFront, " +
        $"    pets = @pets, " +
        $"    shadow = @shadow, " +
        $"    waterSupply = @waterSupply " +
        $"WHERE streetID = {street.LocationID}";
            }
            if (location is Site)
            {
                Site site = location as Site;
                sql = $"UPDATE campSite " +
        $"SET powerSupply = @powerSupply, " +
        $"    waterFront = @waterFront, " +
        $"    pets = @pets, " +
        $"    shadow = @shadow, " +
        $"    waterSupply = @waterSupply " +
        $"WHERE campSiteID = {site.LocationID}";
            }

            List<int> facilities = new List<int>();
            foreach (string facilityName in facilityNames)
            {
                facilities.Add((int)location.GetType().GetProperty(facilityName).GetValue(location));
            }

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@powerSupply", facilities[1]);
                    command.Parameters.AddWithValue("@waterFront", facilities[4]);
                    command.Parameters.AddWithValue("@pets", facilities[2]);
                    command.Parameters.AddWithValue("@shadow", facilities[3]);
                    command.Parameters.AddWithValue("@waterSupply", facilities[0]);
                    command.ExecuteNonQuery();

                }
                connection.Close();
            }
        }

        public bool DeleteCampSite(int campSiteID) {

            int result;
            string sql = "DELETE " +
                "FROM campSite " +
                "WHERE campSiteID = @campSiteID;";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(sql, connection))
                {

                    command.Parameters.AddWithValue("campSiteID", campSiteID);

                    result = command.ExecuteNonQuery();
                }

                connection.Close();

                // will return true if only 1 campsite has been deleted
                return (result == 1);
            }
        }

        public Location AddLocation(Location location, int x1, int y1)
        {
            int coordinatesPairID = addCoordinatesPair(x1, y1);
            List<string> facilityNames = new List<string> { "HasWaterSupply", "OutletPresent", "PetsAllowed", "HasShadow", "AtWater" };
            string sql = "";
            if (location is Street)
            {
                sql = $"INSERT INTO campSite(powerSupply, waterFront, pets, shadow, waterSupply, size, streetID, coordinatesPairs) " +
                    "VALUES(@powerSupply, @waterFront, @pets, @shadow, @waterSupply, 4, @locationID, @coordinatesPairID);"; //size = 4
            }
            List<int> facilities = new List<int>();
            foreach (string facilityName in facilityNames)
            {
                if ((int)location.GetType().GetProperty(facilityName).GetValue(location) % 2 == 1) facilities.Add(3);
                else facilities.Add(2);
            }
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@powerSupply", facilities[1]);
                    command.Parameters.AddWithValue("@waterFront", facilities[4]);
                    command.Parameters.AddWithValue("@pets", facilities[2]);
                    command.Parameters.AddWithValue("@shadow", facilities[3]);
                    command.Parameters.AddWithValue("@waterSupply", facilities[0]);
                    command.Parameters.AddWithValue("@locationID", location.LocationID);
                    command.Parameters.AddWithValue("@coordinatesPairID", coordinatesPairID);

                    command.ExecuteNonQuery();
                }
                connection.Close();
                return location;
            }        
        }

        public int addCoordinatesPair(int x1, int y1)
        {
            int x2 = x1 + 20;
            int y2 = y1 + 20;
            int result = 0;
            string sql = "INSERT INTO coordinatesPair (x1, y1, x2, y2) " +
                "VALUES(@x1, @y1, @x2, @y2);";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataReader reader;
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@x1", x1);
                    command.Parameters.AddWithValue("@y1", y1);
                    command.Parameters.AddWithValue("@x2", x2);
                    command.Parameters.AddWithValue("@y2", y2);
                    command.ExecuteNonQuery();
                }
                sql = "SELECT MAX(coordinatesPairsKey) " +
                    "from coordinatesPair;";
                using ( var command = new SqlCommand(sql,connection))
                {
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        result = reader.GetInt32 (0);
                    }
                       
                }
                connection.Close ();
                return result;
            }
        }
    }
}