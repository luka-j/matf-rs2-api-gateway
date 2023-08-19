using CCO.CCOConfigs;
using CCO.Repositories;
using Grpc.Core;

namespace CCO.Services
{
    public class DatasourceOperationsService : DatasourceOperations.DatasourceOperationsBase
    {
        private readonly CCORepository _repository;
        private readonly DatabaseRepository _databaseRepository;

        public DatasourceOperationsService(CCORepository repository, DatabaseRepository databaseRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _databaseRepository = databaseRepository ?? throw new ArgumentNullException(nameof(databaseRepository));
        }

        public override async Task<DatabaseReadResponse> DatabaseRead (DatabaseReadRequest request, ServerCallContext context)
        {
            var config = getConfig(request.Identifier);
            var database = config.Data.Databases.FirstOrDefault() ?? throw new Exception("Database not found");

            var entries = await _databaseRepository.ReadAll(database);
            var response = new DatabaseReadResponse();
            response.Items.AddRange(entries.Select((entry) => new DatabaseItem { Id = entry.Id, Amount = entry.Amount }));

            return response;
        }

        public override async Task<DatabaseWriteResponse> DatabaseWrite (DatabaseWriteRequest request, ServerCallContext context) 
        {
            var config = getConfig(request.Identifier);
            var database = config.Data.Databases.FirstOrDefault() ?? throw new Exception("Database not found");

            var success = await _databaseRepository.CreateEntry(database, request.Amount);

            return new DatabaseWriteResponse { Success = success };
        }

        public override async Task<DatabaseDeleteResponse> DatabaseDelete(DatabaseDeleteRequest request, ServerCallContext context)
        {
            var config = getConfig(request.Identifier);
            var database = config.Data.Databases.FirstOrDefault() ?? throw new Exception("Database not found");

            var success = await _databaseRepository.DeleteEntry(database, request.Id);

            return new DatabaseDeleteResponse { Success = success };
        }

        public override Task<CacheReadResponse> CacheRead (CacheReadRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

        public override Task<CacheWriteResponse> CacheWrite (CacheWriteRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

        public override Task<QueueReadResponse> QueueRead (QueueReadRequest request, ServerCallContext context) 
        {
            throw new NotImplementedException();
        }

        public override Task<QueueWriteResponse> QueueWrite (QueueWriteRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

        private CCOConfig getConfig (ConfigIdentifier id)
        {
            CCOIdentifier identifier = new CCOIdentifier(id.CcoName, id.CcoVersion);
            var config = _repository.GetCurrentConfig(identifier, DateTime.Now);

            if (config == null)
            {
                throw new Exception("CCO " + id.CcoName + "/" + id.CcoVersion + " not found");
            }

            return config;
        }
    }
}
