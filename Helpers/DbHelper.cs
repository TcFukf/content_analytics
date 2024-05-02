using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Td.Api;

namespace social_analytics.Helpers
{
    static public class DbHelper
    {
        static DbHelper()
        {

        }
        static async private Task<DbConnection> GetConnection()
        {
            if (_connecion == null)
            {
                _connecion = new NpgsqlConnection(ConfigHelper.ConnectionString);
            }
            if (_connecion.State == System.Data.ConnectionState.Closed)
            {
                await _connecion.OpenAsync();
            }
            return _connecion;
        }
        private static NpgsqlConnection _connecion;
        public static async Task<int> ExecuteAsync<T>(string sql, T model)
        {
            var connection = await GetConnection();
            return await connection.ExecuteAsync(sql, model);
        }
        public static async Task<int> ExecuteAsync(string sql, object model)
        {
            var connection = await GetConnection();
            return await connection.ExecuteAsync(sql, model);
        }
        //QueryFirstOrDefaultAsync
        public static async Task<T> QueryFirstOrDefaultAsync<T>(string sql, T model) where T : class
        {
            var connection = await GetConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(sql, model);
        }
        public static async Task<IEnumerable<T>> Query<T>(string sql, T model) where T : class
        {
            var connection = await GetConnection();
            return await connection.QueryAsync<T>(sql, model);
        }
        public static async Task<IEnumerable<T>> Query<T>(string sql, object model) where T : class
        {
            var connection = await GetConnection();
            return await connection.QueryAsync<T>(sql, model);
        }
        internal static async Task<int> ExecuteScalarAsync<T>(string sql, T model)
        {
            var connection = await GetConnection();
            return await connection.ExecuteScalarAsync<int>(sql, model);
        }
    }
}
