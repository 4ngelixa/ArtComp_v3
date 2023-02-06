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
    public class CompetitionJudgeDAL
    {

        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public CompetitionJudgeDAL()
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


        public List<CompetitionJudge> GetCompJudges(int compID)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement 
            cmd.CommandText = @"SELECT * FROM CompetitionJudge WHERE CompetitionID = @compID";
            cmd.Parameters.AddWithValue("@compID", compID);
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a judge list
            List<CompetitionJudge> compjudgeList = new List<CompetitionJudge>();
            while (reader.Read())
            {
                compjudgeList.Add(
                new CompetitionJudge
                {
                    CompetitionID = reader.GetInt32(0),
                    JudgeID = reader.GetInt32(1),
                });
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return compjudgeList;
        }

        public List<CompetitionJudge> GetCompID(int judgeID)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM CompetitionJudge WHERE JudgeID = @judgeID";
            cmd.Parameters.AddWithValue("@judgeID", judgeID);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<CompetitionJudge> judgeCompList = new List<CompetitionJudge>();
            if (reader.HasRows)
            {
                //Read the record from database
                while (reader.Read())
                {
                    judgeCompList.Add(
                    new CompetitionJudge
                    {
                        CompetitionID = reader.GetInt32(0),
                        JudgeID = reader.GetInt32(1),
                    });
                }
            }
            reader.Close();
            conn.Close();
            return judgeCompList;
        }

        public void AddCompJudge(CompetitionJudgeViewModel CJ)
        {
            for (int i = 0; i < CJ.JudgeID.Count; i++)
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"INSERT INTO CompetitionJudge (JudgeID, CompetitionID) VALUES (@jID, @compID)";
                cmd.Parameters.AddWithValue("@jID", CJ.JudgeID[i]);
                cmd.Parameters.AddWithValue("@compID", CJ.CompetitionID);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

        }
        public void Delete(CompetitionJudgeViewModel CJ)
        {
            for (int i = 0; i < CJ.JudgeID.Count; i++)
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"delete from CompetitionJudge where JudgeID = @jID and CompetitionID = @compID";
                cmd.Parameters.AddWithValue("@jID", CJ.JudgeID[i]);
                cmd.Parameters.AddWithValue("@compID", CJ.CompetitionID);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        public int JudgeCount(int id)
        {
            //SELECT COUNT(JudgeID) FROM CompetitionJudge WHERE CompetitionID = 1
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT COUNT(JudgeID) FROM CompetitionJudge WHERE CompetitionID = @compID";
            cmd.Parameters.AddWithValue("@compID", id);
            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            conn.Close();
            return count;
        }
    }
}
