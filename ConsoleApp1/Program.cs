using System.Data.SqlClient;

try
{
    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
    builder.DataSource = "127.0.0.1";
    builder.UserID = "sa";
    builder.Password = "r2Njj8#4";
    builder.InitialCatalog = "TestDB";
    builder.TrustServerCertificate = false;

    Console.WriteLine(builder.ConnectionString);
    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
    {
        Console.WriteLine("\nQuery data example:");
        Console.WriteLine("=========================================\n");

        String sql = "SELECT * FROM Inventory";

        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine("{0} {1} {2}", reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2));
                }
            }
        }
    }
}
catch (SqlException e)
{
    Console.WriteLine(e.ToString());
}
Console.ReadLine();
