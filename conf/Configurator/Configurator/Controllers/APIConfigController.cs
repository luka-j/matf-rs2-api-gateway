using Configurator.DTOs;
using Configurator.Entities;
using Configurator.GrpcServices;
using Configurator.Services;
using Microsoft.AspNetCore.Mvc;

namespace Configurator.Controllers
{
    [ApiController]
    [Route("api")]
    public class APIConfigController : ControllerBase
    {
        private readonly APIGrpcService _apiService;
        private readonly ConfiguratorService _configuratorService;

        public APIConfigController(APIGrpcService apiService, ConfiguratorService configuratorService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _configuratorService = configuratorService ?? throw new ArgumentNullException(nameof(configuratorService));
        }

        [HttpGet("frontend")]
        [ProducesResponseType(typeof(IEnumerable<ConfigMetadataDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ConfigMetadataDTO>>> GetFrontendConfigs() 
        {
            var configs = await _apiService.GetAllFrontend();
            return Ok(configs.Configs);
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
        public async Task<ActionResult<bool>> AddFrontend([FromBody] ConfigPostDTO data)
        {
            try
            {
                Config config = new("frontends", data.ApiName, data.ApiVersion, data.Data);
                await _configuratorService.ModifyAndUpdate(new[] { config });
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
                await _configuratorService.DeleteConfigs(new[] { config });
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
        public async Task<ActionResult<bool>> AddBackend([FromBody] ConfigPostDTO data)
        {
            try
            {
                Config config = new("backends", data.ApiName, data.ApiVersion, data.Data);
                await _configuratorService.ModifyAndUpdate(new[] { config });
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
                await _configuratorService.DeleteConfigs(new[] { config });
                return Ok(true);
            }
            catch { return Ok(false); }
        }
    }
}
