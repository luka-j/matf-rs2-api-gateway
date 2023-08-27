namespace CCO.Entities
{
    public class Spec
    {
        public string Title { get; set; }
        public Datasource Datasource { get; set; }

        public Spec(string title, Datasource datasource)
        {
            Title = title;
            Datasource = datasource;
        }
    }
}
