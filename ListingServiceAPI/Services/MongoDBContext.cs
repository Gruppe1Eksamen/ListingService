using ListingService.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace ListingService.Services;

public class MongoDBContext
{
    public IMongoDatabase Database { get; set; }
    public IMongoCollection<Listing> ListingCollection  { get; }
    public MongoDBContext(ILogger<ListingMongoDBService> logger, IConfiguration config)
    {        
        

        var client = new MongoClient(config["MongoConnectionString"]);
        Database = client.GetDatabase(config["ListingDB"]);
        ListingCollection = Database.GetCollection<Listing>(config["Listings"]);
        
        logger.LogInformation($"Connected to database {config["ListingDB"]}");
        logger.LogInformation($"Using collection {config["Listings"]}");
    }
    
    
}