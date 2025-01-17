﻿using System;
using System.Collections.Generic;
using Npgsql;

namespace StudyBuddy.Persistence
{
    public class QueryHelper
    {
        private readonly string connection_string;
        private readonly Dictionary<string, object> parameters = new Dictionary<string, object>();

        public QueryHelper(string connection_string)
        {
            this.connection_string = connection_string;
        }

        public QueryHelper(string connection_string, object parameters)
        {
            this.connection_string = connection_string;
            AddParameters(parameters);
        }

        public void AddParameter(string name, object value)
        {
            parameters.Add(name, value);
        }

        public void AddParameters(object obj)
        {
            foreach (var p in obj.GetType().GetProperties())
                parameters.Add(":" + p.Name, p.GetValue(obj, null));
        }

        public DataSet ExecuteQuery(string sql)
        {
            DataSet set = new DataSet();

            try
            {
                using (var connection = new NpgsqlConnection(connection_string))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        foreach (var param in parameters)
                            cmd.Parameters.AddWithValue(param.Key, param.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            set.FillFromReader(reader);
                        }
                    }
                }
            }
            finally
            {
                parameters.Clear();
            }

            return set;
        }

        public void ExecuteNonQuery(string sql)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connection_string))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        foreach (var param in parameters)
                            cmd.Parameters.AddWithValue(param.Key, param.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            finally
            {
                parameters.Clear();
            }
        }

        public int ExecuteQueryToSingleInt(string sql)
        {
            var result = 0;

            try
            {
                using (var connection = new NpgsqlConnection(connection_string))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        foreach (var param in parameters)
                            cmd.Parameters.AddWithValue(param.Key, param.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                result = reader.GetInt32(0);
                        }
                    }
                }
            }
            finally
            {
                parameters.Clear();
            }
            
            return result;
        }

        public int ExecuteScalar(string sql)
        {
            var result = 0;

            try
            {
                using (var connection = new NpgsqlConnection(connection_string))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        foreach (var param in parameters)
                            cmd.Parameters.AddWithValue(param.Key, param.Value);

                        result = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            finally
            {
                parameters.Clear();
            }
            
            return result;
        }

        public void Delete(string table_name, string field_name, int id)
        {
            AddParameter(":id", id);
            ExecuteNonQuery(string.Format("delete from {0} where {1}=:id", table_name, field_name));
        }

        public int GetCount(string table_name)
        {
            var result = 0;

            try
            {
                using (var connection = new NpgsqlConnection(connection_string))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand("SELECT count(*) as count FROM " + table_name, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                result = reader.GetInt32(0);
                        }
                    }
                }
            }
            finally
            {
                parameters.Clear();
            }
            
            return result;
        }

        public bool TableExists(string table_name)
        {
            var result = false;

            try
            {
                var sql = "SELECT EXISTS (SELECT FROM pg_tables WHERE tablename=:table_name)";

                using (var connection = new NpgsqlConnection(connection_string))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue(":table_name", table_name);
                        result = Convert.ToBoolean(cmd.ExecuteScalar());
                    }
                }
            }
            finally
            {
                parameters.Clear();
            }
            
            return result;
        }
    }
}