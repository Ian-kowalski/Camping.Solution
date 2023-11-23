using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace camping.Database
{
    public class ReservationRepository
    {
        private string connectionString = "Data Source=127.0.0.1;Initial Catalog=Camping;Persist Security Info=True;User ID=sa;Password=r2Njj8#4;Trust Server Certificate=True";



        // adds a new reservation to the database
        public void addReservation(int campSiteID, string startDate, string endDate, string firstName, string preposition, string lastName, string adress, string city, string postalcode, int houseNumber, int phoneNumber) {

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

        public void addReservationLine(int campSiteID, int reservationID) {
            string sql = "INSERT INTO reservationLines (campSiteID, reservationID) VALUES (@campSiteID, @reservationID);";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("campSiteID", campSiteID);
                    command.Parameters.AddWithValue("reservationID", reservationID);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public int getReservationID(int visitorID, string startDate, string endDate) { 
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
