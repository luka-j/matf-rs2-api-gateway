using Grpc.Net.Client;
using k8s;
using k8s.Models;

namespace Configurator.GrpcServices
{
    public class KubernetesClientGenerator : IClientGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly Kubernetes _client;
        private readonly string _namespaceName;
        public KubernetesClientGenerator(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var config = KubernetesClientConfiguration.InClusterConfig();
            _client = new Kubernetes(config);

            _namespaceName = _configuration["ClusterNamespace"];
        }

        public IEnumerable<ApiGatewayApi.ConfigManagement.ConfigManagementClient> GetAPIClients()
        {
            V1PodList podList = _client.ListNamespacedPod(_namespaceName, labelSelector: "app=api");

            List<ApiGatewayApi.ConfigManagement.ConfigManagementClient> clients = new();

            foreach (V1Pod pod in podList.Items)
            {
                var APIPort = pod.Spec.Containers.First().Ports.First().ContainerPort;
                var URI = $"http://{pod.Status.PodIP}:{APIPort}";

                var channel = GrpcChannel.ForAddress(URI);
                var client = new ApiGatewayApi.ConfigManagement.ConfigManagementClient(channel);

                clients.Add(client);
            }
            return clients;
        }

        public IEnumerable<ApiGatewayRp.ConfigManagement.ConfigManagementClient> GetRPClients()
        {
            V1PodList podList = _client.ListNamespacedPod(_namespaceName, labelSelector: "app=rp");

            List<ApiGatewayRp.ConfigManagement.ConfigManagementClient> clients = new();

            foreach (V1Pod pod in podList.Items)
            {
                var RPPort = pod.Spec.Containers.First().Ports.First().ContainerPort;
                var URI = $"http://{pod.Status.PodIP}:{RPPort}";

                var channel = GrpcChannel.ForAddress(URI);
                var client = new ApiGatewayRp.ConfigManagement.ConfigManagementClient(channel);

                clients.Add(client);
            }
            return clients;
        }

        public IEnumerable<CCO.ConfigManagement.ConfigManagementClient> GetCCOClients()
        {
            V1PodList podList = _client.ListNamespacedPod(_namespaceName, labelSelector: "app=cco");

            List<CCO.ConfigManagement.ConfigManagementClient> clients = new();

            foreach (V1Pod pod in podList.Items)
            {
                var CCOPort = pod.Spec.Containers.First().Ports.First().ContainerPort;
                var URI = $"http://{pod.Status.PodIP}:{CCOPort}";

                var channel = GrpcChannel.ForAddress(URI);
                var client = new CCO.ConfigManagement.ConfigManagementClient(channel);

                clients.Add(client);
            }
            return clients;
        }
    }
}
