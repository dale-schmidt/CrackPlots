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
    public class SecurityTokenService
    {
        public static Guid Insert(SecurityTokenAddRequest model)
        {
            Guid guid = new Guid();

            using(SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using(SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.SecurityToken_Insert";

                    cmd.Parameters.AddWithValue("@AspNetUserId", model.AspNetUserId);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    SqlParameter p = new SqlParameter("@TokenGuid", System.Data.SqlDbType.UniqueIdentifier);
                    p.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(p);

                    cmd.ExecuteNonQuery();

                    guid = (Guid)p.Value;
                }
                conn.Close();
            }
            return guid;
        }
        
        public static SecurityToken SelectByGuid(Guid guid)
        {
            SecurityToken securityToken = new SecurityToken();

            using(SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using(SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.SecurityToken_SelectByGuid";

                    cmd.Parameters.AddWithValue("@TokenGuid", guid);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        int ord = 0;
                        securityToken.AspNetUserId = reader.GetSafeString(ord++);
                        securityToken.Email = reader.GetSafeString(ord++);
                        securityToken.TokenGuid = reader.GetSafeGuid(ord++);
                        securityToken.DateCreated = reader.GetSafeDateTime(ord++);
                    }
                }
            }
            return securityToken;
        }
    }
}