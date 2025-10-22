using Npgsql;
using System.Data;

namespace BOZea.Helpers
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable ExecuteQuery(string query)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            using var cmd = new NpgsqlCommand(query, conn);
            using var adapter = new NpgsqlDataAdapter(cmd);

            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public int ExecuteNonQuery(string query)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand(query, conn);
            return cmd.ExecuteNonQuery();
        }
    }
}
