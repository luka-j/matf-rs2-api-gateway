using CCO;
using Grpc.Core;

namespace GrpcServer.Services
{
    public class DatasourceService : Datasource.DatasourceBase
    {
        private readonly List<GetDatasourceResponse> _datasources = new List<GetDatasourceResponse>();

        public override Task<GetDatasourceResponse> GetDatasource(GetDatasourceRequest request, ServerCallContext context)
        {
            var datasource = _datasources.FirstOrDefault(d => d.Name == request.Name);
            return Task.FromResult(datasource ?? new GetDatasourceResponse());
        }

        public override Task<GetDatasourcesResponse> GetDatasources(GetDatasourcesRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GetDatasourcesResponse { Datasources = { _datasources } });
        }

        public override Task<AddDatasourceResponse> AddDatasource(AddDatasourceRequest request, ServerCallContext context)
        {
            _datasources.Add(new GetDatasourceResponse
            {
                Name = request.Name,
                Type = request.Type,
                Url = request.Url,
                Username = request.Username,
                Password = request.Password
            });
            return Task.FromResult(new AddDatasourceResponse { Message = "Datasource added successfully." });
        }

        public override Task<UpdateDatasourceResponse> UpdateDatasource(UpdateDatasourceRequest request, ServerCallContext context)
        {
            var datasource = _datasources.FirstOrDefault(d => d.Name == request.Name);
            if (datasource != null)
            {
                datasource.Type = request.Type;
                datasource.Url = request.Url;
                datasource.Username = request.Username;
                datasource.Password = request.Password;
                return Task.FromResult(new UpdateDatasourceResponse { Message = "Datasource updated successfully." });
            }
            return Task.FromResult(new UpdateDatasourceResponse { Message = "Datasource not found." });
        }

        public override Task<DeleteDatasourceResponse> DeleteDatasource(DeleteDatasourceRequest request, ServerCallContext context)
        {
            var datasource = _datasources.FirstOrDefault(d => d.Name == request.Name);
            if (datasource != null)
            {
                _datasources.Remove(datasource);
                return Task.FromResult(new DeleteDatasourceResponse { Message = "Datasource deleted successfully." });
            }
            return Task.FromResult(new DeleteDatasourceResponse { Message = "Datasource not found." });
        }
    }
}