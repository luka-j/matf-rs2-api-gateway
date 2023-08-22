namespace Configurator.GrpcServices
{
    public class ClientGenerator : IClientGenerator
    {
        public IEnumerable<ApiGatewayApi.ConfigManagement.ConfigManagementClient> GetAPIClients()
        {
            throw new NotImplementedException();
            // return new[] { "API" };
        }
        public IEnumerable<ApiGatewayRp.ConfigManagement.ConfigManagementClient> GetRPClients()
        {
            throw new NotImplementedException();
            // return new[] { "RP" };
        }
        public IEnumerable<CCO.ConfigManagement.ConfigManagementClient> GetCCOClients()
        {
            throw new NotImplementedException();
        }
    }
}
