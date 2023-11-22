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
    }
}
