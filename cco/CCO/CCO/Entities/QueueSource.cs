namespace CCO.Entities
{
    public class QueueSource
    {
        public string Type { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public QueueSource(string type, string url, string username, string password)
        {
            Type = type;
            Url = url;
            Username = username;
            Password = password;
        }
    }
}
