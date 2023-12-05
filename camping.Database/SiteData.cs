﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft;
using Microsoft.Data.SqlClient;
using Camping.Core;
using System.Runtime.CompilerServices;
using camping.Core;
using System.Drawing;
using System.IO;

namespace camping.Database
{
    public class SiteData : ISiteData
    {
        private string connectionString = "Data Source=127.0.0.1;Initial Catalog=Camping;Persist Security Info=True;User ID=sa;Password=r2Njj8#4;Trust Server Certificate=True";

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
                $"WHERE campSiteID = {siteID}";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataReader reader;
                

                using ( var command = new SqlCommand(sql, connection))
                {
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(new ReservationDates(reader.GetDateTime(0), reader.GetDateTime(1)));
                    }
                }
            }
            return result;

        }

        public void UpdateFacilities(Location location)
        {
            string sql = "";
            List<int> facilities = new List<int>();
            if(location is Area)
            {
                Area area = location as Area;
                facilities = updateArea(area);
                sql = $"UPDATE area " +
          $"SET powerSupply = @powerSupply, " +
          $"    waterFront = @waterFront, " +
          $"    pets = @pets, " +
          $"    shadow = @shadow, " +
          $"    waterSupply = @waterSupply " +
          $"WHERE areaID = {area.AreaID}";
            }
            if (location is Street)
            {
                Street street = location as Street;
                facilities = updateStreet(street);
                sql = $"UPDATE street " +
          $"SET powerSupply = @powerSupply, " +
          $"    waterFront = @waterFront, " +
          $"    pets = @pets, " +
          $"    shadow = @shadow, " +
          $"    waterSupply = @waterSupply " +
          $"WHERE streetID = {street.StreetID}";
            }
            if (location is Site)
            {
                Site site = location as Site;
                facilities = updateSite(site);
                sql = $"UPDATE campSite " +
          $"SET powerSupply = @powerSupply, " +
          $"    waterFront = @waterFront, " +
          $"    pets = @pets, " +
          $"    shadow = @shadow, " +
          $"    waterSupply = @waterSupply " +
          $"WHERE campSiteID = {site.CampSiteID}";
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
            }
            
        }

        private List<int> updateArea(Area area)
        {
            List<string> facilityNames = new List<string> { "HasWaterSupply", "OutletPresent", "PetsAllowed", "HasShadow", "AtWater" };
            List<int> facilities = new List<int>();
            foreach(string facilityName in facilityNames)
            {
                facilities.Add((int)area.GetType().GetProperty(facilityName).GetValue(area));
            }
            return facilities;
        }

        private List<int> updateStreet(Street street)
        {
            List<string> facilityNames = new List<string> { "HasWaterSupply", "OutletPresent", "PetsAllowed", "HasShadow", "AtWater" };
            List<int> facilities = new List<int>();

            foreach(string facilityName in facilityNames)
            {
                int facility = (int)street.GetType().GetProperty($"{facilityName}").GetValue(street);
                facilities.Add((facility));
            }
            return facilities;
        }

        private List<int> updateSite(Site site)
        {
            List<string> facilityNames = new List<string> { "HasWaterSupply", "OutletPresent", "PetsAllowed", "HasShadow", "AtWater" };
            List<int> facilities = new List<int>();

            foreach (string facilityName in facilityNames)
            {
                int facility = (int)site.GetType().GetProperty($"{facilityName}").GetValue(site);
                facilities.Add((facility));
            }
            return facilities;
        }
    }
}