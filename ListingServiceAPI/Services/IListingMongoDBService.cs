using ListingService.Models;

namespace ListingService.Services;

public interface IListingMongoDBService
{
    Task<string> CreateListingAsync(Listing listing);
    Task<bool> DeleteListingAsync(string id);
    Task<string> UpdateListingPriceAsync(string id, float newPrice);
    Task<Listing> GetListingByIdAsync(string id);
    Task<List<Listing>> GetAllListingsAsync();
    

}