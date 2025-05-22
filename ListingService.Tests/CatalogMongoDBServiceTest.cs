using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ListingService.Controllers;
using ListingService.Models;
using ListingService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ListingService.Tests
{
    [TestClass]
    public class CatalogControllerTests
    {
        private Mock<ICatalogMongoDBService> _mockService;
        private CatalogController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockService = new Mock<ICatalogMongoDBService>();
            _controller = new CatalogController(_mockService.Object);
        }

        [TestMethod]
        public async Task GetCatalogs_ReturnsOkWithCatalogList()
        {
            // Arrange
            var catalogs = new List<Catalog>
            {
                new Catalog { Id = Guid.NewGuid(), Name = "Cat1" },
                new Catalog { Id = Guid.NewGuid(), Name = "Cat2" }
            };
            _mockService
                .Setup(s => s.GetAllCatalogsAsync())
                .ReturnsAsync(catalogs);

            // Act
            var actionResult = await _controller.GetCatalogs();

            // Assert
            var ok = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(ok, "Forventer OkObjectResult");
            var returned = ok.Value as List<Catalog>;
            CollectionAssert.AreEqual(catalogs, returned);
        }

        [TestMethod]
        public async Task CreateCatalog_ReturnsOkWithCreatedId_AndPassesCorrectCatalog()
        {
            // Arrange
            var name = "NewCatalog";
            var listings = new List<Listing>
            {
                new Listing { Id = Guid.NewGuid(), Name = "LX" }
            };
            var expectedId = Guid.NewGuid();

            _mockService
                .Setup(s => s.FetchAllListingsAsync())
                .ReturnsAsync(listings);

            Catalog captured = null;
            _mockService
                .Setup(s => s.CreateCatalogAsync(It.IsAny<Catalog>()))
                .Callback<Catalog>(c => captured = c)
                .ReturnsAsync(expectedId);

            // Act
            var actionResult = await _controller.CreateCatalog(name);

            // Assert
            var ok = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(ok, "Forventer OkObjectResult");
            Assert.AreEqual(expectedId, ok.Value);

            // Verificer at service fik den rigtige Catalog
            Assert.IsNotNull(captured, "Service.CreateCatalogAsync blev ikke kaldt");
            Assert.AreEqual(name, captured.Name);
            CollectionAssert.AreEqual(listings, captured.Listings);

            // AuctionDate skal være præcis 7 dage frem (dato‐sammenligning for stabilitet)
            var expectedDate = DateTime.UtcNow.AddDays(7).Date;
            Assert.AreEqual(expectedDate, captured.AuctionDate.Date);
            Assert.AreNotEqual(Guid.Empty, captured.Id, "Catalog.Id bør være sat til en ny Guid");
        }
    }
}
