using ListingService.Services;
using ListingService.Models;

using Microsoft.AspNetCore.Mvc;

namespace ListingService.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ListingController : ControllerBase
{
    private readonly ILogger<ListingController> _logger;
    private string _imagePath = string.Empty;
    private readonly IListingMongoDBService _dbService;

    public ListingController(ILogger<ListingController> logger, IConfiguration configuration,
        IListingMongoDBService dbService)
    {
        _logger = logger;
        _imagePath = configuration["ListingImagePath"];
        _dbService = dbService;
    }

    [HttpPost("CreateListing")]
    public async Task<Guid?> CreateListing([FromBody] Listing listing)
    {
        return await _dbService.CreateListingAsync(listing);
    }

    [HttpDelete("DeleteListing/{id}")]
    public async Task<Guid> DeleteListing(Guid id)
    {
        var listing = new Listing { Id = id }; // Opret Listing med kun ID
        return await _dbService.DeleteListingAsync(listing); // Kald på service med Listing objekt
    }

    [HttpPut("{id}/price")]
    public async Task<Guid> UpdateListingPrice(Guid id, [FromBody] float newPrice)
    {
        return await _dbService.UpdateListingPriceAsync(id, newPrice);
    }

    [HttpGet("GetListing/{id}")]

    public async Task<Listing> GetlistingById(Guid id)
    {
        var listing = new Listing { Id = id };
        return await _dbService.GetListingByIdAsync(id);
    }
    
    [HttpGet("Getall")]
    public async Task<IEnumerable<Listing>> GetAllListings()
    {
        var listings = await _dbService.GetAllListingsAsync();
        return listings;
    }
    
}