namespace CCO.Entities
{
    public class Spec<TSource>
    {
        public string Title { get; set; }
        public TSource Datasource { get; set; }
        public Spec(string title, TSource datasource)
        {
            Title = title;
            Datasource = datasource;
        }
    }
}
