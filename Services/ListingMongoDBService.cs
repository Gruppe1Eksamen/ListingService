using ListingService.Models;

using MongoDB.Driver;

namespace ListingService.Services;

public class ListingMongoDBService : IListingMongoDBService
{
    private readonly IMongoCollection<Listing> _listingCollection;


    public async Task<Guid> CreateListingAsync(Listing listing)
    {
        listing.Id = Guid.NewGuid();
        await _listingCollection.InsertOneAsync(listing);
        return listing.Id;
    }
}