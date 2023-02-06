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
    public class CompetitionDAL
    {

        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public CompetitionDAL()
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

        public List<Competition> GetAllComps()
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement 
            cmd.CommandText = @"SELECT * FROM Competition ORDER BY CompetitionID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Competition> compList = new List<Competition>();
            while (reader.Read())
            {
                compList.Add(
                new Competition
                {
                    CompetitionID = reader.GetInt32(0),
                    AreaInterestID = reader.GetInt32(1),
                    CompetitionName = reader.GetString(2),
                    StartDate = reader.GetDateTime(3),
                    EndDate = reader.GetDateTime(4),
                    ResultReleasedDate = reader.GetDateTime(5),
                }
                                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return compList;
        }

        public List<Competition> GetJudgeCompDetails(List<CompetitionJudge> compJudgeList)
        {
            List<Competition> compList = new List<Competition>();
            foreach(CompetitionJudge competitionJudge in compJudgeList)
            {
                //Create a SqlCommand object from connection object
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT * FROM Competition WHERE CompetitionID = @compID";
                cmd.Parameters.AddWithValue("@compID", competitionJudge.CompetitionID);
                //Open a database connection
                conn.Open();
                //Execute the SELECT SQL through a DataReader
                SqlDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                    compList.Add(
                    new Competition
                    {
                        CompetitionID = reader.GetInt32(0),
                        AreaInterestID = reader.GetInt32(1),
                        CompetitionName = reader.GetString(2),
                        StartDate = reader.GetDateTime(3),
                        EndDate = reader.GetDateTime(4),
                        ResultReleasedDate = reader.GetDateTime(5),
                    });
                }
                //Close DataReader
                reader.Close();
                //Close the database connection
                conn.Close();
            }
            
            return compList;
        }

        public Competition GetDetails(int compID)
        {
            Competition comp = new Competition();
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement that
            //retrieves all attributes of a staff record.
            cmd.CommandText = @"SELECT * FROM Competition
                                WHERE CompetitionID = @scid";
            //Define the parameter used in SQL statement, value for the
            //parameter is retrieved from the method parameter “staffId”.
            cmd.Parameters.AddWithValue("@scid", compID);
            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //Read the record from database
                while (reader.Read())
                {
                    comp.CompetitionID = compID;
                    comp.AreaInterestID = (int)(!reader.IsDBNull(1) ? reader.GetInt32(1) : (int?)null);
                    comp.CompetitionName = !reader.IsDBNull(2) ? reader.GetString(2) : null;
                    comp.StartDate = (DateTime)(!reader.IsDBNull(3) ? reader.GetDateTime(3) : (DateTime?)null);
                    comp.EndDate = (DateTime)(!reader.IsDBNull(4) ? reader.GetDateTime(4) : (DateTime?)null);
                    comp.ResultReleasedDate = (DateTime)(!reader.IsDBNull(5) ? reader.GetDateTime(5) : (DateTime?)null);
                }
            }
            //Close data reader
            reader.Close();
            //Close database connection
            conn.Close();
            return comp;
        }
        public int AddComp(Competition Comp)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            //return the auto-generated StaffID after insertion
            cmd.CommandText = @"INSERT INTO Competition (AreaInterestID, CompetitionName, StartDate, EndDate, ResultReleasedDate)
                                OUTPUT INSERTED.CompetitionID
                                VALUES(@Aid, @compName, @startDate, @endDate, @resulDate)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@Aid", Comp.AreaInterestID);
            cmd.Parameters.AddWithValue("@compName", Comp.CompetitionName);
            cmd.Parameters.AddWithValue("@startDate", Comp.StartDate);
            cmd.Parameters.AddWithValue("@endDate", Comp.EndDate);
            cmd.Parameters.AddWithValue("@resulDate", Comp.ResultReleasedDate);
            //A connection to database must be opened before any operations made.
            conn.Open();
            //ExecuteScalar is used to retrieve the auto-generated
            //StaffID after executing the INSERT SQL statement
            Comp.CompetitionID = (int)cmd.ExecuteScalar();
            //A connection should be closed after operations.
            conn.Close();
            //Return id when no error occurs.
            return Comp.CompetitionID;
        }    
        public void Delete(int compID)
        {
            SqlCommand cmd = conn.CreateCommand();
            //delete from Competition where CompetitionID = 9
            cmd.CommandText = @"delete from Competition where CompetitionID = @compID";
            cmd.Parameters.AddWithValue("@compID", compID);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        public void Update(Competition comp)
        {
            SqlCommand cmd = conn.CreateCommand();
            //delete from Competition where CompetitionID = 9
            cmd.CommandText = @"UPDATE Competition SET CompetitionName = @compName, StartDate = @startDate, 
                                EndDate = @endDate, ResultReleasedDate = @resultRelease WHERE CompetitionID = @compID";
            cmd.Parameters.AddWithValue("@compName", comp.CompetitionName);
            cmd.Parameters.AddWithValue("@startDate", comp.StartDate);
            cmd.Parameters.AddWithValue("@endDate", comp.EndDate);
            cmd.Parameters.AddWithValue("@resultRelease", comp.ResultReleasedDate);
            cmd.Parameters.AddWithValue("@compID", comp.CompetitionID);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

    }
}
