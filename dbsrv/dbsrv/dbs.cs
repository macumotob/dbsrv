using Microsoft.VisualBasic;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;


namespace dbsrv
{
    public class dbs
    {
        public const string DB_HOST = "element-prod.cmeu2ebyhpap.eu-central-1.rds.amazonaws.com";//"3.77.85.138";//
        public const string DB_USER = "element";
        public const string DB_PASSWORD = "dmfll9exoW7kKZ8lm";//"11PV2myUgiJLJhU-a";
        public const int DB_PORT = 3306;

        private static string _connString;
        private static string _GetDBConnection(string host, int port, string database, string username, string password)
        {

            return "Server=" + host + ";Database=" + database
                      + ";port=" + port + ";User Id=" + username + ";password=" + password
                       //+ ";TLS Version=TLS 1.3";
                       + ";SslMode=none;"
                       ;
        }
        private static string CreateConnectionString(string database)
        {
            return _GetDBConnection(DB_HOST, DB_PORT, database, DB_USER, DB_PASSWORD);
        }
        public static void Use(string database)
        {
            Init(CreateConnectionString(database));
        }
        private static void Init(string connectionString)
        {
            _connString = connectionString;
        }

        private static MySqlConnection GetConnection()
        {
            if (string.IsNullOrEmpty(_connString))
                throw new InvalidOperationException("DB not initialized. Call DB.Init(connectionString).");

            return new MySqlConnection(_connString);
        }

        // -------------------------------
        // Execute: INSERT / UPDATE / DELETE
        // -------------------------------
        public static async Task<int> ExecuteAsync(
            string sql,
            IEnumerable<MySqlParameter>? parameters = null)
        {
            await using var conn = GetConnection();
            await conn.OpenAsync();

            await using var cmd = new MySqlCommand(sql, conn);

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            return await cmd.ExecuteNonQueryAsync();
        }

        // -------------------------------
        // Query: SELECT list
        // -------------------------------
        public static async Task<List<T>> QueryAsync<T>(
            string sql,
            Func<MySqlDataReader, T> map,
            IEnumerable<MySqlParameter>? parameters = null)
        {
            var result = new List<T>();

            await using var conn = GetConnection();
            await conn.OpenAsync();

            await using var cmd = new MySqlCommand(sql, conn);

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(map(reader));
            }

            return result;
        }

        // -------------------------------
        // QuerySingle
        // -------------------------------
        public static async Task<T?> QuerySingleAsync<T>(
            string sql,
            Func<MySqlDataReader, T> map,
            IEnumerable<MySqlParameter>? parameters = null)
        {
            await using var conn = GetConnection();
            await conn.OpenAsync();

            await using var cmd = new MySqlCommand(sql, conn);

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return map(reader);
            }

            return default;
        }

        // -------------------------------
        // QueryScalar
        // -------------------------------
        public static async Task<T?> QueryScalarAsync<T>(
            string sql,
            IEnumerable<MySqlParameter>? parameters = null)
        {
            await using var conn = GetConnection();
            await conn.OpenAsync();

            await using var cmd = new MySqlCommand(sql, conn);

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            object? obj = await cmd.ExecuteScalarAsync();

            if (obj == null || obj is DBNull)
                return default;

            return (T)Convert.ChangeType(obj, typeof(T));
        }

        // -------------------------------
        // ExecuteReader (низкого уровня)
        // -------------------------------
        public static async Task ExecuteReaderAsync(
            string sql,
            Func<MySqlDataReader, Task> rowHandler,
            IEnumerable<MySqlParameter>? parameters = null)
        {
            await using var conn = GetConnection();
            await conn.OpenAsync();

            await using var cmd = new MySqlCommand(sql, conn);

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                await rowHandler(reader);
            }
        }

        // -------------------------------
        // Stored Procedure
        // -------------------------------
        public static async Task<int> ExecuteProcedureAsync(
            string procedureName,
            IEnumerable<MySqlParameter>? parameters = null)
        {
            await using var conn = GetConnection();
            await conn.OpenAsync();

            await using var cmd = new MySqlCommand(procedureName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            return await cmd.ExecuteNonQueryAsync();
        }

        // -------------------------------
        // Transaction
        // -------------------------------
        public static async Task ExecuteTransactionAsync(
            Func<MySqlConnection, MySqlTransaction, Task> action)
        {
            await using var conn = GetConnection();
            await conn.OpenAsync();

            await using var tx = await conn.BeginTransactionAsync();

            try
            {
                await action(conn, (MySqlTransaction)tx);
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
        // Додати до класу dbs:

        // -------------------------------
        // ExecuteMultipleResultSetsAsync
        // -------------------------------
        public static async Task ExecuteMultipleResultSetsAsync(
            string sql,
            Func<MySqlDataReader, Task> resultSetHandler,
            IEnumerable<MySqlParameter>? parameters = null,
            CommandType commandType = CommandType.Text)
        {
            await using var conn = GetConnection();
            await conn.OpenAsync();

            await using var cmd = new MySqlCommand(sql, conn)
            {
                CommandType = commandType
            };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            await using var reader = await cmd.ExecuteReaderAsync();

            do
            {
                await resultSetHandler(reader);
            }
            while (await reader.NextResultAsync());
        }
    }
}

