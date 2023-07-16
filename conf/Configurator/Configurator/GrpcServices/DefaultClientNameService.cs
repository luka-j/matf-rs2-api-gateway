namespace Configurator.GrpcServices
{
    public class DefaultClientNameService : IClientNameService
    {
        public IEnumerable<string> GetAPIClientNames()
        {
            return new[] { "API" };
        }

        public IEnumerable<string> GetRPClientNames()
        {
            return new[] { "RP" };
        }
    }
}
