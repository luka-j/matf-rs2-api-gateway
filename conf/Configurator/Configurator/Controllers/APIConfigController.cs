using ApiGatewayApi;
using Configurator.DTOs;
using Configurator.Entities;
using Configurator.GrpcServices;
using Configurator.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Configurator.Controllers
{
    [ApiController]
    [Route("api")]
    public class APIConfigController : ControllerBase
    {
        private readonly APIGrpcService _apiService;
        private readonly IConfigRepository _configRepository;

        public APIConfigController(APIGrpcService apiService, IConfigRepository configRepository)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
        }

        [HttpGet("frontend")]
        [ProducesResponseType(typeof(IEnumerable<ConfigMetadataDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ConfigMetadataDTO>>> GetFrontendConfigs() 
        {
            return Ok(await _apiService.GetAllFrontend());
        }

        [HttpGet("frontend/{apiName}/{apiVersion}")]
        [ProducesResponseType(typeof(ConfigDataDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ConfigDataDTO>> GetFrontendConfig(string apiName, string apiVersion)
        {
            try
            {
                return Ok(await _apiService.GetFrontend(apiName, apiVersion));

            } catch{ return NotFound(); }
        }

        [HttpPost("frontend")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> AddFrontend([FromBody] string data)
        {
            try
            {
                await _apiService.UpdateFrontend(data, DateTime.Now.AddSeconds(10).ToString());
                return Ok(true);
            } catch { return Ok(false); } 
        }

        [HttpDelete("frontend/{apiName}/{apiVersion}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> DeleteFrontendConfig(string apiName, string apiVersion)
        {
            try
            {
                var config = new Config("frontends", apiName, apiVersion, "");
                await _configRepository.DeleteConfigs(new[] { config });
                await _apiService.DeleteFrontend(apiName, apiVersion);
                return Ok(true);
            }
            catch { return Ok(false); }
        }

        [HttpGet("backend")]
        [ProducesResponseType(typeof(IEnumerable<ConfigMetadataDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ConfigMetadataDTO>>> GetBackendConfigs()
        {
            return Ok(await _apiService.GetAllBackend());
        }

        [HttpGet("backend/{apiName}/{apiVersion}")]
        [ProducesResponseType(typeof(ConfigDataDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ConfigDataDTO>> GetBackendConfig(string apiName, string apiVersion)
        {
            try
            {
                return Ok(await _apiService.GetBackend(apiName, apiVersion));
            } catch { return NotFound(); }
        }

        [HttpPost("backend")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> AddBackend([FromBody] string data)
        {
            try
            {
                await _apiService.UpdateBackend(data, DateTime.Now.AddSeconds(10).ToString());
                return Ok(true);
            }
            catch { return Ok(false); }
        }

        [HttpDelete("backend/{apiName}/{apiVersion}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> DeleteBackendConfig (string apiName, string apiVersion)
        {
            try
            {
                var config = new Config("backends", apiName, apiVersion, "");
                await _configRepository.DeleteConfigs(new[] { config });
                await _apiService.DeleteBackend(apiName, apiVersion);
                return Ok(true);
            }
            catch { return Ok(false); }
        }
    }
}
