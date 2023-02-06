using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using Web_Asg.Models;

namespace Web_Asg.DAL
{
    public class JudgeDAL
    {

        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public JudgeDAL()
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

        public List<Judge> GetAllJudges()
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement 
            cmd.CommandText = @"SELECT * FROM Judge ORDER BY JudgeID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a judge list
            List<Judge> judgeList = new List<Judge>();
            while (reader.Read())
            {
                judgeList.Add(
                new Judge
                {
                    JudgeID = reader.GetInt32(0),
                    JudgeName = reader.GetString(1),
                    Salutation = reader.GetString(2),
                    AreaInterestID = reader.GetInt32(3),
                    EmailAddr = reader.GetString(4),
                    Password = reader.GetString(5),
                }
                                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return judgeList;
        }
        public Judge GetDetails(int id)
        {
            Judge judge = new Judge();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Judge WHERE JudgeID = @judgeID";
            cmd.Parameters.AddWithValue("@judgeID", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //Read the record from database
                while (reader.Read())
                {
                    judge.JudgeID = reader.GetInt32(0);
                    judge.JudgeName = reader.GetString(1);
                    judge.Salutation = reader.GetString(2);
                    judge.AreaInterestID = reader.GetInt32(3);
                    judge.EmailAddr = reader.GetString(4);
                    judge.Password = reader.GetString(5);
                }
            }
            reader.Close();
            conn.Close();
            return judge;
        }

        public int Add(Judge judge)
        {
            SqlCommand cmd = conn.CreateCommand();
         
            //Specify an INSERT SQL statement which will return the auto-generated JudgeID after insertion
            cmd.CommandText = @"INSERT INTO dbo.Judge (JudgeName, Salutation, AreaInterestID, EmailAddr, Password) 
                OUTPUT INSERTED.JudgeID
                VALUES(@name, @sal, @aiID, @emailaddr, @pass)";

            cmd.Parameters.AddWithValue("@name", judge.JudgeName);
            cmd.Parameters.AddWithValue("@sal", judge.Salutation);
            cmd.Parameters.AddWithValue("@aiID", judge.AreaInterestID);
            cmd.Parameters.AddWithValue("@emailaddr", judge.EmailAddr);
            cmd.Parameters.AddWithValue("@pass", judge.Password);

            conn.Open();
            judge.JudgeID = (int)cmd.ExecuteScalar();
            conn.Close();

            return judge.JudgeID;
        }

        public bool IsEmailExist(string email, int judgeID)
        {
            bool emailFound = false;
            //Create a SqlCommand object and specify the SQL statement
            //to get a judge record with the email address to be validated
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT JudgeID FROM Judge
                                WHERE EmailAddr=@selectedEmail";
            cmd.Parameters.AddWithValue("@selectedEmail", email);
            //Open a database connection and execute the SQL statement
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            { //Records found
                while (reader.Read())
                {
                    if (reader.GetInt32(0) != judgeID)
                        //The email address is used by another judge
                        emailFound = true;
                }
            }
            else
            { //No record
                emailFound = false; // The email address given does not exist
            }
            reader.Close();
            conn.Close();

            return emailFound;
        }

        public Judge GetJudge(string email)
        {
            Judge judge = new Judge();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Judge WHERE EmailAddr = @email";
            cmd.Parameters.AddWithValue("@email", email);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //Read the record from database
                while (reader.Read())
                {
                    judge.JudgeID = reader.GetInt32(0);
                    judge.JudgeName = reader.GetString(1);
                    judge.Salutation = reader.GetString(2);
                    judge.AreaInterestID = reader.GetInt32(3);
                    judge.EmailAddr = reader.GetString(4);
                    judge.Password = reader.GetString(5);
                }
            }
            reader.Close();
            conn.Close();
            return judge;
        }

    }
}
