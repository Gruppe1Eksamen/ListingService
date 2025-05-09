using Moq;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;
using ListingService.Services;
using ListingService.Controllers;
using ListingService.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace ListingService.Tests;

[TestClass]
public class ListingMongoDBServiceTests
{
    [TestMethod]
    public async Task CreateListingAsync_InsertsListingAndReturnsId()
    {
        
        var mockCollection = new Mock<IMongoCollection<Listing>>();

        mockCollection
            .Setup(c => c.InsertOneAsync(It.IsAny<Listing>(), null, default))
            .Returns(Task.CompletedTask);

        var service = new ListingMongoDBService(mockCollection.Object);

        var listing = new Listing
        {
            Name = "Test Listing",
            AssesedPrice = 123.45f,
            Description = "A beautiful antique chair in excellent condition.",
            ListingCategory = ListingCategory.Furniture,
            Image = new List<Uri>
            {
                new Uri("http://example.com/images/chair1.jpg"),
                new Uri("http://example.com/images/chair2.jpg")
            }
        };

        var resultId = await service.CreateListingAsync(listing);

        Assert.AreNotEqual(Guid.Empty, resultId);

        mockCollection.Verify(
            c => c.InsertOneAsync(It.Is<Listing>(l => l.Id == resultId), null, default),
            Times.Once
        );
    }
}