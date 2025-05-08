using ListingService.Models;

namespace ListingService.Services;

public interface IListingMongoDBService
{
    Task<Guid> CreateListingAsync(Listing listing);
}