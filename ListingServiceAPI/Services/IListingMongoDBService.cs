using ListingService.Models;

namespace ListingService.Services;

public interface IListingMongoDBService
{
    Task<Guid> CreateListingAsync(Listing listing);
    Task<Guid> DeleteListingAsync(Listing listing);
    Task<Guid> UpdateListingPriceAsync(Guid id, float newPrice);
    Task<Listing> GetListingAsync(Guid id);
}