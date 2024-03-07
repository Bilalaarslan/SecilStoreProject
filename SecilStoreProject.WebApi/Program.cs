using Microsoft.Extensions.Options;
using SecilStoreProject.Business;
using SecilStoreProject.Dal.Mongo.Extensions;
using SecilStoreProject.Entities.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMongoDBServices(builder.Configuration);
builder.Services.Configure<MongoDBSettings>(
	builder.Configuration.GetSection("MongoDBConnection"));
builder.Services.AddSingleton<ConfigurationReader>(serviceProvider =>
{
	var mongoSettings = serviceProvider.GetService<IOptions<MongoDBSettings>>().Value;
	return new ConfigurationReader(
		applicationName: "Secil", // Bu deðeri uygun þekilde deðiþtirin
		connectionString: mongoSettings.ConnectionString,
		refreshTimerIntervalInMs: 60000 // Örneðin, 60 saniye
	);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
