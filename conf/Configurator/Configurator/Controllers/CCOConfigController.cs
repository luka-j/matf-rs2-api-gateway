using Configurator.Entities;
using Configurator.Services;
using Microsoft.AspNetCore.Mvc;

namespace Configurator.Controllers
{
    [ApiController]
    [Route("cco")]
    public class CCOConfigController : ControllerBase
    {
        private readonly CCOService _ccoService;
        private readonly ConfiguratorService _configuratorService;

        public CCOConfigController(CCOService ccoService, ConfiguratorService configuratorService)
        {
            _ccoService = ccoService ?? throw new ArgumentNullException(nameof(ccoService));
            _configuratorService = configuratorService ?? throw new ArgumentNullException(nameof(configuratorService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CCOSpec>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CCOSpec>>> GetConfigs()
        {
            return Ok(await _ccoService.GetAll());
        }

        [HttpGet("{apiName}/{apiVersion}")]
        [ProducesResponseType(typeof(CCOSpec), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CCOSpec>> GetConfig(string apiName, string apiVersion)
        {
            try
            {
                return Ok(await _ccoService.Get(apiName, apiVersion));
            }
            catch { return NotFound(); }
        }

        [HttpPost]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> Add([FromBody] CCOSpec data)
        {
            try
            {
                string configData = CCOService.GetDataString(data);
                Config config = new("datasources", data.Title, data.Version, configData);

                await _configuratorService.ModifyAndUpdate(new[] { config });
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
                var config = new Config("datasources", apiName, apiVersion, "");
                await _configuratorService.DeleteConfigs(new[] { config });
                return Ok(true);
            }
            catch { return Ok(false); }
        }
    }
}
