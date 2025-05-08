using System.Reflection.Metadata.Ecma335;

namespace ListingService.Models;

public class Listing
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public float AssesedPrice { get; set; }
    public string Description { get; set; }
    public Category Category { get; set; }
    public List<Uri> Image { get; set; } = new List<Uri>();
}