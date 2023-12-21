using camping.Core;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace camping.Database
{
    public class ReservationRepository : IReservationData
    {
        private string connectionString = Constants.databaseConnectionString;

        public event PropertyChangedEventHandler? PropertyChanged;

        public List<Reservation> GetReservationInfo()
        {
            string sql = "SELECT distinct(reservation.reservationID), startDate, endDate, visitor.visitorID, firstName, lastName, preposition, adress, city, postalcode, houseNumber, phoneNumber, campSiteID "
            + "FROM reservation "
            + "LEFT JOIN visitor ON reservation.visitorID = visitor.visitorID "
            + "LEFT JOIN reservationLines ON reservation.reservationID = reservationLines.reservationID;";
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
                        result.Add(new Reservation(reader.GetInt32(0), reader.GetDateTime(1), reader.GetDateTime(2), new Visitor(reader.GetInt32(3), reader.GetString(4), reader.GetString(5), (reader.IsDBNull(6) ? string.Empty : reader.GetString(6)), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetInt32(11)), reader.GetInt32(12)));
                    }
                }
                connection.Close();
                return result;
            }
        }



        // adds a new reservation to the database
        public bool addReservation(int campSiteID, string startDate, string endDate, string firstName, string preposition, string lastName, string adress, string city, string postalcode, string houseNumber, int phoneNumber)
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
                visitor.addVisitor(firstName, lastName, preposition, adress, city, postalcode, houseNumber, phoneNumber);


                // gets the visitorID of the recently added visitor
                int visitorID = visitor.getVisitorID(firstName, lastName, preposition, adress, city, postalcode, houseNumber, phoneNumber);

                if (visitorID == -1) { return false; }

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

        public bool GetOtherAvailableReservation(int campSite, string startDate, string endDate, int reservationID)
        {
            string sql = "SELECT COUNT(*) FROM reservation LEFT JOIN reservationLines ON reservation.reservationID = reservationLines.reservationID " +
                                "WHERE reservationLines.campSiteID = @campSite AND " +
                                "((@startDate >= startDate AND @startDate <= endDate) OR " +
                                "(@endDate >= startDate AND @endDate <= endDate) OR " +
                                "(@startDate <= startDate AND @endDate >= endDate) AND " +
                                "(startDate <= endDate)) AND (reservation.reservationID != @reservationID)";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("campSite", campSite);
                    command.Parameters.AddWithValue("startDate", startDate);
                    command.Parameters.AddWithValue("endDate", endDate);
                    command.Parameters.AddWithValue("reservationID", reservationID);


                    int result = (int)command.ExecuteScalar();

                    connection.Close();
                    return (result == 0);

                }
            }
        }

        public bool UpdateReservation(int reservationID, DateTime startDate, DateTime endDate, int campSiteID)
        {
            string sql = $"UPDATE reservation SET startDate = @startDate, endDate = @endDate WHERE reservationID = @reservationID";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                int result;
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("startDate", startDate);
                    command.Parameters.AddWithValue("endDate", endDate);
                    command.Parameters.AddWithValue("reservationID", reservationID);
                    command.Parameters.AddWithValue("campSiteID", campSiteID);

                    result = command.ExecuteNonQuery();
                }
                connection.Close();
                return (result != 0);
            }
        }

        public bool UpdateReservationLines(int campSiteID, int reservationID)
        {
            string sql = $"UPDATE reservationLines SET campSiteID = @campSiteID WHERE reservationID = @reservationID";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                int result;

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("campSiteID", campSiteID);
                    command.Parameters.AddWithValue("reservationID", reservationID);

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

        public bool HasUpcomingReservations(int campSiteID, DateTime date) {

            string sql = "SELECT COUNT(*) FROM reservation LEFT JOIN reservationLines ON reservation.reservationID = reservationLines.reservationID " +
                                "WHERE reservationLines.campSiteID = @campSite AND " +
                                "(@startDate <= startDate OR @startDate <= endDate);";
            //      if...   today is before a reservation OR today is before a reservation has ended
            //      if...         upcoming reservation    OR    ongoing reservation

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("campSite", campSiteID);
                    command.Parameters.AddWithValue("startDate", date.ToString("MM-dd-yyyy")); ;


                    int result = (int)command.ExecuteScalar();

                    connection.Close();
                    Console.WriteLine(result + " lines found!");
                    return (result > 0);

                }
            }
        }



        public bool UpdateVisitor(int visitorID, string firstName, string lastName, string preposition, string adress, string city, string postalcode, string houseNumber, int phoneNumber)
        {
            VisitorRepository visitor = new();
            return visitor.UpdateVisitor(visitorID,firstName,lastName, preposition, adress, city, postalcode, houseNumber, phoneNumber);
        }

    }
}
