using ForeSight.Web.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace ForeSight.Web.Services
{
    public class StoryTypeService
    {
        public static List<StoryType> SelectAll()
        {
            List<StoryType> list = null;

            using(SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using(SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.StoryTypes_SelectAll";

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        StoryType st = new StoryType();
                        int ord = 0;

                        st.Id = reader.GetSafeInt32(ord++);
                        st.Name = reader.GetSafeString(ord++);

                        if (list == null)
                        {
                            list = new List<StoryType>();
                        }

                        list.Add(st);
                    }
                }
                conn.Close();
            }
            return list;
        }
    }
}