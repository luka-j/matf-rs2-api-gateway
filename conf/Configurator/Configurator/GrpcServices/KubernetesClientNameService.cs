namespace Configurator.GrpcServices
{
    public class KubernetesClientNameService : IClientNameService
    {
        // return the names from kubernetes

        public IEnumerable<string> GetAPIClientNames()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetRPClientNames()
        {
            throw new NotImplementedException();
        }
    }
}
