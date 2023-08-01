using k8s;
using k8s.Models;
using Zitadel.Api;

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
            string namespaceName = "api-gateway";
            string serviceName = "api-service";

            V1Service service = _client.ReadNamespacedService(serviceName, namespaceName);

            var selectorLabels = service.Spec.Selector;
            V1PodList podList = _client.ListNamespacedPod(namespaceName, labelSelector: string.Join(",", selectorLabels));

            foreach (var pod in podList.Items)
            {
                if (pod.Metadata.Labels["app"] == "api")
                {
                    names.Add(pod.Metadata.Name);
                }
            }
            return names;
        }

        public IEnumerable<string> GetRPClientNames()
        {
            throw new NotImplementedException();
        }
    }
}
