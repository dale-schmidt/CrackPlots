using System;
using context = System.Web.HttpContext;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace ForeSight.Web.Services
{
    public class ExceptionLoggingService
    {
        public static void Insert(Exception ex)
        {
            string apiUrl = String.Empty;
            if (context.Current.Request.Url != null)
            {
                apiUrl = context.Current.Request.Url.ToString();
            }
            string viewUrl = String.Empty;
            if (context.Current.Request.UrlReferrer != null)
            {
                viewUrl = context.Current.Request.UrlReferrer.ToString();
            }
            string userId = String.Empty;
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                userId = HttpContext.Current.User.Identity.GetUserId();
            }
            string body = String.Empty;
            if (HttpContext.Current.Request.Form.Count > 0)
            {
                body = HttpContext.Current.Request.Form.ToString();
            }
            if (context.Current.Request.HttpMethod != null)
            {
                body += " (Method: " + context.Current.Request.HttpMethod + ")";
            }
            using(SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using(SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.ExceptionLog_Insert";

                    cmd.Parameters.AddWithValue("@Message", ex.Message.ToString());
                    cmd.Parameters.AddWithValue("@Type", ex.GetType().Name.ToString());
                    cmd.Parameters.AddWithValue("@ApiUrl", apiUrl);
                    cmd.Parameters.AddWithValue("@ViewUrl", viewUrl);
                    cmd.Parameters.AddWithValue("@RequestBody", body);
                    if (ex.StackTrace != null)
                    {
                        cmd.Parameters.AddWithValue("@StackTrace", ex.StackTrace.ToString());
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@StackTrace", String.Empty);
                    }
                    cmd.Parameters.AddWithValue("@AspNetUserId", userId);

                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            return;
        }
    }
}