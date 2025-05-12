using ListingService.Models;

using MongoDB.Driver;

namespace ListingService.Services;

public class ListingMongoDBService : IListingMongoDBService
{
    private readonly IMongoCollection<Listing> _listingCollection;

    public ListingMongoDBService(IMongoCollection<Listing> collection)
    {
        _listingCollection = collection;
    }


    public async Task<Guid> CreateListingAsync(Listing listing)
    {
        listing.Id = Guid.NewGuid();
        await _listingCollection.InsertOneAsync(listing);
        return listing.Id;
    }

    public async Task<Guid> DeleteListingAsync(Listing listing)
    {
        var filter = Builders<Listing>.Filter.Eq(l => l.Id, listing.Id);
        await _listingCollection.DeleteOneAsync(filter);
        return listing.Id;
    }

    public async Task<Guid> UpdateListingPriceAsync(Guid id, float newPrice)
    {
        var filter = Builders<Listing>.Filter.Eq(l => l.Id, id);
        var update = Builders<Listing>.Update.Set(l => l.AssesedPrice, newPrice);
        await _listingCollection.UpdateOneAsync(filter, update);
        return id;
    }

    public async Task<Listing> GetListingAsync(Guid id)
    {
        var filter = Builders<Listing>.Filter.Eq(l => l.Id, id);
        return await _listingCollection.Find(filter).FirstOrDefaultAsync();
    }
}