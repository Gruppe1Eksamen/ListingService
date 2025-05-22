using Moq;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;
using ListingService.Services;
using ListingService.Controllers;
using ListingService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace ListingService.Tests;

[TestClass]
public class ListingMongoDBServiceTests
{
    //VI mangler GetAllListingsAsync og GetListingByIdAsync
    [TestMethod]
    public async Task CreateListingAsync_InsertsListingAndReturnsId()
    {
        
        var mockCollection = new Mock<IMongoCollection<Listing>>();

        mockCollection
            .Setup(c => c.InsertOneAsync(It.IsAny<Listing>(), null, default))
            .Returns(Task.CompletedTask);

        // laver et test objekt som har de samme funktionaliteter  som det oprindelige
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
    [TestMethod]
    public async Task DeleteListingAsync_DeletesListingById()
    {
        var mockCollection = new Mock<IMongoCollection<Listing>>();
        var id = Guid.NewGuid();

        var filter = Builders<Listing>.Filter.Eq(l => l.Id, id);

        mockCollection
            .Setup(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<Listing>>(), default))
            .ReturnsAsync(new DeleteResult.Acknowledged(1));

        var service = new ListingMongoDBService(mockCollection.Object);

        var resultId = await service.DeleteListingAsync(new Listing { Id = id });

        Assert.AreEqual(id, resultId);
        mockCollection.Verify(
            c => c.DeleteOneAsync(It.IsAny<FilterDefinition<Listing>>(), default),
            Times.Once
        );
    }

    [TestMethod]
    public async Task UpdateListingPriceAsync_UpdatesPriceById()
    {
        var mockCollection = new Mock<IMongoCollection<Listing>>();
        var id = Guid.NewGuid();
        float newPrice = 499.99f;

        var filter = Builders<Listing>.Filter.Eq(l => l.Id, id);
        var update = Builders<Listing>.Update.Set(l => l.AssesedPrice, newPrice);

        mockCollection
            .Setup(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<Listing>>(), It.IsAny<UpdateDefinition<Listing>>(), null, default))
            .ReturnsAsync(new UpdateResult.Acknowledged(1, 1, null));

        var service = new ListingMongoDBService(mockCollection.Object);

        var resultId = await service.UpdateListingPriceAsync(id, newPrice);

        Assert.AreEqual(id, resultId);
        mockCollection.Verify(
            c => c.UpdateOneAsync(It.IsAny<FilterDefinition<Listing>>(), It.IsAny<UpdateDefinition<Listing>>(), null, default),
            Times.Once
        );
    }
}