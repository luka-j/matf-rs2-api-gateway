namespace Configurator.CCOEntities
{
    public class CCOSpec<TSource>
    {
        public string Title { get; set; }
        public TSource Datasource { get; set; }

        public CCOSpec(string title, TSource datasource)
        {
            Title = title;
            Datasource = datasource;
        }
    }
}
