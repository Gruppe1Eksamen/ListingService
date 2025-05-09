using ListingService.Controllers;
using ListingService.Models;
using ListingService.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IListingMongoDBService, ListingMongoDBService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// In-memory storage
var listings = new List<Listing>();

// POST endpoint to create a listing
app.MapPost("/listings", ([FromBody] Listing newListing) =>
{
    newListing.Id = Guid.NewGuid(); // generate a new Id
    listings.Add(newListing);
    return Results.Created($"/listings/{newListing.Id}", newListing);
});

// GET endpoint to return all listings
app.MapGet("/listings", () => listings);

app.Run();