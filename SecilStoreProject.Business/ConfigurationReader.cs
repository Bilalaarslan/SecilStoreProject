using SecilStoreProject.Entities.Entities;
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
		var database = client.GetDatabase("Secil");
		_configurations = database.GetCollection<ConfigurationModel>("SecilDB");

		StartRefreshTask();
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
					// Log exception
					Console.WriteLine($"An error occurred during configuration refresh: {ex.Message}");
				}
				await Task.Delay(_refreshInterval);
			}
		});
	}

	private async Task RefreshConfigurations()
	{
		var filter = Builders<ConfigurationModel>.Filter.Eq(c => c.ApplicationName, _applicationName) &
					 Builders<ConfigurationModel>.Filter.Eq(c => c.IsActive, true);
		var configurations = await _configurations.Find(filter).ToListAsync();

		foreach (var config in configurations)
		{
			_cache.AddOrUpdate(config.Name, (config.Value, DateTime.UtcNow), (key, oldValue) => (config.Value, DateTime.UtcNow));
		}
	}

	public T GetValue<T>(string key)
	{
		var filter = Builders<ConfigurationModel>.Filter.Eq(c => c.Name, key) & Builders<ConfigurationModel>.Filter.Eq(c => c.IsActive, true);
		var configuration = _configurations.Find(filter).FirstOrDefault();

		if (configuration == null)
		{
			throw new KeyNotFoundException($"Configuration key '{key}' not found for application '{_applicationName}'.");
		}

		try
		{

			// Değeri T tipine dönüştürmek için daha genel bir yaklaşım
			// JSON deserialize yerine, Convert.ChangeType kullanılıyor.
			// T'nin değer tipi olduğu durumlar için (null olamayan tipler), varsayılan değeri döndürür
			var typeInfo = typeof(T).GetTypeInfo();
			if (typeInfo.IsValueType && Nullable.GetUnderlyingType(typeof(T)) == null && string.IsNullOrEmpty(configuration.Value))
			{
				return default(T); // veya 'throw new InvalidOperationException' eğer null döndürmek istemiyorsanız
			}
			else
			{
				// Basit tipler için Convert.ChangeType, daha karmaşık tipler için JsonConvert.DeserializeObject kullanılabilir
				return (T)Convert.ChangeType(configuration.Value, typeof(T));
			}
		}
		catch (JsonException ex)
		{
			throw new InvalidOperationException($"Error deserializing configuration value for key '{key}': {ex.Message}", ex);
		}
	}

}
