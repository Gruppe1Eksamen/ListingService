using ListingService.Models;
using CatalogService.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace ListingService.Services;

public class MongoDBContext
{
    public IMongoDatabase Database { get; set; }
    public IMongoCollection<Listing> ListingCollection  { get; }
    public IMongoCollection<Catalog> CatalogCollection  { get; }
    public MongoDBContext(ILogger<ListingMongoDBService> logger, IConfiguration config)
    {        
        

        var client = new MongoClient(config["MongoConnectionString"]);
        Database = client.GetDatabase(config["ListingDB"]);
        ListingCollection = Database.GetCollection<Listing>(config["Listings"]);
        CatalogCollection = Database.GetCollection<Catalog>(config["Catalogs"]);
        
        logger.LogInformation($"Connected to database {config["ListingDB"]}");
        logger.LogInformation($"Using collection {config["Listings"]}");
    }
    
    
}