using System;
using AppConfigMaintenance.DataAccess.Entities;
using AppConfigMaintenance.DataAccess.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppConfigMaintenance.DataAccess.Models;
using AppConfigMaintenance.DataAccess.Services;

namespace AppConfigMaintenance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly IConfigSettingService _configSettingService;
        private readonly IConfigSettingRepository _configSettingRepository;

        public SettingsController(IConfigSettingService configSettingService, IConfigSettingRepository configSettingRepository)
        {
            _configSettingService = configSettingService;
            _configSettingRepository = configSettingRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<ConfigSettingModel>> Get()
        {
            // TODO: If there was a really large number of these, would need to implement a required filter, or paging
            return _configSettingRepository.GetConfigSettings().ToList();
        }

        [HttpGet("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ConfigSettingModel> Get(string name)
        {
            return _configSettingRepository.GetConfigSettings().SingleOrDefault(x => x.Name == name);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ConfigSettingModel>> CreateAsync(ConfigSettingModel configSetting)
        {
            try
            {
                await _configSettingService.AddConfigSettingAsync(configSetting);

                return CreatedAtAction(nameof(Get), new { name = configSetting.Name }, configSetting);
            }
            catch (Exception e)
            {
                //TODO Decide if we want to return exception message or a generic message
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Put(string name, [FromBody] string value)
        {
            try
            {
                await _configSettingService.UpdateConfigSettingByNameAsync(name, value);
                return Ok();
            }
            catch (Exception e)
            {
                //TODO Decide if we want to return exception message or a generic message
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete(string name)
        {
            try
            {
                await _configSettingRepository.DeleteConfigSettingByNameAsync(name);
                return Ok();
            }
            catch (Exception e)
            {
                //TODO Decide if we want to return exception message or a generic message
                return BadRequest(e.Message);
            }
        }
    }
}