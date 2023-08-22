
namespace Configurator.GrpcServices
{
    public interface IClientGenerator
    {
        public IEnumerable<ApiGatewayApi.ConfigManagement.ConfigManagementClient> GetAPIClients();
        public IEnumerable<ApiGatewayRp.ConfigManagement.ConfigManagementClient> GetRPClients();
        public IEnumerable<CCO.ConfigManagement.ConfigManagementClient> GetCCOClients();
    }
}
