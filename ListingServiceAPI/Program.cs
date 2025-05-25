using ListingService.Models;
using ListingService.Services;
using MongoDB.Driver;
using System.Text.Json.Serialization;
using NLog;
using NLog.Web;

// Setup NLog
var logger = LogManager
    .Setup()
    .LoadConfigurationFromFile("NLog.config")
    .GetCurrentClassLogger();

logger.Debug("Init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Clear default logging and enable NLog
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    // Read config / env variables
    var mongoConn = builder.Configuration["MongoConnectionString"]
                    ?? throw new InvalidOperationException("Missing MongoConnectionString");
    var databaseName = builder.Configuration["CatalogDatabase"]
                       ?? throw new InvalidOperationException("Missing CatalogDatabase");
    var collectionName = builder.Configuration["CatalogCollection"]
                         ?? throw new InvalidOperationException("Missing CatalogCollection");

    // Register Mongo services
    builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConn));
    builder.Services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));
    builder.Services.AddSingleton(sp => sp.GetRequiredService<IMongoDatabase>().GetCollection<Listing>(collectionName));
    builder.Services.AddSingleton<IListingMongoDBService, ListingMongoDBService>();

    // Controllers + JSON enum converter
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();

    // app.UseHttpsRedirection();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Application stopped due to exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}
