namespace Configurator.Entities
{
    public class Config
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }

        public Config(string category, string name, string data)
        {
            Category = category;
            Name = name;
            Data = data;
        }
    }
}
