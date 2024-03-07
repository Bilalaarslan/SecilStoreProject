using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SecilStoreProject.Entities.Entities;
using SecilStoreProjectDal.Interface.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecilStoreProject.Dal.Mongo.Extensions;

public static class ServiceExtension
{
	public static IServiceCollection AddMongoDBServices(this IServiceCollection services, IConfiguration configuration)
	{

		services.Configure<MongoDBSettings>(configuration.GetSection("MongoDBConnection"));

		services.AddSingleton<IMongoDatabase>(serviceProvider =>
		{
			var settings = serviceProvider.GetRequiredService<IOptions<MongoDBSettings>>().Value;

			var client = new MongoClient(settings.ConnectionString);

			return client.GetDatabase(settings.DatabaseName);
		});


		services.AddScoped<IConfigurationRepository, MongoDBRepository>();

		return services;
	}
}
