using CCO;
using Configurator.DTOs;
using Configurator.Entities;
using Configurator.GrpcServices;
using Configurator.Repositories;
using Configurator.Services;
using Microsoft.AspNetCore.Mvc;

namespace Configurator.Controllers
{
    [ApiController]
    [Route("cco")]
    public class CCOConfigController : ControllerBase
    {
        //todo 
        //post request should change repo 

        private readonly CCOService _ccoService;
        private readonly IConfigRepository _configRepository;

        public CCOConfigController(CCOService ccoService, IConfigRepository configRepository)
        {
            _ccoService = ccoService ?? throw new ArgumentNullException(nameof(ccoService));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
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
