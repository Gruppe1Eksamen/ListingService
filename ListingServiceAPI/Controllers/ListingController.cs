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
    public async Task<Guid?> CreateListing(Listing listing)
    {
        return await _dbService.CreateListingAsync(listing);
    }
}