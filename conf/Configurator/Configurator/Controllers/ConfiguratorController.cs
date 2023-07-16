using Configurator.Entities;
using Configurator.Services;
using Microsoft.AspNetCore.Mvc;

namespace Configurator.Controllers
{
    [ApiController]
    [Route("conf")]
    public class ConfiguratorController : ControllerBase
    {
        private readonly ConfiguratorService _configuratorService;

        public ConfiguratorController(ConfiguratorService configuratorService)
        {
            _configuratorService = configuratorService ?? throw new ArgumentNullException(nameof(configuratorService));
        }
        [HttpGet("")]
        [ProducesResponseType(typeof(IEnumerable<Config>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Config>>> GetAllConfigs()
        {
            return Ok(await _configuratorService.GetAllConfigs());
        }

        [HttpGet("{category}")]
        [ProducesResponseType(typeof(IEnumerable<Config>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Config>>> GetConfigsByCategory(string category)
        {
            return Ok(await _configuratorService.GetConfigsByCategory(category));
        }

        [HttpPatch("update")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> UpdateConfigs()
        {
            return Ok(await _configuratorService.UpdateConfigs());
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ModifyAndUpdateConfigs([FromBody] IEnumerable<Config> configs)
        {
            return Ok(await _configuratorService.ModifyAndUpdate(configs));
        }

        [HttpDelete("")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> DeleteConfigs([FromBody] IEnumerable<ConfigId> configs)
        {
            return Ok(await _configuratorService.DeleteConfigs(configs));
        }
    }
}
