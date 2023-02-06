using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
using Web_Asg.Models;

namespace Web_Asg.DAL
{
    public class CompetitorDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public CompetitorDAL()
        {
            //Read ConnectionString from appsettings.json file
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString(
            "NPBookConnectionString");
            //Instantiate a SqlConnection object with the
            //Connection String read.
            conn = new SqlConnection(strConn);
        }

        public List<Competitor> GetAllComps()
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement 
            cmd.CommandText = @"SELECT * FROM Competitor ORDER BY CompetitorID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Competitor> competitorList = new List<Competitor>();
            while (reader.Read())
            {
                competitorList.Add(
                new Competitor
                {
                    CompetitorID = reader.GetInt32(0),
                    CompetitorName = reader.GetString(1),
                    Salutation = reader.GetString(2),
                    EmailAddr = reader.GetString(3),
                    Password = reader.GetString(4),
                }
                                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return competitorList;
        }

        public int Create(Competitor x)
        {
            SqlCommand cmd = conn.CreateCommand();

            //Specify an INSERT SQL statement which will
            //return the auto-generated StaffID after insertion
            cmd.CommandText = @"INSERT INTO dbo.Competitor (CompetitorName, Salutation,
				 EmailAddr, Password) 
                OUTPUT INSERTED.CompetitorID
                VALUES(@name, @sal, @emailaddr, @pass)";

            cmd.Parameters.AddWithValue("@name", x.CompetitorName);
            cmd.Parameters.AddWithValue("@sal", x.Salutation);
            cmd.Parameters.AddWithValue("@emailaddr", x.EmailAddr);
            cmd.Parameters.AddWithValue("@pass", x.Password);

            conn.Open();
            x.CompetitorID = (int)cmd.ExecuteScalar();
            conn.Close();

            return x.CompetitorID;
        }

        public Competitor GetDetails(int compID)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement 
            cmd.CommandText = @"SELECT * FROM Competitor WHERE CompetitorID = @compID";
            cmd.Parameters.AddWithValue("@compID", compID);
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();

            Competitor competitor = new Competitor();
            while (reader.Read())
            {
                competitor.CompetitorID = reader.GetInt32(0);
                competitor.CompetitorName = reader.GetString(1);
                competitor.Salutation = reader.GetString(2);
                competitor.EmailAddr = reader.GetString(3);
                competitor.Password = reader.GetString(4);
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return competitor;
        }
    }
}
