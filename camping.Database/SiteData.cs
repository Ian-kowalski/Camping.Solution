using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft;
using Microsoft.Data.SqlClient;

namespace camping.Database
{
    public class SiteData
    {
        private string connectionString = "Data Source=127.0.0.1;Initial Catalog=Camping;Persist Security Info=True;User ID=sa;Password=r2Njj8#4;Trust Server Certificate=True";
    

        public List<string> GetSites(string sitedetail)
        {
            string sql = $"SELECT {sitedetail} FROM campSite";

            using(var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlDataReader reader;
                List<string> result = new List<string>();

                using(var command = new SqlCommand(sql, connection))
                {
                    reader = command.ExecuteReader();

                    while(reader.Read())
                    {
                        result.Add(Convert.ToString(reader.GetInt32(0)));
                    }
                }
                connection.Close();
                return result;
            }
        }
    }
}
