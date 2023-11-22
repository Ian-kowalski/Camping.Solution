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
    

        public List<Site> GetSiteInfo()
        {
            string sql = $"SELECT * FROM campSite ORDER BY campSiteID ASC";

            using(var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlDataReader reader;
                List<Site> result = new List<Site>();

                using(var command = new SqlCommand(sql, connection))
                {
                    reader = command.ExecuteReader();

                    while(reader.Read())
                    { /// 0 nummer, 1 power, 2 atwater, 3 pets, 4 shadow, 5 watersupply, 6 size
                        result.Add(new Site(reader.GetInt32(0), Convert.ToBoolean(reader.GetInt32(1)), Convert.ToBoolean(reader.GetInt32(2)), Convert.ToBoolean(reader.GetInt32(3)), Convert.ToBoolean(reader.GetInt32(4)), Convert.ToBoolean(reader.GetInt32(5)), reader.GetInt32(6)));
                    }
                }
                connection.Close();
                return result;
            }
        }
    }
}
