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
    public class CriteriaDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public CriteriaDAL()
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

        public List<Criteria> GetAllCritera()
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement 
            cmd.CommandText = @"SELECT * FROM Criteria ORDER BY CriteriaID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Criteria> criteriaList = new List<Criteria>();
            while (reader.Read())
            {
                criteriaList.Add(
                new Criteria
                {
                    CriteriaID = reader.GetInt32(0),
                    CompetitionID = reader.GetInt32(1),
                    CriteriaName = reader.GetString(2),
                    Weightage = reader.GetInt32(3),
                }
                                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return criteriaList;
        }

        public int Add(Criteria criteria)
        {
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will return the auto-generated criteriaID after insertion
            cmd.CommandText = @"INSERT INTO dbo.Criteria (CompetitionID, CriteriaName, Weightage) 
                OUTPUT INSERTED.CriteriaID
                VALUES(@compID, @critName, @weightage)";

            cmd.Parameters.AddWithValue("@compID", criteria.CompetitionID);
            cmd.Parameters.AddWithValue("@critName", criteria.CriteriaName);
            cmd.Parameters.AddWithValue("@weightage", criteria.Weightage);
            conn.Open();
            criteria.CriteriaID = (int)cmd.ExecuteScalar();
            conn.Close();

            return criteria.CriteriaID;
        }

        public List<Criteria> GetCriteria(int competitionID)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SQL statement that select all branches
            cmd.CommandText = @"SELECT * FROM Criteria WHERE CompetitionID = @selectedComp ORDER BY CriteriaID";
            //Define the parameter used in SQL statement, value for the
            cmd.Parameters.AddWithValue("@selectedComp", competitionID);

            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            List<Criteria> criteriaList = new List<Criteria>();
            while (reader.Read())
            {
                criteriaList.Add(
                new Criteria
                {
                    CriteriaID = reader.GetInt32(0),
                    CompetitionID = reader.GetInt32(1),
                    CriteriaName = reader.GetString(2),
                    Weightage = reader.GetInt32(3),
                });
            }
            //Close DataReader
            reader.Close();
            //Close database connection
            conn.Close();
            return criteriaList;
        }

        public bool IsValid(int addedWeightage, int compID)
        {
            bool weightageMax = false;

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Criteria WHERE CompetitionID = @compID";
            cmd.Parameters.AddWithValue("@compID", compID);
            //Open a database connection and execute the SQL statement
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            int weightage = 0;
            List<Criteria> criteriaList = new List<Criteria>();
            while (reader.Read())
            {
                criteriaList.Add(
                new Criteria
                {
                    CriteriaID = reader.GetInt32(0),
                    CompetitionID = reader.GetInt32(1),
                    CriteriaName = reader.GetString(2),
                    Weightage = reader.GetInt32(3),
                });
            }
            foreach (Criteria criteria in criteriaList)
            {
                weightage += criteria.Weightage;
            }
            if (weightage + addedWeightage > 100)
            {
                weightageMax = true;
            }
            reader.Close();
            conn.Close();

            return weightageMax;
        }

        public bool IsValid(string criteriaName, int competitionID)
        {
            bool foundName = false;

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Criteria WHERE CompetitionID = @competitionID";
            cmd.Parameters.AddWithValue("@competitionID", competitionID);
            //Open a database connection and execute the SQL statement
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Criteria> criteriaList = new List<Criteria>();
            while (reader.Read())
            {
                criteriaList.Add(
                new Criteria
                {
                    CriteriaID = reader.GetInt32(0),
                    CompetitionID = reader.GetInt32(1),
                    CriteriaName = reader.GetString(2),
                    Weightage = reader.GetInt32(3),
                });
            }
            foreach (Criteria criteria in criteriaList)
            {
                if (criteria.CriteriaName.ToLower() == criteriaName.ToLower())
                {
                    foundName = true;
                }
            }
            reader.Close();
            conn.Close();

            return foundName;
        }

        public int GetTotalWeightage(int competitionID)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SQL statement that select all branches
            cmd.CommandText = @"SELECT * FROM Criteria WHERE CompetitionID = @selectedComp ORDER BY CriteriaID";
            //Define the parameter used in SQL statement, value for the
            cmd.Parameters.AddWithValue("@selectedComp", competitionID);

            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            List<Criteria> criteriaList = new List<Criteria>();
            while (reader.Read())
            {
                criteriaList.Add(
                new Criteria
                {
                    CriteriaID = reader.GetInt32(0),
                    CompetitionID = reader.GetInt32(1),
                    CriteriaName = reader.GetString(2),
                    Weightage = reader.GetInt32(3),
                });
            }
            //Close DataReader
            reader.Close();
            //Close database connection
            conn.Close();
            int totalweightage = 0;
            foreach (Criteria criteria in criteriaList)
            {
                totalweightage += criteria.Weightage;
            }
            return totalweightage;
        }

    }

}