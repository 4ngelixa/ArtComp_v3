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
    public class AreaInterestDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public AreaInterestDAL()
        {
            //Read ConnectionString from appsettings.json file
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString("NPBookConnectionString");
            //Instantiate a SqlConnection object with the
            //Connection String read.
            conn = new SqlConnection(strConn);
        }

        public List<AreaInterest> GetAllAi()
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement 
            cmd.CommandText = @"SELECT * FROM AreaInterest ORDER BY AreaInterestID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<AreaInterest> areaInterestList = new List<AreaInterest>();
            while (reader.Read())
            {
                areaInterestList.Add(
                new AreaInterest
                {
                    AreaInterestID = reader.GetInt32(0),
                    Name = reader.GetString(1),
                }
                                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return areaInterestList;
        }
        public int GetCompAmt(int aID)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"select COUNT(CompetitionID)from Competition where AreaInterestID = @selectedAreaInterest";
            cmd.Parameters.AddWithValue("@selectedAreaInterest", aID);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            int compNo = 0; 
            while (reader.Read())
            {
                compNo = reader.GetInt32(0);
            }
            reader.Close();
            conn.Close();
            return compNo;
        }
        public AreaInterest GetDetails(int id)
        {
            AreaInterest areaInterest = new AreaInterest();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"select * from AreaInterest where AreaInterestID = @selectedAreaInterest";
            cmd.Parameters.AddWithValue("@selectedAreaInterest", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //Read the record from database
                while (reader.Read())
                {
                    areaInterest.AreaInterestID = reader.GetInt32(0);
                    areaInterest.Name = reader.GetString(1);
                }
            }
            reader.Close();
            conn.Close();
            return areaInterest;
        }
        public int Add(AreaInterest areaInterest)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            //return the auto-generated StaffID after insertion
            cmd.CommandText = @"INSERT INTO AreaInterest (Name)
                                OUTPUT INSERTED.AreaInterestID VALUES(@name)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@name", areaInterest.Name);
            //A connection to database must be opened before any operations made.
            conn.Open();
            //ExecuteScalar is used to retrieve the auto-generated
            //StaffID after executing the INSERT SQL statement
            areaInterest.AreaInterestID = (int)cmd.ExecuteScalar();
            //A connection should be closed after operations.
            conn.Close();
            //Return id when no error occurs.
            return areaInterest.AreaInterestID;
        }
        public int Delete(int Aid)
        {  
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM AreaInterest WHERE AreaInterestID = @selectAreaInterestID";
            cmd.Parameters.AddWithValue("@selectAreaInterestID", Aid);
            conn.Open();
            int rowAffected = 0;
            rowAffected += cmd.ExecuteNonQuery();
            conn.Close();
            return rowAffected;
        }
    }
}
