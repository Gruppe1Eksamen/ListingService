using ListingService.Models;

namespace ListingService.Services
{
    public interface ICatalogMongoDBService
    {
        Task<List<Catalog>> GetAllCatalogsAsync();
       
        Task<Guid> CreateCatalogAsync(Catalog catalog);
        
        Task<List<Listing>> FetchAllListingsAsync();
        
        
    }
}