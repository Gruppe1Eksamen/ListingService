using ListingService.Models;
using ListingService.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// 1) Read your environment variables (as set under the "http" profile in launchSettings.json)
var mongoConn     = builder.Configuration["MongoConnectionString"]
                    ?? throw new InvalidOperationException("Missing MongoConnectionString");
var databaseName  = builder.Configuration["CatalogDatabase"]   // e.g. "catalogDB"
                    ?? throw new InvalidOperationException("Missing CatalogDatabase");
var collectionName = builder.Configuration["CatalogCollection"] // e.g. "listings"
                     ?? throw new InvalidOperationException("Missing CatalogCollection");

// 2) Register Mongo types
builder.Services.AddSingleton<IMongoClient>(_ => 
    new MongoClient(mongoConn));

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IMongoClient>()
        .GetDatabase(databaseName));            // uses your "catalogDB"

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IMongoDatabase>()
        .GetCollection<Listing>(collectionName)); // uses your "listings"

// 3) Register your service
builder.Services.AddSingleton<IListingMongoDBService, ListingMongoDBService>();

// 4) Enable controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5) Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 6) Wire up attribute‐routed controllers
app.MapControllers();

app.Run();