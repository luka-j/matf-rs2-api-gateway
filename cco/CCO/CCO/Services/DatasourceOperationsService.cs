using CCO.CCOConfigs;
using CCO.Repositories;
using Grpc.Core;

namespace CCO.Services
{
    public class DatasourceOperationsService : DatasourceOperations.DatasourceOperationsBase
    {
        private readonly ConfigRepository _repository;
        private readonly DatabaseRepository _databaseRepository;
        private readonly CacheRepository _cacheRepository;
        private readonly QueueRepository _queueRepository;

        public DatasourceOperationsService(ConfigRepository repository, DatabaseRepository databaseRepository, CacheRepository cacheRepository, QueueRepository queueRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _databaseRepository = databaseRepository ?? throw new ArgumentNullException(nameof(databaseRepository));
            _cacheRepository = cacheRepository ?? throw new ArgumentNullException(nameof(cacheRepository));
            _queueRepository = queueRepository ?? throw new ArgumentNullException(nameof(queueRepository));
        }

        public override async Task<DatabaseReadResponse> DatabaseRead (DatabaseReadRequest request, ServerCallContext context)
        {
            var config = getDatabaseConfig(request.Identifier);
            var database = config.Data.Datasource ?? throw new Exception("Database not found");

            var entries = await _databaseRepository.ReadAll(database);
            var response = new DatabaseReadResponse();
            response.Items.AddRange(entries.Select((entry) => new DatabaseItem { Id = entry.Id.ToString(), Amount = entry.Amount }));

            return response;
        }

        public override async Task<DatabaseWriteResponse> DatabaseWrite (DatabaseWriteRequest request, ServerCallContext context) 
        {
            var config = getDatabaseConfig(request.Identifier);
            var database = config.Data.Datasource ?? throw new Exception("Database not found");

            var success = await _databaseRepository.Create(database, request.Amount);

            return new DatabaseWriteResponse { Success = success };
        }

        public override async Task<DatabaseDeleteResponse> DatabaseDelete(DatabaseDeleteRequest request, ServerCallContext context)
        {
            var config = getDatabaseConfig(request.Identifier);
            var database = config.Data.Datasource ?? throw new Exception("Database not found");

            var success = await _databaseRepository.Delete(database, request.Id);

            return new DatabaseDeleteResponse { Success = success };
        }

        public override async Task<CacheReadResponse> CacheRead (CacheReadRequest request, ServerCallContext context)
        {
            var config = getCacheConfig(request.Identifier);
            var cache = config.Data.Datasource ?? throw new Exception("Database not found");

            string value = await _cacheRepository.GetAsync(cache, request.Key);
            
            return new CacheReadResponse { Value = value };
        }

        public override async Task<CacheWriteResponse> CacheWrite (CacheWriteRequest request, ServerCallContext context)
        {
            var config = getCacheConfig(request.Identifier);
            var cache = config.Data.Datasource ?? throw new Exception("Database not found");

            var success = await _cacheRepository.SetAsync(cache, request.Key, request.Value, TimeSpan.Parse(request.Ttl));

            return new CacheWriteResponse { Success = success };
        }

        public override Task<QueueWriteResponse> QueueWrite (QueueWriteRequest request, ServerCallContext context)
        {
            var config = getQueueConfig(request.Identifier);
            var queue = config.Data.Datasource ?? throw new Exception("Database not found");

            _queueRepository.Publish(queue, request.QueueName, request.Message);

            return Task.FromResult(new QueueWriteResponse());
        }

        private CCOConfig getDatabaseConfig(ConfigIdentifier id)
        {
            CCOConfigIdentifier identifier = new CCOConfigIdentifier(id.Name);
            var config = _repository.Databases.GetCurrentConfig(identifier, DateTime.Now);

            return config == null ? throw new Exception("Database config " + id.Name + " not found") : config;
        }

        private CCOConfig getCacheConfig(ConfigIdentifier id)
        {
            CCOConfigIdentifier identifier = new CCOConfigIdentifier(id.Name);
            var config = _repository.Caches.GetCurrentConfig(identifier, DateTime.Now);

            return config == null ? throw new Exception("Database config " + id.Name + " not found") : config;
        }

        private CCOConfig getQueueConfig(ConfigIdentifier id)
        {
            CCOConfigIdentifier identifier = new CCOConfigIdentifier(id.Name);
            var config = _repository.Queues.GetCurrentConfig(identifier, DateTime.Now);

            return config == null ? throw new Exception("Database config " + id.Name + " not found") : config;
        }

    }
}
