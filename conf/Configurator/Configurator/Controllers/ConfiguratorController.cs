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
    }
}
