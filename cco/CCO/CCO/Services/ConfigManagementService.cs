using System.Globalization;
using CCO.CCOConfigs;
using Grpc.Core;

namespace CCO.Services
{
    public class ConfigManagementService : CCOConfigManagement.CCOConfigManagementBase
    {
        private readonly CCORepository _repository;

        public ConfigManagementService(CCORepository repository)
        {
            _repository = repository;
        }

        public override Task<ConfigData> GetConfig(ConfigId request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                try
                {
                    var configData = _repository.GetCurrentConfig(
                        new CCOIdentifier(request.CcoName, request.CcoVersion), DateTime.Now);
                    return new ConfigData
                    {
                        Data = configData?.GetDataString(),
                        ValidFrom = configData?.ValidFrom.ToString("O")
                    };
                }
                catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "No active Config with such id exists"));
                }
            });
        }

        public override Task<Empty> DeleteConfig(ConfigId request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var status = _repository.DeleteConfig(new CCOIdentifier(request.CcoName, request.CcoVersion));
                if (!status)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Failed to delete Config"));
                }
                return new Empty();
            });
        }

        public override Task<Empty> UpdateConfig(ConfigData request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                try
                {
                    _repository.AddConfig(new CCOSpec(request.Data, DateTime.Parse(request.ValidFrom, null, DateTimeStyles.RoundtripKind)));
                    return new Empty();
                }
                catch (Exception ex)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
                }
            });
        }

        public override Task<RevertChangesResponse> RevertPendingUpdates(Empty request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                DateTime now = DateTime.Now;
                int reverted = _repository.RevertPendingChanges(now);
                return new RevertChangesResponse
                {
                    Reverted = reverted,
                };
            });
        }


        public override Task<ConfigList> GetAllConfigs(Empty request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var configIds = _repository.GetAllConfigs(DateTime.Now)
                    .Select(id => new ConfigId
                    {
                        CcoName = id.Name,
                        CcoVersion = id.Version,
                    });
                var ret = new ConfigList();
                ret.Configs.AddRange(configIds);
                return ret;
            });
        }

    }
}