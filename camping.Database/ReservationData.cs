using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace camping.Database
{
    public class ReservationData
    {
        private string connectionString = "Data Source=127.0.0.1;Initial Catalog=Camping;Persist Security Info=True;User ID=sa;Password=r2Njj8#4;Trust Server Certificate=True";

        public List<Reservation> GetReservationInfo()
        {
            string sql = "SELECT * FROM reservation LEFT JOIN visitor ON reservation.visitorID = visitor.visitorID ORDER BY reservationID ASC";


            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlDataReader reader;
                List<Reservation> result = new List<Reservation>();

                using (var command = new SqlCommand(sql, connection))
                {
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    { /// 0 reservationID, 1 startDate, visitor(2 visitorID, 5 firstName, 6 lastName, 7 preposition, 8 adress, 9 city, 10 postalcode, 11 houseNumber, 12 phoneNumber), 3 endDate
                        result.Add(new Reservation(reader.GetInt32(0), reader.GetDateTime(1), new Visitor(reader.GetInt32(2), reader.GetString(5), reader.GetString(6), string.Empty, reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetInt32(11), reader.GetInt32(12)), reader.GetDateTime(3)));
                    }
                }
                connection.Close();
                return result;
            }
        }

        public int GetCampSiteID(int reservationID)
        {
            string sql = $"SELECT campSiteID FROM reservationLines WHERE reservationID = {reservationID}";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlDataReader reader;
                int result;

                using (var command = new SqlCommand(sql, connection))
                {
                    reader = command.ExecuteReader();

                    result = reader.GetInt32(0);
                }
                connection.Close();
                return result;
            }
        }

        // adds a new reservation to the database
        public void addReservation(int campSiteID, string startDate, string endDate, string firstName, string preposition, string lastName, string adress, string city, string postalcode, int houseNumber, int phoneNumber)
        {

            VisitorRepository visitor = new();

            // adds a new visitor to the database
            visitor.addVisitor(firstName, preposition, lastName, adress, city, postalcode, houseNumber, phoneNumber);



            // gets the visitorID of the recently added visitor
            int visitorID = visitor.getVisitorID(firstName, preposition, lastName, adress, city, postalcode, houseNumber, phoneNumber);



            // adds a new reservation
            string sql = "INSERT INTO reservation (visitorID, startDate, endDate) VALUES (@visitorID, @startDate, @endDate);";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("visitorID", visitorID);
                    command.Parameters.AddWithValue("startDate", startDate);
                    command.Parameters.AddWithValue("endDate", endDate);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }

            int reservationID = getReservationID(visitorID, startDate, endDate);

            addReservationLine(campSiteID, reservationID);

        }

        public int addReservationLine(int campSiteID, int reservationID)
        {
            int linesInserted;
            string sql = "INSERT INTO reservationLines (campSiteID, reservationID) VALUES (@campSiteID, @reservationID);";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("campSiteID", campSiteID);
                    command.Parameters.AddWithValue("reservationID", reservationID);

                    linesInserted = command.ExecuteNonQuery();
                }

                connection.Close();
                return linesInserted;
            }
        }

        public int getReservationID(int visitorID, string startDate, string endDate)
        {
            string sql = "SELECT reservationID FROM reservation WHERE " +
                "visitorID = @visitorID AND " +
                "startDate = @startDate AND " +
                "endDate = @endDate;";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("visitorID", visitorID);
                    command.Parameters.AddWithValue("startDate", startDate);
                    command.Parameters.AddWithValue("endDate", endDate);

                    SqlDataReader result = command.ExecuteReader();

                    int ID = -1;
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            ID = result.GetInt32(0);
                        }
                    }

                    connection.Close();
                    return ID;

                }
            }
        }
    }
}
