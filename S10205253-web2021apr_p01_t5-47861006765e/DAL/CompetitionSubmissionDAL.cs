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
    public class CompetitionSubmissionDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public CompetitionSubmissionDAL()
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

        public CompetitionSubmission GetDetails(int competitorID, int competitionID)
        {
            CompetitionSubmission cs = new CompetitionSubmission();
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement that
            //retrieves all attributes of a staff record.
            cmd.CommandText = @"SELECT * FROM CompetitionSubmission
                                WHERE CompetitorID = @competitorID AND CompetitionID = @competitionID;";
            //Define the parameter used in SQL statement, value for the
            //parameter is retrieved from the method parameter “staffId”.
            cmd.Parameters.AddWithValue("@competitorID", competitorID);
            cmd.Parameters.AddWithValue("@competitionID", competitionID);
            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //Read the record from database
                while (reader.Read())
                {
                    // Fill staff object with values from the data reader
                    cs.CompetitionID = competitionID;
                    cs.CompetitorID = competitorID;
                    cs.FileSubmitted = !reader.IsDBNull(2) ?
                                reader.GetString(2) : (string)null;
                    cs.DateTimeFileUpload = !reader.IsDBNull(3) ?
                                reader.GetDateTime(3) : (DateTime?)null;
                    cs.Appeal = !reader.IsDBNull(4) ?
                                reader.GetString(4) : (string)null;
                    cs.VoteCount = reader.GetInt32(5);
                    cs.Ranking = !reader.IsDBNull(6) ?
                                 reader.GetInt32(6) : (int?)null;
                }
            }
            //Close data reader
            reader.Close();
            //Close database connection
            conn.Close();
            return cs;
        }

        public List<CompetitionSubmission> GetAllSubs()
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement 
            cmd.CommandText = @"SELECT * FROM CompetitionSubmission ORDER BY CompetitorID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<CompetitionSubmission> csList = new List<CompetitionSubmission>();
            while (reader.Read())
            {
                csList.Add(
                new CompetitionSubmission
                {
                    /*
                    CompetitionID = reader.GetInt32(0),
                    CompetitorID = reader.GetInt32(1),
                    FileSubmitted = reader.GetString(2),
                    DateTimeFileUpload = reader.GetDateTime(3),
                    Appeal = reader.GetString(4),
                    VoteCount = reader.GetInt32(5),
                    Ranking = reader.GetInt32(6),
                    */
                    
                    CompetitionID = reader.GetInt32(0),
                    CompetitorID = reader.GetInt32(1),
                    FileSubmitted = !reader.IsDBNull(2) ?
                                reader.GetString(2) : (string)null,
                    DateTimeFileUpload = !reader.IsDBNull(3) ?
                                reader.GetDateTime(3) : (DateTime?)null,
                    Appeal = !reader.IsDBNull(4) ?
                                reader.GetString(4) : (string)null,
                    VoteCount = reader.GetInt32(5),
                    Ranking = !reader.IsDBNull(6) ?
                                 reader.GetInt32(6) : (int?)null,
                    /*
                    IsFullTime = reader.GetBoolean(11), //11: 12th column
                                                        //7 - 8th column, assign Branch Id, 
                                                        //if null value in db, assign integer null value
                    Appeal = reader.GetString(4),
                    VoteCount = reader.GetInt32(5),
                    Ranking = reader.GetInt32(2),


                    BranchNo = !reader.IsDBNull(7) ?
                                reader.GetInt32(7) : (int?)null,*/
                }
                                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return csList;
        }

        public bool FileSubmission(string fileName, int compID, int userID)
        {

            SqlCommand cmd = conn.CreateCommand();
            SqlCommand cmd2 = conn.CreateCommand();
            cmd.CommandText = @"UPDATE CompetitionSubmission
                                SET FileSubmitted = @fileName, DateTimeFileUpload = GETDATE()
                                WHERE CompetitionID = @compID AND CompetitorID = @userID;";

            cmd.Parameters.AddWithValue("@fileName", fileName);
            cmd.Parameters.AddWithValue("@compID", compID);
            cmd.Parameters.AddWithValue("@userID", userID);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            cmd2.CommandText = @"SELECT * FROM CompetitionSubmission
                                WHERE CompetitionID = @compID AND CompetitorID = @userID;";

            cmd2.Parameters.AddWithValue("@compID", compID);
            cmd2.Parameters.AddWithValue("@userID", userID);

            bool fileIn = true;

            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd2.ExecuteReader();
            while (reader.Read())
            {
                if (reader.GetString(2) == null)
                {
                    fileIn =  false;
                }
            }

            reader.Close();
            //Close the database connection
            conn.Close();

            return fileIn;

        }

        public bool AppealMessage(string appealDesc, int compID, int userID)
        {

            SqlCommand cmd = conn.CreateCommand();
            SqlCommand cmd2 = conn.CreateCommand();
            cmd.CommandText = @"UPDATE CompetitionSubmission
                                SET Appeal = @appeal
                                WHERE CompetitionID = @compID AND CompetitorID = @userID;";

            cmd.Parameters.AddWithValue("@appeal", appealDesc);
            cmd.Parameters.AddWithValue("@compID", compID);
            cmd.Parameters.AddWithValue("@userID", userID);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            return true;
        }

        public int JoinCompeition(CompetitionSubmission cs)
        {
            SqlCommand cmd = conn.CreateCommand();

            //Specify an INSERT SQL statement which will
            //return the auto-generated StaffID after insertion
            cmd.CommandText = @"INSERT INTO dbo.CompetitionSubmission (CompetitionID, CompetitorID, FileSubmitted, DateTimeFileUpload, Appeal, VoteCount, Ranking) 
VALUES(@CompetitionID, @CompetitorID, @FileSubmitted, @DateTimeFileUpload, @Appeal, @VoteCount, @Ranking)";

            cmd.Parameters.AddWithValue("@CompetitionID", cs.CompetitionID);
            cmd.Parameters.AddWithValue("@CompetitorID", cs.CompetitorID);
            cmd.Parameters.AddWithValue("@FileSubmitted",  DBNull.Value);
            cmd.Parameters.AddWithValue("@DateTimeFileUpload", DBNull.Value);
            cmd.Parameters.AddWithValue("@Appeal", DBNull.Value);
            cmd.Parameters.AddWithValue("@VoteCount", cs.VoteCount);
            cmd.Parameters.AddWithValue("@Ranking", DBNull.Value);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            return cs.CompetitorID;
        }

        public List<CompetitionSubmission> GetAllSubmissionsByComp(int competitionID)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement 
            cmd.CommandText = @"SELECT * FROM CompetitionSubmission WHERE CompetitionID = @competitionID ORDER BY CompetitorID";
            //Open a database connection
            cmd.Parameters.AddWithValue("@competitionID", competitionID);
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a submissions list
            List<CompetitionSubmission> competitionSubmissionsList = new List<CompetitionSubmission>();
            while (reader.Read())
            {
                competitionSubmissionsList.Add(
                new CompetitionSubmission
                {
                    CompetitionID = reader.GetInt32(0),
                    CompetitorID = reader.GetInt32(1),
                    FileSubmitted = !reader.IsDBNull(2) ?
                                reader.GetString(2) : (string)null,
                    DateTimeFileUpload = !reader.IsDBNull(3) ?
                                reader.GetDateTime(3) : (DateTime?)null,
                    Appeal = !reader.IsDBNull(4) ?
                                reader.GetString(4) : (string)null,
                    VoteCount = reader.GetInt32(5),
                    Ranking = !reader.IsDBNull(6) ?
                                 reader.GetInt32(6) : (int?)null,
                }
                                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return competitionSubmissionsList;
        }

        public List<CompetitionSubmission> UpdateSubmissionRanking(List<CompetitionSubmission> submissionList,List<int> rankingList)
        {
            for (int i = 0; i < submissionList.Count(); i++)
            {
                //Create a SqlCommand object from connection object
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = @"UPDATE CompetitionSubmission SET Ranking = @ranking WHERE CompetitionID = @competitionID AND 
                                CompetitorID = @competitorID";
                //Define the parameters used in SQL statement, value for each parameter
                //is retrieved from respective class's property.
                cmd.Parameters.AddWithValue("@ranking", rankingList[i]);
                cmd.Parameters.AddWithValue("@competitionID", submissionList[i].CompetitionID);
                cmd.Parameters.AddWithValue("@competitorID", submissionList[i].CompetitorID);
                //A connection to database must be opened before any operations made.
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                //A connection should be closed after operations.
                conn.Close();
            }
            return submissionList;
        }
    }
}
