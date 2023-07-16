namespace Configurator.GrpcServices
{
    public interface IClientNameService
    {
        public IEnumerable<string> GetAPIClientNames();
        public IEnumerable<string> GetRPClientNames();
    }
}
