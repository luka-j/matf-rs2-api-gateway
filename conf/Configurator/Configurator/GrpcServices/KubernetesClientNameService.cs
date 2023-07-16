using k8s;

namespace Configurator.GrpcServices
{
    public class KubernetesClientNameService : IClientNameService
    {
        private readonly Kubernetes _client;
        public KubernetesClientNameService() {
            var config = KubernetesClientConfiguration.InClusterConfig();
            _client = new Kubernetes(config);

        }

        public IEnumerable<string> GetAPIClientNames()
        {
            List<string> names = new();
            var APIPods = _client.ListNamespacedPod("api-gateway");
            foreach(var pod in APIPods)
            {
                names.Add(pod.Metadata.Name);
            }
            return names;
        }

        public IEnumerable<string> GetRPClientNames()
        {
            throw new NotImplementedException();
        }
    }
}
