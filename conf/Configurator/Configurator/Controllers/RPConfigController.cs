﻿using Configurator.DTOs;
using Configurator.Entities;
using Configurator.GrpcServices;
using Configurator.Services;
using Microsoft.AspNetCore.Mvc;

namespace Configurator.Controllers
{
    [ApiController]
    [Route("rp")]
    public class RPConfigController : ControllerBase
    {
        private readonly RPGrpcService _rpService;
        private readonly ConfiguratorService _configuratorService;

        public RPConfigController(RPGrpcService rpService, ConfiguratorService configuratorService)
        {
            _rpService = rpService ?? throw new ArgumentNullException(nameof(rpService));
            _configuratorService = configuratorService ?? throw new ArgumentNullException(nameof(configuratorService));
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
        public async Task<ActionResult<bool>> Add([FromBody] ConfigPostDTO data)
        {
            try
            {
                Config config = new("middlewares", data.ApiName, data.ApiVersion, data.Data);
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
                var config = new Config("middlewares", apiName, apiVersion, "");
                await _configuratorService.DeleteConfigs(new[] { config });
                return Ok(true);
            }
            catch { return Ok(false); }
        }
    }
}
