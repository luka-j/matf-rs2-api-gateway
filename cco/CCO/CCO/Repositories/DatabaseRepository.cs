using CCO.Entities;
using Dapper;
using Npgsql;

namespace CCO.Repositories
{
    public class DatabaseRepository
    {
        public async Task<IEnumerable<DatabaseEntry>> ReadAll (DatabaseSource database)
        {
            using var connection = GetConnection(database);

            var entries = await connection.QueryAsync<DatabaseEntry>("SELECT * FROM public.test");

            return entries;
        }

        public async Task<bool> Create (DatabaseSource database, int amount)
        {
            using var connection = GetConnection(database);

            var affected = await connection.ExecuteAsync(
                "INSERT INTO public.test (amount) VALUES (@Amount)", new { Amount = amount });

            return affected != 0;
        }

        public async Task<bool> Delete (DatabaseSource database, string id)
        {
            using var connection = GetConnection(database);

            var affected = await connection.ExecuteAsync("DELETE FROM public.test WHERE id = @Id", new { Id = new Guid(id) });

            return affected != 0;
        }


        private static NpgsqlConnection GetConnection (DatabaseSource database)
        {
            string connectionString = $"Server={database.Url};Port={database.Port};Database={database.DatabaseName};User Id={database.Username};Password={database.Password}";

            return new NpgsqlConnection(connectionString);
        }
    }
}
