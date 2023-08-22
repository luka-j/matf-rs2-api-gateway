using CCO;
using Configurator.DTOs;
using Configurator.Entities;
using Configurator.GrpcServices;
using Configurator.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Configurator.Controllers
{
    [ApiController]
    [Route("cco")]
    public class CCOConfigController : ControllerBase
    {
        private readonly CCOGrpcService _ccoService;
        private readonly IConfigRepository _configRepository;

        public CCOConfigController(CCOGrpcService ccoService, IConfigRepository configRepository)
        {
            _ccoService = ccoService ?? throw new ArgumentNullException(nameof(ccoService));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ConfigMetadataDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ConfigMetadataDTO>>> GetConfigs()
        {
            return Ok(await _ccoService.GetAll());
        }

        [HttpGet("{apiName}/{apiVersion}")]
        [ProducesResponseType(typeof(ConfigDataDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ConfigDataDTO>> GetConfig(string apiName, string apiVersion)
        {
            try
            {
                return Ok(await _ccoService.Get(apiName, apiVersion));
            }
            catch { return NotFound(); }
        }

        [HttpPost]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> Add([FromBody] string data)
        {
            try
            {
                await _ccoService.Update(data, DateTime.Now.AddSeconds(10).ToString());
                return Ok(true);
            }
            catch { return Ok(false); }
        }

        [HttpDelete("{apiName}/{apiVersion}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> DeleteConfig(string apiName, string apiVersion)
        {
            try
            {
                var config = new Config("frontends", apiName, apiVersion, "");
                await _configRepository.DeleteConfigs(new[] { config });
                await _ccoService.Delete(apiName, apiVersion);
                return Ok(true);
            }
            catch { return Ok(false); }
        }
    }
}
