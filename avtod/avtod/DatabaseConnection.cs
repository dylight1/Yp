using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avtod
{
    public static class DatabaseConnection
    {
       
        private static readonly string connectionString = @"Data Source=adclg1; Initial catalog=Музей; Integrated Security=True";

        
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
