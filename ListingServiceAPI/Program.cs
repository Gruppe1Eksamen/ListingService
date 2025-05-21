using NLog;
using NLog.Web;
using ListingService.Models;
using ListingService.Services;
using MongoDB.Driver;
using System.Text.Json.Serialization;
using CatalogService.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

// Setup NLog for Dependency injection and configuration
var logger = LogManager.Setup()
    .LoadConfigurationFromFile("NLog.config")
    .GetCurrentClassLogger();

try
{
    logger.Debug("Init main");
    logger.Info(">>> THIS IS A TEST LOG TO LOKI <<<");
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.Commons;

var endPoint = Environment.GetEnvironmentVariable("VAULT_ENDPOINT") ?? "https://localhost:8201";
Console.WriteLine($"VAULT_ENDPOINT: {endPoint}");

var httpClientHandler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
};

IAuthMethodInfo authMethod = new TokenAuthMethodInfo("00000000-0000-0000-0000-000000000000");
var vaultClientSettings = new VaultClientSettings(endPoint, authMethod)
{
    MyHttpClientProviderFunc = handler => new HttpClient(httpClientHandler)
    {
        BaseAddress = new Uri(endPoint)
    }
};
IVaultClient vaultClient = new VaultClient(vaultClientSettings);

Secret<SecretData> kv2Secret;
try
{
    kv2Secret = await ReadVaultSecretWithRetryAsync(
        vaultClient,
        path: "passwords",
        mountPoint: "secret",
        maxRetries: 5,
        delayBetweenRetries: TimeSpan.FromSeconds(5));
}
catch (Exception ex)
{
    Console.WriteLine("Kunne ikke hente secrets fra Vault efter 5 forsøg: " + ex.Message);
    return;
}

var mySecret = kv2Secret.Data.Data["Secret"].ToString();
var myIssuer = kv2Secret.Data.Data["Issuer"].ToString();
Console.WriteLine($"Issuer er: {myIssuer}, og secret: {mySecret}");

    var builder = WebApplication.CreateBuilder(args);
    BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

    // Use NLog for logging instead of default .NET logging
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    // 1) Read your environment variables (as set under the "http" profile in launchSettings.json)
    var mongoConn = builder.Configuration["MongoConnectionString"]
                    ?? throw new InvalidOperationException("Missing MongoConnectionString");
    var databaseName = builder.Configuration["ListingDB"]   // e.g. "catalogDB"
                       ?? throw new InvalidOperationException("Missing CatalogDatabase");
    var collectionName = builder.Configuration["CatalogCollection"] // e.g. "listings"
                         ?? throw new InvalidOperationException("Missing CatalogCollection");

    // 2) Register Mongo types
    builder.Services.AddSingleton<IMongoClient>(_ =>
        new MongoClient(mongoConn));

    builder.Services.AddSingleton(sp =>
        sp.GetRequiredService<IMongoClient>()
            .GetDatabase(databaseName)); // uses your "catalogDB"

    builder.Services.AddSingleton(sp =>
        sp.GetRequiredService<IMongoDatabase>()
            .GetCollection<Listing>(collectionName)); // uses your "listings"
builder.Configuration["Secret"] = mySecret;
builder.Configuration["Issuer"] = myIssuer;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = myIssuer,
            ValidAudience = "http://localhost",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(mySecret))
        };
    });



var mongoConn     = builder.Configuration["MongoConnectionString"]
                    ?? throw new InvalidOperationException("Missing MongoConnectionString");
var databaseName  = builder.Configuration["CatalogDatabase"]
                    ?? throw new InvalidOperationException("Missing CatalogDatabase");
var collectionName = builder.Configuration["CatalogCollection"]
                     ?? throw new InvalidOperationException("Missing CatalogCollection");

builder.Services.AddSingleton<IMongoClient>(_ => 
    new MongoClient(mongoConn));

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IMongoClient>()
        .GetDatabase(databaseName));

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IMongoDatabase>()
        .GetCollection<Listing>(collectionName));

    
    builder.Services.AddScoped<MongoDBContext>();
    
    builder.Services.AddHttpClient("ListingClient", c =>
    {
        c.BaseAddress = new Uri("http://localhost:5136"); // Ret port hvis nødvendig!
    });
    
    builder.Services.AddSingleton<IListingMongoDBService, ListingMongoDBService>();
    builder.Services.AddScoped<ICatalogMongoDBService, CatalogMongoDBService>();
    
    builder.Services
        .AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI();

    // app.UseHttpsRedirection(); // Uncomment if using HTTPS

    app.MapControllers();

    app.Run();
}
{
    // NLog: catch setup errors
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown(); // Ensure to flush and stop internal timers/threads
}
// 6) Wire up attribute‐routed controllers
app.MapControllers();
app.Run();

static async Task<Secret<SecretData>> ReadVaultSecretWithRetryAsync(
    IVaultClient vaultClient,
    string path,
    string mountPoint,
    int maxRetries = 5,
    TimeSpan? delayBetweenRetries = null)
{
    if (delayBetweenRetries == null)
        delayBetweenRetries = TimeSpan.FromSeconds(5);

    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            return await vaultClient.V1.Secrets.KeyValue.V2
                .ReadSecretAsync(path: path, mountPoint: mountPoint);
        }
        catch (Exception ex) when (attempt < maxRetries)
        {
            Console.WriteLine(
                $"[Vault] Forsøg {attempt} mislykkedes: {ex.Message}. " +
                $"Venter {delayBetweenRetries.Value.TotalSeconds} sek. før retry...");
            await Task.Delay(delayBetweenRetries.Value);
        }
    }

    throw new Exception($"Failed to read secret from Vault efter {maxRetries} forsøg.");
}
