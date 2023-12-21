using Microsoft.Data.SqlClient;

namespace camping.Database
{
    public class VisitorRepository
    {

        private string connectionString = Constants.databaseConnectionString;

        public bool addVisitor(string firstName, string lastName, string preposition, string adress, string city, string postalcode, string houseNumber, int phoneNumber)
        {
            // checks if that visitor already exists in the database
            // will return -1 if it does not exist
            int visitorID = getVisitorID(firstName, lastName, preposition, adress, city, postalcode, houseNumber, phoneNumber);

            // will not add a visitor if its already in the database
            if (visitorID >= 0) return false;

            string sql = "INSERT INTO visitor (firstName, lastName, preposition, adress, city, postalcode, houseNumber, phoneNumber) VALUES (@firstName, @lastName, @preposition, @adress, @city, @postalcode, @houseNumber, @phoneNumber);";


            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

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


                    command.ExecuteNonQuery();
                }

                connection.Close();
                return true;
            }
        }

        public int getVisitorID(string firstName, string lastName, string? preposition, string adress, string city, string postalcode, string houseNumber, int phoneNumber)
        {

            string sql = "SELECT visitorID FROM visitor WHERE " +
                "firstName = @firstName AND " +
                "lastName = @lastName AND " +
                "preposition = @preposition AND " +
                "adress = @adress AND " +
                "city = @city AND " +
                "postalcode = @postalcode AND " +
                "houseNumber = @houseNumber AND " +
                "phoneNumber = @phoneNumber;";

            using (var connection = new SqlConnection(connectionString))
            {
                Console.WriteLine("Opening connection...");
                connection.Open();
                Console.WriteLine("Connection established!");

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

                    Console.WriteLine("Executing reader...");
                    SqlDataReader result = command.ExecuteReader();
                    Console.WriteLine("Reader saved in result...");

                    int visitorID = -1;
                    if (result.HasRows)
                    {
                        Console.WriteLine("Results found!");
                        while (result.Read())
                        {
                            visitorID = result.GetInt32(0);

                        }
                    }

                    connection.Close();
                    Console.WriteLine($"Result ID: {visitorID}");
                    return visitorID;

                }
            }
        }

        public bool UpdateVisitor(int visitorID, string firstName, string lastName, string preposition, string adress, string city, string postalcode, string houseNumber, int phoneNumber)
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
    }
}
