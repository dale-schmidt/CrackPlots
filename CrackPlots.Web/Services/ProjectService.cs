using ForeSight.Web.Domain;
using ForeSight.Web.Models.Requests;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace ForeSight.Web.Services
{
    public class ProjectService
    {
        private static string _getId()
        {
            return HttpContext.Current.User.Identity.GetUserId();
        }

        public static int Insert(ProjectAddRequest model)
        {
            int id = 0;

            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Project_Insert";
                    if(model.StoryTypeId == 6)
                    {
                        model.Plots = new List<Plot>();
                        Plot pA = new Plot();
                        pA.PlotName = "A";
                        model.Plots.Add(pA);
                        Plot pB = new Plot();
                        pB.PlotName = "B";
                        model.Plots.Add(pB);
                        Plot pC = new Plot();
                        pC.PlotName = "C";
                        model.Plots.Add(pC);
                    }
                    MapCommonProjectParameters(model, cmd);
                    cmd.Parameters.AddWithValue("@AspNetUserId", _getId());

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
        public static int InsertStructured(ProjectAddRequest model)
        {
            int id = 0;

            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    switch (model.StoryTypeId)
                    {
                        case 1:
                            cmd.CommandText = "dbo.Project_InsertStructuredFeature";
                            break;
                        case 2:
                            cmd.CommandText = "dbo.Project_InsertTvSeries";
                            break;
                        case 6:
                            cmd.CommandText = "dbo.Project_InsertTvEpisode";
                            cmd.Parameters.AddWithValue("@ProjectId", model.ProjectId);
                            cmd.Parameters.AddWithValue("@SeasonId", model.SeasonId);
                            cmd.Parameters.AddWithValue("@ActsCount", model.ActsCount);
                            break;
                        case 7:
                            cmd.CommandText = "dbo.Project_InsertTvSeason";
                            cmd.Parameters.AddWithValue("@ProjectId", model.ProjectId);
                            break;
                        default:
                            cmd.CommandText = "dbo.Project_Insert";
                            break;
                    }
                    if (model.StoryTypeId == 6)
                    {
                        model.Plots = new List<Plot>();
                        Plot pA = new Plot();
                        pA.PlotName = "A";
                        model.Plots.Add(pA);
                        Plot pB = new Plot();
                        pB.PlotName = "B";
                        model.Plots.Add(pB);
                        Plot pC = new Plot();
                        pC.PlotName = "C";
                        model.Plots.Add(pC);
                    }
                    MapCommonProjectParameters(model, cmd);
                    cmd.Parameters.AddWithValue("@AspNetUserId", _getId());

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

        public static void Update(ProjectUpdateRequest model)
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Project_Update";
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    MapCommonProjectParameters(model, cmd);

                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
        public static Project SelectById(int id)
        {
            Project proj = null;
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Project_SelectById";
                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        proj = MapProject(reader);
                    }
                    reader.NextResult();

                    if (proj.StoryType.Id == 2)
                    {
                        while (reader.Read())
                        {
                            Project p = MapProject(reader);
                            if (p.SeasonId == 0)
                            {
                                if (proj.Seasons == null)
                                {
                                    proj.Seasons = new List<Project>();
                                }
                                proj.Seasons.Add(p);
                            }
                            else
                            {
                                Project season = proj.Seasons.Find(sea => sea.Id == p.SeasonId);
                                if (season.Episodes == null)
                                {
                                    season.Episodes = new List<Project>();
                                }
                                season.Episodes.Add(p);
                            }
                        }
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            Act a = MapAct(reader);
                            if (proj.Acts == null)
                            {
                                proj.Acts = new List<Act>();
                            }
                            proj.Acts.Add(a);
                        }
                    }
                    reader.NextResult();

                    while (reader.Read())
                    {
                        Plot p = MapPlot(reader);
                        if(proj.Plots == null)
                        {
                            proj.Plots = new List<Plot>();
                        }
                        proj.Plots.Add(p);
                    }
                    reader.NextResult();

                    while (reader.Read())
                    {
                        Person p = MapPerson(reader);
                        if (proj.Users == null)
                        {
                            proj.Users = new List<Person>();
                        }
                        proj.Users.Add(p);
                    }
                }
                conn.Close();
            }
            return proj;
        }
        public static Project SelectEpisodeById(int id)
        {
            Project proj = null;
            List<Sequence> sequences = null;
            List<Scene> scenes = null;
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Project_SelectEpisodeById";
                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        proj = MapProject(reader);
                    }
                    reader.NextResult();

                    while (reader.Read())
                    {
                        int i = reader.GetSafeInt32(0);
                        if(proj.EpisodeIds == null)
                        {
                            proj.EpisodeIds = new List<int>();
                        }
                        proj.EpisodeIds.Add(i);
                    }
                    reader.NextResult();

                    while (reader.Read())
                    {
                        Plot p = MapPlot(reader);
                        if (proj.Plots == null)
                        {
                            proj.Plots = new List<Plot>();
                        }
                        proj.Plots.Add(p);
                    }
                    reader.NextResult();

                    while (reader.Read())
                    {
                        Act a = MapAct(reader);
                        if (proj.Acts == null)
                        {
                            proj.Acts = new List<Act>();
                        }
                        proj.Acts.Add(a);
                    }
                    reader.NextResult();

                    while (reader.Read())
                    {
                        Sequence seq = MapSequence(reader);
                        Act a = proj.Acts.Find(item => item.Id == seq.ActId);
                        if(a.Sequences == null)
                        {
                            a.Sequences = new List<Sequence>();
                        }
                        a.Sequences.Add(seq);
                        if (sequences == null)
                        {
                            sequences = new List<Sequence>();
                        }
                        sequences.Add(seq);
                    }
                    reader.NextResult();

                    while (reader.Read())
                    {
                        Scene sce = MapScene(reader);
                        Sequence seq = sequences.Find(item => item.Id == sce.SequenceId);
                        if (seq.Scenes == null)
                        {
                            seq.Scenes = new List<Scene>();
                        }
                        seq.Scenes.Add(sce);
                        if (scenes == null)
                        {
                            scenes = new List<Scene>();
                        }
                        scenes.Add(sce);
                    }
                    reader.NextResult();

                    while (reader.Read())
                    {
                        CharacterScene character = MapCharacterScene(reader);

                        Scene scene = scenes.Find(sce => sce.Id == character.SceneId);

                        if (scene.Characters == null)
                        {
                            scene.Characters = new List<CharacterScene>();
                        }

                        scene.Characters.Add(character);
                    }
                    reader.NextResult();

                    while (reader.Read())
                    {
                        Person p = MapPerson(reader);
                        if (proj.Users == null)
                        {
                            proj.Users = new List<Person>();
                        }
                        proj.Users.Add(p);
                    }
                }
                conn.Close();
            }
            return proj;
        }
        public static Project SelectByIdPrint(int id)
        {
            Project proj = null;
            Dictionary<int, int> SequenceActList = null;
            Dictionary<int, int> SceneSequenceList = null;
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Project_SelectByIdPrint";
                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        proj = MapProject(reader);
                    }
                    reader.NextResult();

                    while (reader.Read())
                    {
                        Act a = MapAct(reader);
                        if (proj.Acts == null)
                        {
                            proj.Acts = new List<Act>();
                        }
                        proj.Acts.Add(a);
                    }
                    reader.NextResult();

                    while(reader.Read())
                    {
                        Sequence seq = MapSequence(reader);
                        Act a = proj.Acts.Find(act => act.Id == seq.ActId);
                        if(a.Sequences == null)
                        {
                            a.Sequences = new List<Sequence>();
                        }
                        a.Sequences.Add(seq);
                        if(SequenceActList == null)
                        {
                            SequenceActList = new Dictionary<int, int>();
                        }
                        SequenceActList.Add(seq.Id, a.Id);
                    }
                    reader.NextResult();

                    while(reader.Read())
                    {
                        Scene sce = MapScene(reader);
                        Sequence seq = proj.Acts.Find(a => a.Id == SequenceActList[sce.SequenceId]).Sequences.Find(s => s.Id == sce.SequenceId);
                        if(seq.Scenes == null)
                        {
                            seq.Scenes = new List<Scene>();
                        }
                        seq.Scenes.Add(sce);
                        if (SceneSequenceList == null)
                        {
                            SceneSequenceList = new Dictionary<int, int>();
                        }
                        SceneSequenceList.Add(sce.Id, seq.Id);
                    }
                    reader.NextResult();

                    while(reader.Read())
                    {
                        CharacterScene cs = MapCharacterScene(reader);
                        Scene sce = proj.Acts.Find(a => a.Id == SequenceActList[SceneSequenceList[cs.SceneId]]).Sequences.Find(s => s.Id == SceneSequenceList[cs.SceneId]).Scenes.Find(c => c.Id == cs.SceneId);
                        if(sce.Characters == null)
                        {
                            sce.Characters = new List<CharacterScene>();
                        }
                        sce.Characters.Add(cs);
                    }
                }
                conn.Close();
            }
            return proj;
        }
        public static List<Project> SelectByPersonId(string aspNetUserId)
        {
            List<Project> list = null;
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Project_SelectByPersonId";
                    cmd.Parameters.AddWithValue("@AspNetUserId", aspNetUserId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Project proj = new Project();
                        proj = MapProject(reader);
                        if (list == null)
                        {
                            list = new List<Project>();
                        }
                        list.Add(proj);
                    }
                }
                conn.Close();
            }
            return list;
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
                    cmd.CommandText = "dbo.Project_Delete";
                    cmd.Parameters.AddWithValue("@Id", id);

                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        private static void MapCommonProjectParameters(ProjectAddRequest model, SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@Logline", model.Logline ?? String.Empty);
            cmd.Parameters.AddWithValue("@StoryTypeId", model.StoryTypeId);
            cmd.Parameters.AddWithValue("@Notes", model.Notes ?? String.Empty);
            if(model.StoryTypeId == 6)
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(Plot));
                DataTable table = new DataTable();
                foreach (PropertyDescriptor prop in properties)
                {
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
                foreach (Plot item in model.Plots)
                {
                    DataRow row = table.NewRow();
                    foreach (PropertyDescriptor prop in properties)
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    table.Rows.Add(row);
                }
                cmd.Parameters.AddWithValue("@List", table);
            }
        }

        private static Project MapProject(SqlDataReader reader)
        {
            Project proj = new Project();
            proj.StoryType = new StoryType();
            int ord = 0;

            proj.Id = reader.GetSafeInt32(ord++);
            proj.Title = reader.GetSafeString(ord++);
            proj.Logline = reader.GetSafeString(ord++);
            proj.StoryType.Id = reader.GetSafeInt32(ord++);
            proj.StoryType.Name = reader.GetSafeString(ord++);
            proj.Notes = reader.GetSafeString(ord++);
            proj.DateCreated = reader.GetSafeDateTime(ord++);
            proj.DateModified = reader.GetSafeDateTime(ord++);
            proj.ProjectId = reader.GetSafeInt32(ord++);
            proj.SeasonId = reader.GetSafeInt32(ord++);
            if (reader.FieldCount > 10)
            {
                proj.ProjectId = reader.GetSafeInt32(ord++);
                proj.SeasonId = reader.GetSafeInt32(ord++);
            }
            return proj;
        }
        private static Act MapAct(SqlDataReader reader)
        {
            Act act = new Act();
            int ord = 0;

            act.Id = reader.GetSafeInt32(ord++);
            act.ProjectId = reader.GetSafeInt32(ord++);
            act.Title = reader.GetSafeString(ord++);
            act.Summary = reader.GetSafeString(ord++);
            act.Notes = reader.GetSafeString(ord++);
            act.CentralQuestion = reader.GetSafeString(ord++);
            act.DateCreated = reader.GetSafeDateTime(ord++);
            act.DateModified = reader.GetSafeDateTime(ord++);
            return act;
        }
        private static Person MapPerson(SqlDataReader reader)
        {
            Person p = new Person();
            int ord = 0;

            p.Id = reader.GetSafeInt32(ord++);
            p.Name = reader.GetSafeString(ord++);
            p.Email = reader.GetSafeString(ord++);

            return p;
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
        private static Plot MapPlot(SqlDataReader reader)
        {
            Plot p = new Plot();
            int ord = 0;
            p.Id = reader.GetSafeInt32(ord++);
            p.ProjectId = reader.GetSafeInt32(ord++);
            p.PlotName = reader.GetSafeString(ord++);
            p.Description = reader.GetSafeString(ord++);
            return p;
        }
    }
}