using camping.Core;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace camping.Database
{
    public class ReservationData : IReservationData
    {
        private string connectionString = "Data Source=127.0.0.1;Initial Catalog=Camping;Persist Security Info=True;User ID=sa;Password=r2Njj8#4;Trust Server Certificate=True";

        public event PropertyChangedEventHandler? PropertyChanged;

        public List<Reservation> GetReservationInfo()
        {
            string sql = "SELECT reservationID, startDate, endDate, visitor.visitorID, firstName, lastName, preposition, adress, city, postalcode, houseNumber, phoneNumber FROM reservation LEFT JOIN visitor ON reservation.visitorID = visitor.visitorID";


            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlDataReader reader;
                List<Reservation> result = new List<Reservation>();

                using (var command = new SqlCommand(sql, connection))
                {
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    { /// 0 reservationID, 1 startDate, 3 endDate, visitor(2 visitorID, 5 firstName, 6 lastName, 7 preposition, 8 adress, 9 city, 10 postalcode, 11 houseNumber, 12 phoneNumber)
                        result.Add(new Reservation(reader.GetInt32(0), reader.GetDateTime(1), reader.GetDateTime(2), new Visitor(reader.GetInt32(3), reader.GetString(4), reader.GetString(5), (reader.IsDBNull(6) ? string.Empty :reader.GetString(6)) , reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetInt32(10), reader.GetInt32(11))));
                    }
                }
                connection.Close();
                return result;
            }
        }

        public List<Reservation> GetReservationInfo(DateTime date)
        {
            string sql = "SELECT reservationID, startDate, endDate, visitor.visitorID, firstName, lastName, preposition, adress, city, postalcode, houseNumber, phoneNumber " +
                "FROM reservation LEFT JOIN visitor ON reservation.visitorID = visitor.visitorID " +
                "WHERE startDate > @startDate;";


            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                
                List<Reservation> result = new List<Reservation>();
                

                using (var command = new SqlCommand(sql, connection))
                {
                    string d = date.ToString("MM-dd-yyyy");
                    command.Parameters.AddWithValue("startDate", d);
                    Console.WriteLine(sql);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    { /// 0 reservationID, 1 startDate, 3 endDate, visitor(2 visitorID, 5 firstName, 6 lastName, 7 preposition, 8 adress, 9 city, 10 postalcode, 11 houseNumber, 12 phoneNumber)
                        result.Add(new Reservation(reader.GetInt32(0), reader.GetDateTime(1), reader.GetDateTime(2), new Visitor(reader.GetInt32(3), reader.GetString(4), reader.GetString(5), (reader.IsDBNull(6) ? string.Empty : reader.GetString(6)), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetInt32(10), reader.GetInt32(11))));
                    }
                }
                connection.Close();
                return result;
            }
        }

        // adds a new reservation to the database
        public bool addReservation(int campSiteID, string startDate, string endDate, string firstName, string preposition, string lastName, string adress, string city, string postalcode, int houseNumber, int phoneNumber)
        {
            if (!GetAvailableReservation(campSiteID, startDate, endDate))
            {
                Console.WriteLine("Spot is already reserved during these dates!");
                return false;
            }
            else
            {

                int linesInserted;
                VisitorRepository visitor = new();

                // adds a new visitor to the database
                // will use existing visitor if already present
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

                        linesInserted = command.ExecuteNonQuery();
                    }

                    connection.Close();
                }

                // gets the reservation ID of the recently added reservation using the requested visitorID
                int reservationID = getReservationID(visitorID, startDate, endDate);

                // adds a reservation line using the requested reservationID
                // addReservationLine(campSiteID, reservationID);

                // will return true if both the reservation and reservation line gets added
                return (linesInserted > 0 && addReservationLine(campSiteID, reservationID));
            }
        }

        public bool addReservationLine(int campSiteID, int reservationID)
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

                // will return true if the reservation line has been added
                return (linesInserted > 0);
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

        public bool GetAvailableReservation(int campSite, string startDate, string endDate)
        {
            string sql = "SELECT COUNT(*) FROM reservation LEFT JOIN reservationLines ON reservation.reservationID = reservationLines.reservationID " +
                                "WHERE reservationLines.campSiteID = @campSite AND " +
                                "((@startDate >= startDate AND @startDate <= endDate) OR " +
                                "(@endDate >= startDate AND @endDate <= endDate) OR " +
                                "(@startDate <= startDate AND @endDate >= endDate)) AND " +
                                "(startDate <= endDate);";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("campSite", campSite);
                    command.Parameters.AddWithValue("startDate", startDate);
                    command.Parameters.AddWithValue("endDate", endDate);


                    int result = (int)command.ExecuteScalar();

                    connection.Close();
                    return (result == 0);

                }
            }
        }

        public bool UpdateReservation(int reservationID, DateTime startDate, int visitorID, DateTime endDate)
        {
            string sql = $"UPDATE reservation SET startDate = @startDate, visitorID = @visitorID, endDate = @endDate WHERE reservationID = @reservationID";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                int result;
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("startDate", startDate);
                    command.Parameters.AddWithValue("visitorID", visitorID);
                    command.Parameters.AddWithValue("endDate", endDate);
                    command.Parameters.AddWithValue("reservationID", reservationID);

                    result = command.ExecuteNonQuery();
                }
                connection.Close();
                return (result != 0);
            }
        }
        
        public bool UpdateVisitor(int visitorID, string firstName, string lastName, string preposition, string adress, string city, string postalcode, int houseNumber, int phoneNumber)
        {
            string sql = $"UPDATE visitor SET firstName = @firstName, lastName = @lastName, preposition = @preposition, adress = @adress, city = @city, postalcode = @Postalcode, houseNumber = @houseNumber, phoneNumber = @phoneNumber WHERE visitorID = @visitorID";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                int result;

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("firstName", firstName);
                    command.Parameters.AddWithValue("lastName", lastName);
                    command.Parameters.AddWithValue("preposition", preposition);
                    command.Parameters.AddWithValue("adress", adress);
                    command.Parameters.AddWithValue("city", city);
                    command.Parameters.AddWithValue("postalcode", postalcode);
                    command.Parameters.AddWithValue("houseNumber", houseNumber);
                    command.Parameters.AddWithValue("phoneNumber", phoneNumber);
                    command.Parameters.AddWithValue("visitorID", visitorID);

                    result = command.ExecuteNonQuery();
                }
                connection.Close();
                return (result != 0);
            }
        }
    
        public bool DeleteReservation(int reservationID)
        {
            int result;
            string sql = "DELETE " +
                "FROM reservation " +
                "WHERE reservationID = @reservationID;";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(sql, connection))
                {

                    command.Parameters.AddWithValue("reservationID", reservationID);

                   result = command.ExecuteNonQuery();
                }

                connection.Close();

                // will return true if the reservation line has been added
                return (result > 0);
            }
        }
    }
}
