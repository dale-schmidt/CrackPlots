using ForeSight.Web.Domain;
using ForeSight.Web.Models.Requests;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace ForeSight.Web.Services
{
    public class SceneService
    {
        private static string _getId()
        {
            return HttpContext.Current.User.Identity.GetUserId();
            //return "f7987305-3b9d-4d5c-9790-b39b4373a8e1";
        }
        public static int Insert(SceneAddRequest model)
        {
            int id = 0;

            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Scene_Insert";

                    MapCommonSceneParameters(model, cmd);
                    cmd.Parameters.AddWithValue("@UserIdCreated", _getId());
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

        public static void Update(SceneUpdateRequest model)
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Scene_Update";

                    MapCommonSceneParameters(model, cmd);
                    cmd.Parameters.AddWithValue("@Id", model.Id);

                    cmd.ExecuteNonQuery();

                }
                conn.Close();
            }
        }
        public static void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Scene_Delete";

                    cmd.Parameters.AddWithValue("@Id", id);

                    cmd.ExecuteNonQuery();

                }
                conn.Close();
            }
        }
        public static Scene SelectById(int id)
        {
            Scene scene = null;
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Scene_SelectById";
                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        scene = MapScene(reader);
                    }
                }
                conn.Close();
            }
            return scene;
        }

        public static List<Scene> SelectBySequenceId(int sequenceId)
        {
            List<Scene> list = null;
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Scene_SelectBySequenceId";
                    cmd.Parameters.AddWithValue("@SequenceId", sequenceId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Scene scene = MapScene(reader);

                        if (list == null)
                        {
                            list = new List<Scene>();
                        }
                        list.Add(scene);
                    }
                }
                conn.Close();
            }
            return list;
        }

        private static Scene MapScene(SqlDataReader reader)
        {
            Scene scene = new Scene();
            int ord = 0;

            scene.Id = reader.GetSafeInt32(ord++);
            scene.SequenceId = reader.GetSafeInt32(ord++);
            scene.Title = reader.GetSafeString(ord++);
            scene.Summary = reader.GetSafeString(ord++);
            scene.ProtagonistId = reader.GetSafeInt32(ord++);
            scene.AntagonistId = reader.GetSafeInt32(ord++);
            scene.Conflict = reader.GetSafeString(ord++);
            scene.PhysicalGoal = reader.GetSafeString(ord++);
            scene.EmotionalGoal = reader.GetSafeString(ord++);
            scene.Turn = reader.GetSafeString(ord++);
            scene.DateCreated = reader.GetSafeDateTime(ord++);
            scene.DateModified = reader.GetSafeDateTime(ord++);
            scene.Setting = reader.GetSafeString(ord++);
            return scene;
        }

        private static void MapCommonSceneParameters(SceneAddRequest model, SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@SequenceId", model.SequenceId);
            cmd.Parameters.AddWithValue("@Title", model.Title ?? String.Empty);
            cmd.Parameters.AddWithValue("@Summary", model.Summary ?? String.Empty);
            cmd.Parameters.AddWithValue("@ProtagonistId", model.ProtagonistId);
            cmd.Parameters.AddWithValue("@AntagonistId", model.AntagonistId);
            cmd.Parameters.AddWithValue("@Conflict", model.Conflict ?? String.Empty);
            cmd.Parameters.AddWithValue("@PhysicalGoal", model.PhysicalGoal ?? String.Empty);
            cmd.Parameters.AddWithValue("@EmotionalGoal", model.EmotionalGoal ?? String.Empty);
            cmd.Parameters.AddWithValue("@Turn", model.Turn ?? String.Empty);
            cmd.Parameters.AddWithValue("@Setting", model.Setting ?? String.Empty);
        }
    }
}