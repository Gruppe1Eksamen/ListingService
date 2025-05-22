using ListingService.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ListingService.Services;

public class ListingMongoDBService : IListingMongoDBService
{
    private readonly IMongoCollection<Listing> _listingCollection;

    public ListingMongoDBService(IMongoCollection<Listing> collection)
    {
        _listingCollection = collection;
    }


    public async Task<string> CreateListingAsync(Listing listing)
    {
        listing.Id = ObjectId.GenerateNewId().ToString(); // Generate a string-based ID
        await _listingCollection.InsertOneAsync(listing);
        return listing.Id;
    }
    
    public async Task<bool> DeleteListingAsync(string id)
    {
        var result = await _listingCollection.DeleteOneAsync(u => u.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<string> UpdateListingPriceAsync(string id, float newPrice)
    {
        var filter = Builders<Listing>.Filter.Eq(l => l.Id, id);
        var update = Builders<Listing>.Update.Set(l => l.AssesedPrice, newPrice);
        await _listingCollection.UpdateOneAsync(filter, update);
        return id;
    }

    public async Task<Listing> GetListingByIdAsync(string id)
    {
        return await _listingCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
    }
    
    public async Task<List<Listing>> GetAllListingsAsync()
    {
        return await _listingCollection.Find(_ => true).ToListAsync();
    }
    
    
}