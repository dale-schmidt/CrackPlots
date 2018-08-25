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
    public class CharacterService
    {
        public static int Insert(CharacterAddRequest model)
        {
            int id = 0;

            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Character_Insert";
                    MapCommonCharacterParameters(model, cmd);
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

        public static void Update(CharacterUpdateRequest model)
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Character_Update";

                    MapCommonCharacterParameters(model, cmd);
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
                    cmd.CommandText = "dbo.Character_Delete";

                    cmd.Parameters.AddWithValue("@Id", id);

                    cmd.ExecuteNonQuery();

                }
                conn.Close();
            }
        }
        public static Character SelectById(int id)
        {
            Character character = null;
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Character_SelectById";
                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        character = MapCharacter(reader);
                    }
                }
                conn.Close();
            }
            return character;
        }

        public static List<Character> SelectBySceneId(int sceneId)
        {
            List<Character> list = null;
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Character_SelectBySceneId";
                    cmd.Parameters.AddWithValue("@SceneId", sceneId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Character character = MapCharacter(reader);

                        if (list == null)
                        {
                            list = new List<Character>();
                        }
                        list.Add(character);
                    }
                }
                conn.Close();
            }
            return list;
        }

        public static List<Character> SelectByProjectId(int projectId)
        {
            List<Character> list = null;
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Character_SelectByProjectId";
                    cmd.Parameters.AddWithValue("@ProjectId", projectId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Character character = MapCharacter(reader);

                        if (list == null)
                        {
                            list = new List<Character>();
                        }
                        list.Add(character);
                    }
                }
                conn.Close();
            }
            return list;
        }

        private static void MapCommonCharacterParameters(CharacterAddRequest model, SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Want", model.Want);
            cmd.Parameters.AddWithValue("@Need", model.Need);
            cmd.Parameters.AddWithValue("@Biography", model.Biography);
            cmd.Parameters.AddWithValue("@ProjectId", model.ProjectId);
        }

        private static Character MapCharacter(SqlDataReader reader)
        {
            Character character = new Character();
            int ord = 0;

            character.Id = reader.GetSafeInt32(ord++);
            character.Name = reader.GetSafeString(ord++);
            character.Want = reader.GetSafeString(ord++);
            character.Need = reader.GetSafeString(ord++);
            character.Biography = reader.GetSafeString(ord++);
            character.ProjectId = reader.GetSafeInt32(ord++);
            return character;
        }
    }
}