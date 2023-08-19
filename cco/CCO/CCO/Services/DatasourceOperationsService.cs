using Grpc.Core;

namespace CCO.Services
{
    public class DatasourceOperationsService : DatasourceOperations.DatasourceOperationsBase
    {
         public override Task<DatabaseReadResponse> DatabaseRead (DatabaseReadRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

        public override Task<DatabaseWriteResponse> DatabaseWrite (DatabaseWriteRequest request, ServerCallContext context) 
        { 
            throw new NotImplementedException(); 
        }

        public override Task<DatabaseDeleteResponse> DatabaseDelete(DatabaseDeleteRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
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
    }
}
