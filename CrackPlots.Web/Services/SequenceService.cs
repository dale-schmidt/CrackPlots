using ForeSight.Web.Domain;
using ForeSight.Web.Models.Requests;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace ForeSight.Web.Services
{
    public class SequenceService
    {
        private static string _getId()
        {
            return HttpContext.Current.User.Identity.GetUserId();
            //return "f7987305-3b9d-4d5c-9790-b39b4373a8e1";
        }
        public static int Insert(SequenceAddRequest model)
        {
            int id = 0;

            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Sequence_Insert";

                    MapCommonSequenceParameters(model, cmd);
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
        public static void Update(SequenceUpdateRequest model)
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Sequence_Update";

                    MapCommonSequenceParameters(model, cmd);
                    cmd.Parameters.AddWithValue("@Id", model.Id);

                    cmd.ExecuteNonQuery();

                }
                conn.Close();
            }
        }
        public static int TitlesUpdate(SequenceTitlesUpdateRequest model)
        {
            int id = 0;

            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Sequence_TitlesUpdate";

                    PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(SequenceUpdateRequest));
                    DataTable table = new DataTable();
                    foreach (PropertyDescriptor prop in properties)
                    {
                        table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    }
                    foreach (SequenceUpdateRequest item in model.Sequences)
                    {
                        DataRow row = table.NewRow();
                        foreach (PropertyDescriptor prop in properties)
                            row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                        table.Rows.Add(row);
                    }

                    cmd.Parameters.AddWithValue("@List", table);

                    cmd.ExecuteNonQuery();
                }
                if (model.Sequences.Find(sce => sce.Id == null) != null)
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandText = "dbo.Sequence_InsertWithScenes";
                        SequenceAddRequest add = new SequenceAddRequest();
                        add = model.Sequences.Find(seq => seq.Id == null);

                        MapCommonSequenceParameters(add, cmd);
                        cmd.Parameters.AddWithValue("@UserIdCreated", _getId());
                        SqlParameter outputId = new SqlParameter("@SequenceId", System.Data.SqlDbType.Int)
                        {
                            Direction = System.Data.ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputId);

                        cmd.ExecuteNonQuery();

                        id = (int)outputId.Value;
                    }
                }
                conn.Close();
            }
            return id;
        }
        public static int ScenesUpdate(SequenceScenesUpdateRequest model)
        {
            int id = 0;

            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Sequence_ScenesUpdate";

                    PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(SceneUpdateRequest));
                    DataTable table = new DataTable();
                    foreach (PropertyDescriptor prop in properties)
                    {
                        table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    }
                    foreach (SceneUpdateRequest item in model.Scenes)
                    {
                        DataRow row = table.NewRow();
                        foreach (PropertyDescriptor prop in properties)
                            row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                        table.Rows.Add(row);
                    }

                    cmd.Parameters.AddWithValue("@List", table);

                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            if (model.Scenes.Find(sce => sce.Id == null) != null)
            {
                SceneAddRequest add = new SceneAddRequest();
                add = model.Scenes.Find(sce => sce.Id == null);
                id = SceneService.Insert(add);
            }
            return id;
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
                    cmd.CommandText = "dbo.Sequence_Delete";

                    cmd.Parameters.AddWithValue("@Id", id);

                    cmd.ExecuteNonQuery();

                }
                conn.Close();
            }
        }
        public static Sequence SelectById(int id)
        {
            Sequence seq = null;
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Sequence_SelectById";
                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        seq = MapSequence(reader);
                    }
                }
                conn.Close();
            }
            return seq;
        }
        public static List<Sequence> SelectByActId(int actId)
        {
            List<Sequence> list = null;
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "dbo.Sequence_SelectByActId";
                    cmd.Parameters.AddWithValue("@ActId", actId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Sequence seq = MapSequence(reader);

                        if (list == null)
                        {
                            list = new List<Sequence>();
                        }
                        list.Add(seq);
                    }
                }
                conn.Close();
            }
            return list;
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

        private static void MapCommonSequenceParameters(SequenceAddRequest model, SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@ActId", model.ActId);
            cmd.Parameters.AddWithValue("@Title", model.Title ?? String.Empty);
            cmd.Parameters.AddWithValue("@Summary", model.Summary ?? String.Empty);
            cmd.Parameters.AddWithValue("@Notes", model.Notes ?? String.Empty);
            cmd.Parameters.AddWithValue("@CentralQuestion", model.CentralQuestion ?? String.Empty);
        }
    }
}