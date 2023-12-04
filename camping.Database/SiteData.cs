using System;
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
                        result.Add(new Site(reader.GetInt32(0), Convert.ToBoolean(reader.GetInt32(1)), Convert.ToBoolean(reader.GetInt32(2)), Convert.ToBoolean(reader.GetInt32(3)), Convert.ToBoolean(reader.GetInt32(4)), Convert.ToBoolean(reader.GetInt32(5)), reader.GetInt32(6), reader.GetInt32(7)));
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
                        result.Add(new Street(reader.GetInt32(0), reader.GetInt32(1), Convert.ToBoolean(reader.GetInt32(2)), Convert.ToBoolean(reader.GetInt32(3)), Convert.ToBoolean(reader.GetInt32(4)), Convert.ToBoolean(reader.GetInt32(5)), Convert.ToBoolean(reader.GetInt32(6))));
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
                        result.Add(new Area(reader.GetInt32(0), Convert.ToBoolean(reader.GetInt32(1)), Convert.ToBoolean(reader.GetInt32(2)), Convert.ToBoolean(reader.GetInt32(3)), Convert.ToBoolean(reader.GetInt32(4)), Convert.ToBoolean(reader.GetInt32(5))));
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
            if (location is Site)
            {
                Site site = location as Site;
                sql = $"UPDATE campSite " +
                      $"SET powerSupply = @powerSupply, " +
                      $"    waterFront = @waterFront, " +
                      $"    pets = @pets, " +
                      $"    shadow = @shadow, " +
                      $"    waterSupply = @waterSupply " +
                      $"WHERE campSiteID = {site.CampSiteID}";
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
                      $"WHERE streetID = {street.StreetID}";
            }
            if (location is Area)
            {
                Area area = location as Area;
                sql = $"UPDATE area " +
                      $"SET powerSupply = @powerSupply, " +
                      $"    waterFront = @waterFront, " +
                      $"    pets = @pets, " +
                      $"    shadow = @shadow, " +
                      $"    waterSupply = @waterSupply " +
                      $"WHERE areaID = {area.AreaID}";
            }
            using (var connection = new SqlConnection(connectionString))
            {
                int outletPresent = 0;
                if (location.OutletPresent) outletPresent = 1;
                int atWater = 0;
                if (location.AtWater) atWater = 1;
                int petsAllowed = 0;
                if (location.PetsAllowed) petsAllowed = 1;
                int hasShadow = 0;
                if (location.HasShadow) hasShadow = 1;
                int hasWaterSupply = 0;
                if (location.HasWaterSupply) hasWaterSupply = 1;

                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@powerSupply", outletPresent);
                    command.Parameters.AddWithValue("@waterFront", atWater);
                    command.Parameters.AddWithValue("@pets", petsAllowed);
                    command.Parameters.AddWithValue("@shadow", hasShadow);
                    command.Parameters.AddWithValue("@waterSupply", hasWaterSupply);                    
                    command.ExecuteNonQuery();

                }
            }
            
        }


    }
}