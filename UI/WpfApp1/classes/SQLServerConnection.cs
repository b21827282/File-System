using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Configuration;

namespace WpfApp1.classes
{
    class DbClass
    {
        public static string GetConnectionStrings()
        {
            string strConString = ConfigurationManager.ConnectionStrings["conString"].ToString();
            return strConString;
        }

        public static string sql;
        public static SqlConnection con = new SqlConnection();
        public static SqlCommand cmd = new SqlCommand("", con);
        public static SqlDataReader rd;
        public static DataTable dt;
        public static SqlDataAdapter da;

        public static void openConnection(string connectionString)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("The system failed to establish a connection." + Environment.NewLine +
                    "Descriptions: " + e.Message.ToString(), "C# wpf connect to sql server", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void closeConnection()
        {
            try
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
