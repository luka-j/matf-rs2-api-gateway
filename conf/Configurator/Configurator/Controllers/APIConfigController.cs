using ApiGatewayApi;
using Configurator.GrpcServices;
using Microsoft.AspNetCore.Mvc;

namespace Configurator.Controllers
{
    [ApiController]
    [Route("api")]
    public class APIConfigController : ControllerBase
    {
        private readonly APIGrpcService _apiService;

        public APIConfigController(APIGrpcService apiService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        }

        [HttpGet("frontend")]
        [ProducesResponseType(typeof(ConfigList), StatusCodes.Status200OK)]
        public async Task<ActionResult<ConfigList>> GetFrontendConfigs() 
        {
            return Ok(await _apiService.GetAllFrontend());
        }

        [HttpGet("frontend/{apiName}/{apiVersion}")]
        [ProducesResponseType(typeof(ConfigList), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ConfigData>> GetFrontendConfig(string apiName, string apiVersion)
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
                await _apiService.DeleteFrontend(apiName, apiVersion);
                return Ok(true);
            }
            catch { return Ok(false); }
        }

        [HttpGet("backend")]
        [ProducesResponseType(typeof(ConfigList), StatusCodes.Status200OK)]
        public async Task<ActionResult<ConfigList>> GetBackendConfigs()
        {
            return Ok(await _apiService.GetAllBackend());
        }

        [HttpGet("backend/{apiName}/{apiVersion}")]
        [ProducesResponseType(typeof(ConfigList), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ConfigData>> GetBackendConfig(string apiName, string apiVersion)
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
                await _apiService.DeleteBackend(apiName, apiVersion);
                return Ok(true);
            }
            catch { return Ok(false); }
        }
    }
}
