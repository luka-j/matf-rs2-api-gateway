using ApiGatewayRp;
using Configurator.DTOs;
using Configurator.Entities;
using Configurator.GrpcServices;
using Configurator.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Configurator.Controllers
{
    [ApiController]
    [Route("rp")]
    public class RPConfigController : ControllerBase
    {
        private readonly RPGrpcService _rpService;
        private readonly IConfigRepository _configRepository;

        public RPConfigController(RPGrpcService rpService, IConfigRepository configRepository)
        {
            _rpService = rpService ?? throw new ArgumentNullException(nameof(rpService));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ConfigMetadataDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ConfigMetadataDTO>>> GetConfigs()
        {
            return Ok(await _rpService.GetAll());
        }

        [HttpGet("{apiName}/{apiVersion}")]
        [ProducesResponseType(typeof(ConfigDataDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ConfigDataDTO>> GetConfig(string apiName, string apiVersion)
        {
            try
            {
                return Ok(await _rpService.Get(apiName, apiVersion));
            }
            catch { return NotFound(); }
        }

        [HttpPost]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> Add([FromBody] string data)
        {
            try
            {
                await _rpService.Update(data, DateTime.Now.AddSeconds(10).ToString());
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
                await _rpService.Delete(apiName, apiVersion);
                return Ok(true);
            }
            catch { return Ok(false); }
        }
    }
}
