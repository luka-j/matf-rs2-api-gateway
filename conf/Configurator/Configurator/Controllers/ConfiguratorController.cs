using Configurator.Entities;
using Configurator.Services;
using Microsoft.AspNetCore.Mvc;

namespace Configurator.Controllers
{
    [ApiController]
    public class ConfiguratorController : ControllerBase
    {
        private readonly ConfiguratorService _configuratorService;

        public ConfiguratorController(ConfiguratorService configuratorService)
        {
            _configuratorService = configuratorService ?? throw new ArgumentNullException(nameof(configuratorService));
        }

        [HttpPost("update")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> UpdateConfigs()
        {
            return Ok(await _configuratorService.UpdateConfigs());
        }

        [HttpPost("modify")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ModifyAndUpdateConfigs([FromBody] IEnumerable<Config> configs)
        {
            return Ok(await _configuratorService.ModifyAndUpdate(configs));
        }

        [HttpDelete("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> DeleteConfigs([FromBody] IEnumerable<ConfigId> configs)
        {
            return Ok(await _configuratorService.DeleteConfigs(configs));
        }
    }
}
