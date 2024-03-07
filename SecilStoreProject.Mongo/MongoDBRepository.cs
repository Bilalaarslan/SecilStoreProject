using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using SecilStoreProject.Entities.Entities;
using SecilStoreProjectDal.Interface.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SecilStoreProject.Dal.Mongo;


public class MongoDBRepository : IConfigurationRepository
{
	private readonly IMongoCollection<ConfigurationModel> _configurations;

	public MongoDBRepository(IOptions<MongoDBSettings> settings)
	{
		var client = new MongoClient(settings.Value.ConnectionString);
		var database = client.GetDatabase(settings.Value.DatabaseName);
		_configurations = database.GetCollection<ConfigurationModel>(settings.Value.CollectionName);
	}

	public async Task<IEnumerable<ConfigurationModel>> GetAllConfigurationsAsync(string applicationName)
	{
		return await _configurations
			.Find(configuration => configuration.ApplicationName == applicationName)
			.ToListAsync();
	}

	public async Task<ConfigurationModel> GetConfigurationByIdAsync(string id)
	{
		return await _configurations
			.Find(configuration => configuration.Id == id)
			.FirstOrDefaultAsync();
	}

	public async Task CreateConfigurationAsync(ConfigurationModel configurationModel)
	{
		configurationModel.Id = ObjectId.GenerateNewId().ToString();
		await _configurations.InsertOneAsync(configurationModel);
	}

	public async Task<bool> UpdateConfigurationAsync(ConfigurationModel configurationModel)
	{
		var result = await _configurations.ReplaceOneAsync(
			configuration => configuration.Id == configurationModel.Id, configurationModel);

		return result.IsAcknowledged && result.ModifiedCount > 0;
	}

	public async Task<bool> DeleteAsync(string id)
	{
		var result = await _configurations.DeleteOneAsync(configuration => configuration.Id == id);

		return result.IsAcknowledged && result.DeletedCount > 0;
	}

}
