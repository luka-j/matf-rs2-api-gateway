namespace CCO.Entities
{
    public class DatabaseSpec : ISpec
    {
        public string Title { get; set; }
        public DatabaseSource Database { get; set; }

        public DatabaseSpec(string title, DatabaseSource database)
        {
            Title = title;
            Database = database;
        }
    }
}
