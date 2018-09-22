using ForeSight.Web.Domain;
using ForeSight.Web.Models.Requests;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;

namespace ForeSight.Web.Services
{
    public class ActService
    {
        public static int Insert(ActAddRequest model)
        {
            int id = 0;

            using(SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using(SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Act_Insert";

                    MapCommonActParameters(model, cmd);
                    cmd.Parameters.AddWithValue("@UserIdCreated", HttpContext.Current.User.Identity.GetUserId());
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
        public static void Update(ActUpdateRequest model)
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Act_Update";

                    MapCommonActParameters(model, cmd);
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
                    cmd.CommandText = "dbo.Act_Delete";

                    cmd.Parameters.AddWithValue("@Id", id);

                    cmd.ExecuteNonQuery();

                }
                conn.Close();
            }
        }
        public static Act SelectById(int id)
        {
            Act act = null;
            List<Scene> scenes = null;

            using(SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Act_SelectById";
                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        act = MapAct(reader);
                    }
                    reader.NextResult();

                    while (reader.Read())
                    {
                        int i = reader.GetSafeInt32(0);
                        if (act.ActIds == null)
                        {
                            act.ActIds = new List<int>();
                        }
                        act.ActIds.Add(i);
                    }
                    reader.NextResult();

                    while(reader.Read())
                    {
                        Sequence seq = MapSequence(reader);
                        if(act.Sequences == null)
                        {
                            act.Sequences = new List<Sequence>();
                        }
                        act.Sequences.Add(seq);
                    }
                    reader.NextResult();

                    while (reader.Read())
                    {
                        Scene scene = MapScene(reader);
                        Sequence seq = act.Sequences.Find(item => item.Id == scene.SequenceId);
                        if (seq.Scenes == null)
                        {
                            seq.Scenes = new List<Scene>();
                        }
                        seq.Scenes.Add(scene);
                        if (scenes == null)
                        {
                            scenes = new List<Scene>();
                        }
                        scenes.Add(scene);
                    }
                    reader.NextResult();

                    while(reader.Read())
                    {
                        CharacterScene character = MapCharacterScene(reader);

                        Scene scene = scenes.Find(sce => sce.Id == character.SceneId);

                        if(scene.Characters == null)
                        {
                            scene.Characters = new List<CharacterScene>();
                        }

                        scene.Characters.Add(character);
                    }
                }
                conn.Close();
            }
            return act;
        }

        public static List<Act> SelectByProjectId(int projectId)
        {
            List<Act> list = null;
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Act_SelectByProjectId";
                    cmd.Parameters.AddWithValue("@ProjectId", projectId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Act act = MapAct(reader);

                        if (list == null)
                        {
                            list = new List<Act>();
                        }
                        list.Add(act);
                    }
                }
                conn.Close();
            }
            return list;
        }

        private static Act MapAct(SqlDataReader reader)
        {
            Act act = new Act();
            int ord = 0;

            act.Id = reader.GetSafeInt32(ord++);
            act.ProjectId = reader.GetSafeInt32(ord++);
            act.ProjectTitle = reader.GetSafeString(ord++);
            act.StoryTypeId = reader.GetSafeInt32(ord++);
            act.Title = reader.GetSafeString(ord++);
            act.Summary = reader.GetSafeString(ord++);
            act.Notes = reader.GetSafeString(ord++);
            act.CentralQuestion = reader.GetSafeString(ord++);
            act.DateCreated = reader.GetSafeDateTime(ord++);
            act.DateModified = reader.GetSafeDateTime(ord++);
            return act;
        }

        private static void MapCommonActParameters(ActAddRequest model, SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@Title", model.Title ?? String.Empty);
            cmd.Parameters.AddWithValue("@Summary", model.Summary ?? String.Empty);
            cmd.Parameters.AddWithValue("@Notes", model.Notes ?? String.Empty);
            cmd.Parameters.AddWithValue("@CentralQuestion", model.CentralQuestion ?? String.Empty);
            cmd.Parameters.AddWithValue("@ProjectId", model.ProjectId);
        }

        private static Sequence MapSequence(SqlDataReader reader)
        {
            Sequence seq = new Sequence();
            int ord = 0;

            seq.Id = reader.GetSafeInt32(ord++);
            seq.ActId = reader.GetSafeInt32(ord++);
            seq.Title = reader.GetSafeString(ord++);
            seq.Summary = reader.GetSafeString(ord++);
            seq.Notes = reader.GetSafeString(ord++);
            seq.CentralQuestion = reader.GetSafeString(ord++);
            seq.DateCreated = reader.GetSafeDateTime(ord++);
            seq.DateModified = reader.GetSafeDateTime(ord++);
            return seq;
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