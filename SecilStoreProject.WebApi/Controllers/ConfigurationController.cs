﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecilStoreProject.Business;
using SecilStoreProject.Entities.Entities;
using SecilStoreProjectDal.Interface.Repositories;

namespace SecilStoreProject.WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ConfigurationController : ControllerBase
	{
		private readonly IConfigurationRepository _configurationRepository;
		private readonly ConfigurationReader _configurationReader;

		public ConfigurationController(IConfigurationRepository configurationRepository, ConfigurationReader configurationReader)
		{
			_configurationRepository = configurationRepository;
			_configurationReader = configurationReader;
		}

		// GET: api/configuration/{applicationName}
		[HttpGet("{applicationName}")]
		public async Task<IActionResult> GetAllConfigurations(string applicationName)
		{
			try
			{
				var configurations = await _configurationRepository.GetAllConfigurationsAsync(applicationName);
				return Ok(configurations);
			}
			catch (Exception ex)
			{
				// Log exception
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// GET: api/configuration/value/{key}
		[HttpGet("value/{key}")]
		public IActionResult GetConfigurationValue(string key)
		{
			try
			{
				var value = _configurationReader.GetValue<string>(key); // Assuming the value is of type string for demonstration
				return Ok(value);
			}
			catch (KeyNotFoundException knfEx)
			{
				return NotFound(knfEx.Message);
			}
			catch (Exception ex)
			{
				// Log exception
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// POST: api/configuration
		[HttpPost]
		public async Task<IActionResult> CreateConfiguration([FromBody] ConfigurationModel configurationModel)
		{
			try
			{
				await _configurationRepository.CreateConfigurationAsync(configurationModel);
				return CreatedAtAction(nameof(GetAllConfigurations), new { applicationName = configurationModel.ApplicationName }, configurationModel);
			}
			catch (Exception ex)
			{
				// Log exception
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// PUT: api/configuration
		[HttpPut]
		public async Task<IActionResult> UpdateConfiguration([FromBody] ConfigurationModel configurationModel)
		{
			try
			{
				var updateResult = await _configurationRepository.UpdateConfigurationAsync(configurationModel);
				if (!updateResult)
				{
					return NotFound();
				}

				return NoContent();
			}
			catch (Exception ex)
			{
				// Log exception
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// DELETE: api/configuration/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteConfiguration(string id)
		{
			try
			{
				var deleteResult = await _configurationRepository.DeleteAsync(id);
				if (!deleteResult)
				{
					return NotFound();
				}

				return NoContent();
			}
			catch (Exception ex)
			{
				// Log exception
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
	}
}