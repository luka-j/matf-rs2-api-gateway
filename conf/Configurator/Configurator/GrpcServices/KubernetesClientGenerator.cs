using k8s;
using k8s.Models;

namespace Configurator.GrpcServices
{
    public class KubernetesClientGenerator : IClientGenerator
    {
        private readonly Kubernetes _client;
        public KubernetesClientGenerator() {
            var config = KubernetesClientConfiguration.InClusterConfig();
            _client = new Kubernetes(config);

        }
        public IEnumerable<ApiGatewayApi.ConfigManagement.ConfigManagementClient> GetAPIClients()
        {
            throw new NotImplementedException();
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

        public IEnumerable<ApiGatewayRp.ConfigManagement.ConfigManagementClient> GetRPClients()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CCO.ConfigManagement.ConfigManagementClient> GetCCOClients()
        {
            throw new NotImplementedException();
        }
    }
}
