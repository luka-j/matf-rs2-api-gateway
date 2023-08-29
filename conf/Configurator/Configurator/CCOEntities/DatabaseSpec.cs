namespace Configurator.CCOEntities
{
    public class DatabaseSpec
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
