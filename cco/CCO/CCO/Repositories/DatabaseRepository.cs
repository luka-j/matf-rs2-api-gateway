using CCO.Entities;
using Dapper;
using Npgsql;

namespace CCO.Repositories
{
    public class DatabaseRepository
    {
        public async Task<IEnumerable<DatabaseEntry>> ReadAll (Datasource database)
        {
            using var connection = GetConnection(database.ConnectionString);

            var entries = await connection.QueryAsync<DatabaseEntry>("SELECT * FROM public.test");

            return entries;
        }

        public async Task<bool> CreateEntry (Datasource database, int amount)
        {
            using var connection = GetConnection(database.ConnectionString);

            var affected = await connection.ExecuteAsync(
                "INSERT INTO public.test (amount) VALUES (@Amount)", new { Amount = amount });

            return affected != 0;
        }

        public async Task<bool> DeleteEntry (Datasource database, string id)
        {
            using var connection = GetConnection(database.ConnectionString);

            var affected = await connection.ExecuteAsync("DELETE FROM public.test WHERE id = @Id", new { Id = id });

            return affected != 0;
        }


        private static NpgsqlConnection GetConnection (string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}
