using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace ClinicRayeheSib.Services
{
    public interface IDbServices
    {
        DataTable ExecuteQuery(string sql, params SqlParameter[] parameters);
        Task<DataTable> ExecuteQueryAsync(string sql, params SqlParameter[] parameters);
        int ExecuteNonQuery(string sql, params SqlParameter[] parameters);
        Task<int> ExecuteNonQueryAsync(string sql, params SqlParameter[] parameters);
        object ExecuteScalar(string sql, params SqlParameter[] parameters);
        Task<object> ExecuteScalarAsync(string sql, params SqlParameter[] parameters);
        List<Dictionary<string, object>> ToDictionaryList(DataTable dt);
        Task<List<Dictionary<string, object>>> QueryToDictionaryListAsync(string sql, params SqlParameter[] parameters);
        DataTable ExecuteProcedure(string procedureName, params SqlParameter[] parameters);
        Task<DataTable> ExecuteProcedureAsync(string procedureName, params SqlParameter[] parameters);
        List<SqlParameter> GetStoredProcedureParameters(string NameStoredProcedure);
        void MapParameters(List<SqlParameter> parameters, Dictionary<string, object> data);
        DataTable ReadTable(string StoreProcedureName);
    }
}