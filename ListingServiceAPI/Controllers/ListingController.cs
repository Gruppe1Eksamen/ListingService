using ListingService.Services;
using ListingService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingService.Controllers;

[ApiController]
[Route("[controller]")]

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

    [Authorize]
    [HttpPost]
    public async Task<string?> CreateListing([FromBody] Listing listing)
    {
        return await _dbService.CreateListingAsync(listing);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteListing(string id)
    {
        var result = await _dbService.DeleteListingAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    [Authorize]
    [HttpPut("{id}/price")]
    public async Task<string> UpdateListingPrice(string id, [FromBody] float newPrice)
    {
        return await _dbService.UpdateListingPriceAsync(id, newPrice);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<Listing> GetlistingById(string id)
    {
        var listing = new Listing { Id = id };
        return await _dbService.GetListingByIdAsync(id);
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IEnumerable<Listing>> GetAllListings()
    {
        var listings = await _dbService.GetAllListingsAsync();
        return listings;
    }
    
    [Authorize]
    [HttpGet("auth")]
    public async Task<IActionResult> Get()
    {
        return Ok("You're authorized");
    }
}