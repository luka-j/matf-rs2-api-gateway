namespace Configurator.CCOEntities
{
    public class QueueSpec
    {
        public string Title { get; set; }
        public QueueSource Queue { get; set; }

        public QueueSpec(string title, QueueSource queue)
        {
            Title = title;
            Queue = queue;
        }
    }
}
