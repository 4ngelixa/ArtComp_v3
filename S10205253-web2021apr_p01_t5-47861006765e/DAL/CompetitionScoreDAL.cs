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
    public class CompetitionScoreDAL
    {

        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public CompetitionScoreDAL()
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

        public List<CompetitionScore> GetAllScore()
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement 
            cmd.CommandText = @"SELECT * FROM CompetitionScore ORDER BY CompetitorID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<CompetitionScore> scoreList = new List<CompetitionScore>();
            while (reader.Read())
            {
                scoreList.Add(
                new CompetitionScore
                {
                    CriteriaID = reader.GetInt32(0),
                    CompetitorID = reader.GetInt32(1),
                    CompetitionID = reader.GetInt32(2),
                    Score = reader.GetInt32(3),
                });
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return scoreList;
        }

        public List<CompetitionScore> GetScoresOfCompetitor(int competitorID, int competitionID)
        {
            List<CompetitionScore> scoreList = new List<CompetitionScore>();
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement 
            cmd.CommandText = @"SELECT * FROM CompetitionScore WHERE CompetitionID = @competitionID AND CompetitorID = @competitorID ORDER BY CriteriaID";
            cmd.Parameters.AddWithValue("@competitionID", competitionID);
            cmd.Parameters.AddWithValue("@competitorID", competitorID);
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                scoreList.Add(
                new CompetitionScore
                {
                    CriteriaID = reader.GetInt32(0),
                    CompetitorID = reader.GetInt32(1),
                    CompetitionID = reader.GetInt32(2),
                    Score = reader.GetInt32(3),
                });
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return scoreList;
        }

        public CompetitionScore AddCompetitionScore(CompetitionScore competitionScore)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            //return the auto-generated StaffID after insertion
            cmd.CommandText = @"INSERT INTO CompetitionScore (CriteriaID, CompetitorID, CompetitionID, Score)
                                OUTPUT INSERTED.CriteriaID
                                VALUES(@criteriaID, @competitorID, @competitionID, @score)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@criteriaID", competitionScore.CriteriaID);
            cmd.Parameters.AddWithValue("@competitorID", competitionScore.CompetitorID);
            cmd.Parameters.AddWithValue("@competitionID", competitionScore.CompetitionID);
            cmd.Parameters.AddWithValue("@score", competitionScore.Score);
            //A connection to database must be opened before any operations made.
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            //A connection should be closed after operations.
            conn.Close();
            return competitionScore;
        }

        public CompetitionScore UpdateCompetitionScore(CompetitionScore competitionScore)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"UPDATE CompetitionScore SET Score = @score WHERE CriteriaID = @criteriaID AND 
                                CompetitorID = @competitorID AND CompetitionID = @competitionID";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@criteriaID", competitionScore.CriteriaID);
            cmd.Parameters.AddWithValue("@competitorID", competitionScore.CompetitorID);
            cmd.Parameters.AddWithValue("@competitionID", competitionScore.CompetitionID);
            cmd.Parameters.AddWithValue("@score", competitionScore.Score);
            //A connection to database must be opened before any operations made.
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            //A connection should be closed after operations.
            conn.Close();
            return competitionScore;
        }

        public bool IsUnique(int competitionID, int competitorID, int criteriaID)
        {
            bool IsUnique = true;
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SQL statement that select all branches
            cmd.CommandText = @"SELECT * FROM CompetitionScore WHERE CompetitionID = @competitionID AND CompetitorID = @competitorID
                                AND CriteriaID = @criteriaID";
            //Define the parameter used in SQL statement, value for the
            cmd.Parameters.AddWithValue("@competitionID", competitionID);
            cmd.Parameters.AddWithValue("@competitorID", competitorID);
            cmd.Parameters.AddWithValue("@criteriaID", criteriaID);

            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            List<CompetitionScore> competitionScoreList = new List<CompetitionScore>();
            while (reader.Read())
            {
                competitionScoreList.Add(
                new CompetitionScore
                {
                    CriteriaID = reader.GetInt32(0),
                    CompetitionID = reader.GetInt32(1),
                    CompetitorID = reader.GetInt32(2),
                    Score = reader.GetInt32(3),
                });
            }
            //Close DataReader
            reader.Close();
            //Close database connection
            conn.Close();
            if (competitionScoreList.Count() != 0)
            {
                IsUnique = false;
            }
            return IsUnique;
        }

    }
}
