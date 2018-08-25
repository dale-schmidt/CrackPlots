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
    public class CharacterSceneService
    {
        public static void Insert(CharacterSceneRequest model)
        {
            using(SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using(SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.CharacterScene_Insert";
                    MapCharacterScene(model, cmd);

                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            return;
        }
        public static void Update(CharacterSceneRequest model)
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.CharacterScene_Update";
                    MapCharacterScene(model, cmd);

                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            return;
        }
        public static void Delete(int characterId, int sceneId)
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.CharacterScene_Delete";
                    cmd.Parameters.AddWithValue("@CharacterId", characterId);
                    cmd.Parameters.AddWithValue("@SceneId", sceneId);

                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            return;
        }
        public static List<CharacterScene> SelectBySceneId(int sceneId)
        {
            List<CharacterScene> list = null;

            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using(SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.CharacterScene_GetBySceneId";
                    cmd.Parameters.AddWithValue("@SceneId", sceneId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        CharacterScene cs = MapCharacterScene(reader);

                        if (list == null)
                        {
                            list = new List<CharacterScene>();
                        }

                        list.Add(cs);
                    }
                }
                conn.Close();
            }
            return list;
        }
        public static List<CharacterSceneExitTypes> SelectAllExitTypes()
        {
            List<CharacterSceneExitTypes> list = null;

            using(SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using(SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.CharacterSceneExitTypes_SelectAll";

                    SqlDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        CharacterSceneExitTypes type = new CharacterSceneExitTypes();
                        int ord = 0;

                        type.Id = reader.GetSafeInt32(ord++);
                        type.Name = reader.GetSafeString(ord++);

                        if(list == null)
                        {
                            list = new List<CharacterSceneExitTypes>();
                        }

                        list.Add(type);
                    }
                }
                conn.Close();
            }
            return list;
        }

        private static void MapCharacterScene(CharacterSceneRequest model, SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@CharacterId", model.CharacterId);
            cmd.Parameters.AddWithValue("@SceneId", model.SceneId);
            cmd.Parameters.AddWithValue("@CharacterSceneExitTypeId", model.CharacterSceneExitTypeId);
            cmd.Parameters.AddWithValue("@Start", model.Start ?? String.Empty);
            cmd.Parameters.AddWithValue("@End", model.End ?? String.Empty);
            cmd.Parameters.AddWithValue("@Notes", model.Notes ?? String.Empty);
            cmd.Parameters.AddWithValue("@PhysicalGoal", model.PhysicalGoal ?? String.Empty);
            cmd.Parameters.AddWithValue("@EmotionalGoal", model.EmotionalGoal ?? String.Empty);
            cmd.Parameters.AddWithValue("@Obstacle", model.Obstacle ?? String.Empty);
        }
        private static CharacterScene MapCharacterScene(SqlDataReader reader)
        {
            CharacterScene character = new CharacterScene();
            character.Character = new Character();
            int ord = 0;

            character.Character.Id = reader.GetSafeInt32(ord++);
            character.SceneId = reader.GetSafeInt32(ord++);
            character.CharacterSceneExitTypeId = reader.GetSafeInt32(ord++);
            character.Start = reader.GetSafeString(ord++);
            character.End = reader.GetSafeString(ord++);
            character.Notes = reader.GetSafeString(ord++);
            character.PhysicalGoal = reader.GetSafeString(ord++);
            character.EmotionalGoal = reader.GetSafeString(ord++);
            character.Obstacle = reader.GetSafeString(ord++);
            character.Character.Name = reader.GetSafeString(ord++);
            character.Character.Want = reader.GetSafeString(ord++);
            character.Character.Need = reader.GetSafeString(ord++);
            character.Character.Biography = reader.GetSafeString(ord++);
            return character;
        }
    }
}