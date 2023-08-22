using Grpc.Net.Client;

namespace Configurator.GrpcServices
{
    public class ClientGenerator : IClientGenerator
    {
        private readonly IConfiguration _configuration;

        public ClientGenerator(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IEnumerable<ApiGatewayApi.ConfigManagement.ConfigManagementClient> GetAPIClients()
        {
            var channel = GrpcChannel.ForAddress(_configuration["GrpcSettings:APIURL"]);
            var client = new ApiGatewayApi.ConfigManagement.ConfigManagementClient(channel);

            return new[] { client };
        }
        public IEnumerable<ApiGatewayRp.ConfigManagement.ConfigManagementClient> GetRPClients()
        {
            var channel = GrpcChannel.ForAddress(_configuration["GrpcSettings:RPURL"]);
            var client = new ApiGatewayRp.ConfigManagement.ConfigManagementClient(channel);

            return new[] { client };
        }
        public IEnumerable<CCO.ConfigManagement.ConfigManagementClient> GetCCOClients()
        {
            var channel = GrpcChannel.ForAddress(_configuration["GrpcSettings:CCOURL"]);
            var client = new CCO.ConfigManagement.ConfigManagementClient(channel);

            return new[] { client };
        }
    }
}
