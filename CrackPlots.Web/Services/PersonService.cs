using ForeSight.Web.Domain;
using ForeSight.Web.Models.Requests;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace ForeSight.Web.Services
{
    public class PersonService
    {
        public static int Insert(PersonAddRequest model)
        {
            int id = 0;

            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Person_Insert";
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@AspNetUserId", model.AspNetUserId);
                    SqlParameter outputId = new SqlParameter("@Id", System.Data.SqlDbType.Int)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputId);

                    cmd.ExecuteNonQuery();

                    id = (int)outputId.Value;
                }
                conn.Close();
            }
            return id;
        }

        public static bool CheckIfPerson(string aspNetUserId)
        {
            bool personExists = false;

            using(SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using(SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Person_CheckIfExists";
                    cmd.Parameters.AddWithValue("@AspNetUserId", aspNetUserId);
                    SqlParameter exists = new SqlParameter("@Exists", System.Data.SqlDbType.Bit)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };
                    cmd.Parameters.Add(exists);

                    cmd.ExecuteNonQuery();

                    personExists = (bool)exists.Value;
                }
                conn.Close();
            }
            return personExists;
        }

        //public static void Update(PersonUpdateRequest model)
        //{
        //    using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = new SqlCommand())
        //        {
        //            cmd.Connection = conn;
        //            cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //            cmd.CommandText = "dbo.Person_Update";
        //            cmd.Parameters.AddWithValue("@Id", model.Id);
        //            cmd.Parameters.AddWithValue("@AspNetUserId", model.AspNetUserId);
        //            cmd.Parameters.AddWithValue("@Name", model.Name);
        //            cmd.Parameters.AddWithValue("@Email", model.Email);
        //            cmd.ExecuteNonQuery();
        //        }
        //        conn.Close();
        //    }
        //}
        //public static void Delete(int id)
        //{
        //    using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = new SqlCommand())
        //        {
        //            cmd.Connection = conn;
        //            cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //            cmd.CommandText = "dbo.Person_Delete";

        //            cmd.Parameters.AddWithValue("@Id", id);

        //            cmd.ExecuteNonQuery();

        //        }
        //        conn.Close();
        //    }
        //}
        //public static Character SelectById(int id)
        //{
        //    Character character = null;
        //    using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = new SqlCommand())
        //        {
        //            cmd.Connection = conn;
        //            cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //            cmd.CommandText = "dbo.Person_SelectById";
        //            cmd.Parameters.AddWithValue("@Id", id);

        //            SqlDataReader reader = cmd.ExecuteReader();

        //            while (reader.Read())
        //            {
        //                character = MapCharacter(reader);
        //            }
        //        }
        //        conn.Close();
        //    }
        //    return character;
        //}
    }
}