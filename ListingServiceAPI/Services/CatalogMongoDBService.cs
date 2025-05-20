using CatalogService.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using CatalogService.Services;
using ListingService.Models;
using ListingService.Services;

namespace ListingService.Services
{
    public class CatalogMongoDBService : ICatalogMongoDBService
    {
        private readonly IMongoCollection<Catalog> _catalogCollection;
        
        private readonly IMongoCollection<Listing> _listingCollection;
        private readonly HttpClient _http;

        public CatalogMongoDBService(MongoDBContext context, IHttpClientFactory httpFactory)
        {
            _catalogCollection = context.CatalogCollection;
            _http = httpFactory.CreateClient("ListingClient");
        }

        public async Task<List<Catalog>> GetAllCatalogsAsync()
        {
            return await _catalogCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Guid> CreateCatalogAsync(Catalog catalog)
        {
            catalog.Id = Guid.NewGuid();
            await _catalogCollection.InsertOneAsync(catalog);
            return catalog.Id;
        }
        

        public async Task<List<Listing>> FetchAllListingsAsync()
        {
            var listings = await _http.GetFromJsonAsync<List<Listing>>("/api/listing/Getall");
            return listings ?? new List<Listing>();
        }
        
     
    }
}
