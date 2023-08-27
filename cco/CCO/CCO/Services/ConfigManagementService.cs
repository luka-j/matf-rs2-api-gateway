using System.Globalization;
using CCO.CCOConfigs;
using Grpc.Core;

namespace CCO.Services
{
    public class ConfigManagementService : ConfigManagement.ConfigManagementBase
    {
        private readonly ConfigRepository _repository;

        public ConfigManagementService(ConfigRepository repository)
        {
            _repository = repository;
        }

        public override Task<ConfigData> GetDatabaseConfig(ConfigId request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                try
                {
                    var configData = _repository.Databases.GetCurrentConfig(
                        new CCOConfigIdentifier(request.Name), DateTime.Now);
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
    
        public override Task<Empty> DeleteDatabaseConfig(ConfigId request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var status = _repository.Databases.DeleteConfig(new CCOConfigIdentifier(request.Name));
                if (!status)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Failed to delete Config"));
                }
                return new Empty();
            });
        }

        public override Task<Empty> UpdateDatabaseConfig(ConfigData request,  ServerCallContext context)
        {
            return Task.Run(() =>
            {
                try
                {
                    _repository.Databases.AddConfig(new ConfigSpec(request.Data, DateTime.Parse(request.ValidFrom, null, DateTimeStyles.RoundtripKind)));
                    return new Empty();
                }
                catch (Exception ex)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
                }
            });
        }

        public override Task<ConfigList> GetAllDatabaseConfigs(Empty request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var configIds = _repository.Databases.GetAllConfigs(DateTime.Now)
                    .Select(id => new ConfigId
                    {
                        Name = id.Name,
                    });
                var ret = new ConfigList();
                ret.Configs.AddRange(configIds);
                return ret;
            });
        }

        public override Task<ConfigData> GetCacheConfig(ConfigId request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                try
                {
                    var configData = _repository.Caches.GetCurrentConfig(
                        new CCOConfigIdentifier(request.Name), DateTime.Now);
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

        public override Task<Empty> DeleteCacheConfig(ConfigId request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var status = _repository.Caches.DeleteConfig(new CCOConfigIdentifier(request.Name));
                if (!status)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Failed to delete Config"));
                }
                return new Empty();
            });
        }

        public override Task<Empty> UpdateCacheConfig(ConfigData request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                try
                {
                    _repository.Caches.AddConfig(new ConfigSpec(request.Data, DateTime.Parse(request.ValidFrom, null, DateTimeStyles.RoundtripKind)));
                    return new Empty();
                }
                catch (Exception ex)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
                }
            });
        }

        public override Task<ConfigList> GetAllCacheConfigs(Empty request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var configIds = _repository.Caches.GetAllConfigs(DateTime.Now)
                    .Select(id => new ConfigId
                    {
                        Name = id.Name,
                    });
                var ret = new ConfigList();
                ret.Configs.AddRange(configIds);
                return ret;
            });
        }

        public override Task<ConfigData> GetQueueConfig(ConfigId request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                try
                {
                    var configData = _repository.Queues.GetCurrentConfig(
                        new CCOConfigIdentifier(request.Name), DateTime.Now);
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

        public override Task<Empty> DeleteQueueConfig(ConfigId request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var status = _repository.Queues.DeleteConfig(new CCOConfigIdentifier(request.Name));
                if (!status)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Failed to delete Config"));
                }
                return new Empty();
            });
        }

        public override Task<Empty> UpdateQueueConfig(ConfigData request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                try
                {
                    _repository.Queues.AddConfig(new ConfigSpec(request.Data, DateTime.Parse(request.ValidFrom, null, DateTimeStyles.RoundtripKind)));
                    return new Empty();
                }
                catch (Exception ex)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
                }
            });
        }

        public override Task<ConfigList> GetAllQueueConfigs(Empty request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var configIds = _repository.Queues.GetAllConfigs(DateTime.Now)
                    .Select(id => new ConfigId
                    {
                        Name = id.Name,
                    });
                var ret = new ConfigList();
                ret.Configs.AddRange(configIds);
                return ret;
            });
        }


        public override Task<RevertChangesResponse> RevertPendingUpdates(Empty request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                DateTime now = DateTime.Now;
                int revertedDatabases = _repository.Databases.RevertPendingChanges(now);
                int revertedCaches = _repository.Caches.RevertPendingChanges(now);
                int revertedQueues = _repository.Queues.RevertPendingChanges(now);

                return new RevertChangesResponse
                {
                    RevertedDatabases = revertedDatabases,
                    RevertedCaches = revertedCaches,
                    RevertedQueues = revertedQueues,
                };
            });
        }

    }
}