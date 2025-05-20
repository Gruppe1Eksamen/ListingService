using CatalogService.Models;
using ListingService.Models;

namespace CatalogService.Services
{
    public interface ICatalogMongoDBService
    {
        Task<List<Catalog>> GetAllCatalogsAsync();
       
        Task<Guid> CreateCatalogAsync(Catalog catalog);
        
        Task<List<Listing>> FetchAllListingsAsync();
        
        
    }
}