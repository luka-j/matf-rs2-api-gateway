namespace Configurator.Entities
{
    public class CCOSpec
    {
        public string Title { get; set; }
        public CCODatasource Datasource { get; set; }

        public CCOSpec(string title, CCODatasource datasource)
        {
            Title = title;
            Datasource = datasource;
        }
    }
}
