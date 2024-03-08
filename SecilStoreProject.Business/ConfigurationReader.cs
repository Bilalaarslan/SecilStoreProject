using MongoDB.Driver;
using SecilStoreProject.Entities.Entities;
using SecilStoreProject.Entities.Enums;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;

namespace SecilStoreProject.Business;

public class ConfigurationReader
{
	private readonly string _applicationName;
	private readonly IMongoCollection<ConfigurationModel> _configurations;
	private readonly ConcurrentDictionary<string, (string Value, DateTime LastUpdated)> _cache = new();
	private readonly TimeSpan _refreshInterval;

	public ConfigurationReader(string applicationName, string connectionString, int refreshTimerIntervalInMs)
	{
		_applicationName = applicationName;
		_refreshInterval = TimeSpan.FromMilliseconds(refreshTimerIntervalInMs);

		var client = new MongoClient(connectionString);
		var database = client.GetDatabase("SecilDataBase");
		_configurations = database.GetCollection<ConfigurationModel>("SecilDB");

		StartRefreshTask();
	}

    public async Task CheckForUpdatesAndNotify()
    {
        var latestUpdate = _cache.Values.Any() ? _cache.Values.OrderByDescending(v => v.LastUpdated).First().LastUpdated : DateTime.MinValue;


        var filter = Builders<ConfigurationModel>.Filter.Eq(c => c.ApplicationName, _applicationName) &
                     Builders<ConfigurationModel>.Filter.Gt(c => c.LastUpdated, latestUpdate);

        var updates = await _configurations.Find(filter).ToListAsync();

        foreach (var update in updates)
        {
            _cache.AddOrUpdate(update.Name, (update.Value, DateTime.UtcNow), (key, oldValue) => (update.Value, DateTime.UtcNow));

        }
    }

    private void StartRefreshTask()
	{
		Task.Run(async () =>
		{
			while (true)
			{
				try
				{
					await RefreshConfigurations();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"An error occurred during configuration refresh: {ex.Message}");
				}
				await Task.Delay(_refreshInterval);
			}
		});
	}
    private async Task RefreshConfigurations()
    {
        try
        {
            var filter = Builders<ConfigurationModel>.Filter.Eq(c => c.ApplicationName, _applicationName) &
                         Builders<ConfigurationModel>.Filter.Eq(c => c.IsActive, true);
            var configurations = await _configurations.Find(filter).ToListAsync();

            foreach (var config in configurations)
            {
                _cache.AddOrUpdate(config.Name, (config.Value, DateTime.UtcNow), (key, oldValue) => (config.Value, DateTime.UtcNow));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not refresh configurations from MongoDB: {ex.Message}. Continuing with cached values.");

        }
    }

    public T GetValue<T>(string key)
    {
        var configuration = _configurations.Find(Builders<ConfigurationModel>.Filter.Eq(c => c.Name, key) & Builders<ConfigurationModel>.Filter.Eq(c => c.IsActive, true)).FirstOrDefault();

        if (configuration == null)
        {
            throw new KeyNotFoundException($"Configuration key '{key}' not found for application '{_applicationName}'.");
        }

        try
        {
            switch (configuration.Type)
            {
                case ConfigurationValueType.String:
                    return (T)Convert.ChangeType(configuration.Value, typeof(T));
                case ConfigurationValueType.Int:
                    return (T)Convert.ChangeType(int.Parse(configuration.Value), typeof(T));
                case ConfigurationValueType.Bool:
                    return (T)Convert.ChangeType(bool.Parse(configuration.Value), typeof(T));
                case ConfigurationValueType.Double:
                    return (T)Convert.ChangeType(double.Parse(configuration.Value), typeof(T));
                default:
                    throw new InvalidOperationException($"Unsupported configuration type '{configuration.Type}' for key '{key}'.");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error processing configuration value for key '{key}': {ex.Message}", ex);
        }
    }

}
