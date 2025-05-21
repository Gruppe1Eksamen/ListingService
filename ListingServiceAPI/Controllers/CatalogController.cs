using ListingService.Models;
using ListingService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ListingService.Controllers;

[ApiController]

[Route("[controller]")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogMongoDBService _catalogService;
    
    public CatalogController(ICatalogMongoDBService catalogService)
    {
        _catalogService = catalogService;
    }
    
    [HttpGet("catalogs")]//fjern ekstra tekst?
    public async Task<ActionResult<List<Catalog>>> GetCatalogs()
    {
        var catalogs = await _catalogService.GetAllCatalogsAsync();
        return Ok(catalogs);
    }

    //findes allerede på listings?
    [HttpGet("listings")]
    public async Task<ActionResult<List<Listing>>> GetListingsFromListingService()
    {
        var listings = await _catalogService.FetchAllListingsAsync();
        return Ok(listings);
    }
    
    
    [HttpPost("create")]//fjern ekstra tekst?
    public async Task<ActionResult<Guid>> CreateCatalog([FromBody] string name)
    {
        var listings = await _catalogService.FetchAllListingsAsync();

        var catalog = new Catalog
        {
            Id = Guid.NewGuid(),
            Name = name,
            AuctionDate = DateTime.UtcNow.AddDays(7),
            Listings = listings
        };

        var id = await _catalogService.CreateCatalogAsync(catalog);
        return Ok(id);
    }
    
    [HttpGet("ping")]
    public ActionResult<bool> Ping()
    {
        return Ok(true);
    }
}