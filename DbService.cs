using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NuGet.Protocol.Plugins;

namespace ClinicRayeheSib.Services
{
    public class DbService : IDbServices
    {
        private readonly string _connectionString;

        public DbService(IConfiguration configuration)
        {
            // از appsettings.json خوانده می‌شود 
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
        }

        public DataTable ExecuteQuery(string sql, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, con))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }
        public async Task<DataTable> ExecuteQueryAsync(string sql, params SqlParameter[] parameters)
        {
            var dt = new DataTable();
            using (var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (parameters != null && parameters.Length > 0)
                        cmd.Parameters.AddRange(parameters);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader); // بارگذاری از reader به DataTable
                    }
                }
            }
            return dt;
        }

        public int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, con))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public async Task<int> ExecuteNonQueryAsync(string sql, params SqlParameter[] parameters)
        {
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, con))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                await con.OpenAsync();
                return await cmd.ExecuteNonQueryAsync();
            }
        }

        public object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, con))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                con.Open();
                return cmd.ExecuteScalar();
            }
        }

        public async Task<object> ExecuteScalarAsync(string sql, params SqlParameter[] parameters)
        {
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, con))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                await con.OpenAsync();
                return await cmd.ExecuteScalarAsync();
            }
        }

        public List<Dictionary<string, object>> ToDictionaryList(DataTable dt)
        {
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                    dict[col.ColumnName] = row[col];
                list.Add(dict);
            }
            return list;
        }

        public async Task<List<Dictionary<string, object>>> QueryToDictionaryListAsync(string sql, params SqlParameter[] parameters)
        {
            var dt = await ExecuteQueryAsync(sql, parameters);
            return ToDictionaryList(dt);
        }
        public DataTable ExecuteProcedure(string procedureName, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();

            using (var con = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(procedureName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }
        public async Task<DataTable> ExecuteProcedureAsync(string procedureName, params SqlParameter[] parameters)
        {
            var dt = new DataTable();

            using (var con = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(procedureName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                await con.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }

            return dt;
        }
        
        public List<SqlParameter> GetStoredProcedureParameters(string procedureName)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(procedureName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlCommandBuilder.DeriveParameters(cmd);

                foreach (SqlParameter parameter in cmd.Parameters)
                {
                    if (parameter.Direction == ParameterDirection.ReturnValue)
                        continue;
                    parameters.Add((SqlParameter)((ICloneable)parameter).Clone());
                }
            }
            return parameters;
        }
        public void MapParameters( List<SqlParameter> parameters, Dictionary<string, object> data)
        {
            foreach (var param in parameters)
            {
                string paramName = param.ParameterName.TrimStart('@');
                var matchingKey = data.Keys.FirstOrDefault(k =>
                    string.Equals(k, paramName, StringComparison.OrdinalIgnoreCase));
                if (matchingKey != null)
                {
                    param.Value = data[matchingKey] ?? DBNull.Value;
                }
                else
                {
                    param.Value = DBNull.Value;
                }
            }
        }
        public DataTable ReadTable(string StoreProcedureName)
        {
            string OperationRead = "Read";
            List<SqlParameter> AllParams = GetStoredProcedureParameters(StoreProcedureName);
            Dictionary<string, object> DicParams = new Dictionary<string, object>();
            DicParams["Operation"] = OperationRead;
            MapParameters(AllParams, DicParams);
            DataTable AllInTable = ExecuteProcedure(StoreProcedureName, AllParams.ToArray());
            return AllInTable;
        }
    }
}
