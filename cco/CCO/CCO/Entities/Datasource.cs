namespace CCO.Entities
{
    public class Datasource
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConnectionString { get; set; }

        public Datasource(string name, string type, string url, string username, string password, string connectionString)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }
    }
}
