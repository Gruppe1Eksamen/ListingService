using ListingService.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace ListingService.Services;

public class MongoDBContext
{
    public IMongoDatabase Database { get; set; }
    public IMongoCollection<Listing> Collection { get; set; }
    
    public MongoDBContext(ILogger<ListingMongoDBService> logger, IConfiguration config)
    {        
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

        var client = new MongoClient(config["MongoConnectionString"]);
        Database = client.GetDatabase(config["CatalogDB"]);
        Collection = Database.GetCollection<Listing>(config["Listings"]);

        logger.LogInformation($"Connected to database {config["CatalogDB"]}");
        logger.LogInformation($"Using collection {config["Listings"]}");
    }
    
    
}