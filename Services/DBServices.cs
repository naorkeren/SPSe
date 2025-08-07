using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SurfingPointServer.Models;

namespace SurfingPointServer.Services
{
    public class DBServices
    {
        static string connectionString = @"Server=.\SQLEXPRESS;Database=SurfingPoint;Trusted_Connection=True;TrustServerCertificate=True";

        public static List<Beach> GetAllBeaches()
        {
            List<Beach> beaches = new List<Beach>();

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Beaches", con))
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    beaches.Add(new Beach
                    {
                        BeachID = Convert.ToInt32(reader["BeachID"]),
                        Name = reader["Name"].ToString(),
                        Location = reader["Location"].ToString(),
                        HasCamera = Convert.ToBoolean(reader["HasCamera"])
                    });
                }
            }

            return beaches;
        }

        public static int AddUser(User user)
        {
            string sql = @"
                INSERT INTO Users (Name, Email, PasswordHash, DateJoined)
                VALUES (@Name, @Email, @PasswordHash, @DateJoined);
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@DateJoined", user.DateJoined);

                con.Open();
                int newId = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();

                return newId;
            }
        }

        public static User GetUserByEmail(string email)
        {
            string sql = "SELECT * FROM Users WHERE Email = @Email";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Email", email);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new User
                    {
                        UserID = Convert.ToInt32(reader["UserID"]),
                        Name = reader["Name"].ToString(),
                        Email = reader["Email"].ToString(),
                        PasswordHash = reader["PasswordHash"].ToString(),
                        DateJoined = Convert.ToDateTime(reader["DateJoined"])
                    };
                }
            }

            return null;
        }

        // 🔹 פונקציה 1 – שליפת פרטי חוף לפי ID
        public static BeachDetails GetBeachDetailsById(int beachId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetBeachDetails", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BeachID", beachId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new BeachDetails
                    {
                        BeachID = Convert.ToInt32(reader["BeachID"]),
                        Name = reader["Name"].ToString(),
                        Location = reader["Location"].ToString(),
                        WaveHeight = reader["WaveHeight"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["WaveHeight"]),
                        WindSpeed = reader["WindSpeed"] == DBNull.Value ? 0 : Convert.ToSingle(reader["WindSpeed"]),
                        ForecastDate = reader["ForecastDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["ForecastDate"])
                    };
                }

                con.Close();
            }

            return null;
        }


        // 🔹 פונקציה 2 – שליפת פרטים לכל החופים
        public static List<BeachDetails> GetAllBeachDetails()
        {
            List<BeachDetails> list = new List<BeachDetails>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sql = "SELECT BeachID FROM Beaches";

                SqlCommand cmd = new SqlCommand(sql, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int beachId = Convert.ToInt32(reader["BeachID"]);
                    BeachDetails details = GetBeachDetailsById(beachId);
                    if (details != null)
                        list.Add(details);
                }

                con.Close();
            }

            return list;
        }
    }
}
