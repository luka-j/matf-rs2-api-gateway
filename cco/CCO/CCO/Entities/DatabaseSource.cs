namespace CCO.Entities
{
    public class DatabaseSource : IDatasource
    {
        public string Type { get; set; }
        public string Url { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string DatabaseName { get; set; }
        public string Username { get; set; }

        public DatabaseSource(string type, string url, string password, int port, string databaseName, string username)
        {
            Type = type;
            Url = url;
            Password = password;
            Port = port;
            DatabaseName = databaseName;
            Username = username;
        }
    }
}
