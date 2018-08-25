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
    public class ProjectPersonService
    {
        public static Person Add(ProjectPersonRequest model)
        {
            Person p = null;
            using(SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using(SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.PersonProject_Insert";
                    cmd.Parameters.AddWithValue("@ProjectId", model.ProjectId);
                    cmd.Parameters.AddWithValue("@Email", model.Email);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        p = new Person();
                        int ord = 0;

                        p.Id = reader.GetSafeInt32(ord++);
                        p.Name = reader.GetSafeString(ord++);
                        p.Email = reader.GetSafeString(ord++);
                    }
                }
                conn.Close();
            }
            return p;
        }
        public static void Delete(int projectId, int personId)
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.PersonProject_Delete";
                    cmd.Parameters.AddWithValue("@ProjectId", projectId);
                    cmd.Parameters.AddWithValue("@PersonId", personId);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            return;
        }
    }
}