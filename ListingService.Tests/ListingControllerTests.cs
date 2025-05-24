using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ListingService.Controllers;
using ListingService.Models;
using ListingService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ListingService.Tests
{
    [TestClass]
    public class ListingControllerTests
    {
        private Mock<ILogger<ListingController>> _mockLogger;
        private Mock<IConfiguration> _mockConfig;
        private Mock<IListingMongoDBService> _mockDbService;
        private ListingController _controller;
        private Listing _sampleListing;

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<ListingController>>();
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.SetupGet(c => c["ListingImagePath"]).Returns("images/");

            _mockDbService = new Mock<IListingMongoDBService>();
            _controller = new ListingController(
                _mockLogger.Object,
                _mockConfig.Object,
                _mockDbService.Object
            );

            _sampleListing = new Listing
            {
                Id = "507f1f77bcf86cd799439011",
                Name = "Antik stol",
                AssesedPrice = 1500.0f,
                Description = "En smuk antik stol i egetræ.",
                ListingCategory = ListingCategory.Furniture,
                Location = "København",
                SellerId = Guid.Parse("11111111-2222-3333-4444-555555555555"),
                Image = new List<Uri>
                {
                    new Uri("http://example.com/img1.jpg"),
                    new Uri("http://example.com/img2.jpg")
                }
            };
        }

        [TestMethod]
        public async Task CreateListing_ReturnsNewId()
        {
            // Arrange
            var newListing = new Listing
            {
                Name = "Malerisamling",
                AssesedPrice = 5000f,
                Description = "Eksklusiv maleri fra 1800-tallet",
                ListingCategory = ListingCategory.Painting,
                Location = "Aarhus",
                SellerId = Guid.NewGuid(),
                Image = new List<Uri> { new Uri("http://example.com/painting.jpg") }
            };
            var expectedId = "abcdef123456";

            _mockDbService
                .Setup(s => s.CreateListingAsync(newListing))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _controller.CreateListing(newListing);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedId, result);
        }

        [TestMethod]
        public async Task DeleteListing_WhenExists_ReturnsNoContent()
        {
            // Arrange
            _mockDbService
                .Setup(s => s.DeleteListingAsync(_sampleListing.Id!))
                .ReturnsAsync(true);

            // Act
            var actionResult = await _controller.DeleteListing(_sampleListing.Id!);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task DeleteListing_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockDbService
                .Setup(s => s.DeleteListingAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act
            var actionResult = await _controller.DeleteListing("nonexistent");

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task UpdateListingPrice_ReturnsUpdatedId()
        {
            // Arrange
            var id = _sampleListing.Id!;
            float newPrice = 2000f;
            var updatedId = "updatedId123";

            _mockDbService
                .Setup(s => s.UpdateListingPriceAsync(id, newPrice))
                .ReturnsAsync(updatedId);

            // Act
            var result = await _controller.UpdateListingPrice(id, newPrice);

            // Assert
            Assert.AreEqual(updatedId, result);
        }

        [TestMethod]
        public async Task GetlistingById_ReturnsCorrectListing()
        {
            // Arrange
            var id = _sampleListing.Id!;
            _mockDbService
                .Setup(s => s.GetListingByIdAsync(id))
                .ReturnsAsync(_sampleListing);

            // Act
            var listing = await _controller.GetlistingById(id);

            // Assert
            Assert.IsNotNull(listing);
            Assert.AreEqual(_sampleListing.Id, listing.Id);
            Assert.AreEqual(_sampleListing.Name, listing.Name);
            Assert.AreEqual(_sampleListing.AssesedPrice, listing.AssesedPrice);
            Assert.AreEqual(_sampleListing.Description, listing.Description);
            Assert.AreEqual(_sampleListing.ListingCategory, listing.ListingCategory);
            Assert.AreEqual(_sampleListing.Location, listing.Location);
            Assert.AreEqual(_sampleListing.SellerId, listing.SellerId);
            CollectionAssert.AreEqual(_sampleListing.Image, listing.Image);
        }

        [TestMethod]
        public async Task GetAllListings_ReturnsAllListings()
        {
            // Arrange
            var list = new List<Listing>
            {
                _sampleListing,
                new Listing
                {
                    Id = "507f191e810c19729de860ea",
                    Name = "Sølvbestik",
                    AssesedPrice = 800f,
                    Description = "Retro sølvbestik",
                    ListingCategory = ListingCategory.Silver,
                    Location = "Odense",
                    SellerId = Guid.NewGuid(),
                    Image = new List<Uri>()
                }
            };

            _mockDbService
                .Setup(s => s.GetAllListingsAsync())
                .ReturnsAsync(list);

            // Act
            var result = await _controller.GetAllListings();

            // Assert
            CollectionAssert.AreEqual(list, new List<Listing>(result));
        }
    }
}
