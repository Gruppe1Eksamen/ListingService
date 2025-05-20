using Moq;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Models;
using ListingService.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace ListingService.Tests
{
    [TestClass]
    public class CatalogMongoDbServiceTest
    {
        [TestMethod]
        public async Task GetAllCatalogsAsync_WhenCalled_ReturnsAllCatalogs()
        {
            // ARRANGE: Fake data
            var fakeCatalogs = new List<Catalog>
            {
                new Catalog
                {
                    Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                    Name = "Spring Antiques Auction 2025",
                    AuctionDate = new DateTime(2025, 6, 15),
                    Listings = new List<ListingService.Models.Listing>()
                },
                new Catalog
                {
                    Id = Guid.Parse("d4444444-4444-4444-4444-444444444444"),
                    Name = "Summer Fine Art & Porcelain",
                    AuctionDate = new DateTime(2025, 7, 20),
                    Listings = new List<ListingService.Models.Listing>()
                }
            };

            // 1. Mock IFindFluent<Catalog, Catalog>
            var mockFluent = new Mock<IFindFluent<Catalog, Catalog>>();
            mockFluent
                .Setup(f => f.ToListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeCatalogs);

            // 2. Mock IMongoCollection<Catalog> for begge relevante overloads af Find
            var mockCatalogCollection = new Mock<IMongoCollection<Catalog>>();

            // a) Overload med kun filter (det din service bruger)
            mockCatalogCollection
                .Setup(c => c.Find(It.IsAny<Expression<Func<Catalog, bool>>>(), (FindOptions<Catalog, Catalog>)null))
                .Returns(mockFluent.Object);

            // b) Overload med filter + options (for robusthed)
            mockCatalogCollection
                .Setup(c => c.Find(It.IsAny<Expression<Func<Catalog, bool>>>(), It.IsAny<FindOptions<Catalog, Catalog>>()))
                .Returns(mockFluent.Object);

            // 3. Mock Logger og Config for MongoDBContext
            var mockLogger = new Mock<ILogger<ListingMongoDBService>>();
            var mockConfig = new Mock<IConfiguration>();

            // 4. Mock MongoDBContext (brug mockLogger & mockConfig)
            var mockContext = new Mock<MongoDBContext>(mockLogger.Object, mockConfig.Object);
            mockContext
                .SetupGet(ctx => ctx.CatalogCollection)
                .Returns(mockCatalogCollection.Object);

            // 5. Mock IHttpClientFactory
            var mockHttpFactory = new Mock<IHttpClientFactory>();

            // 6. Opret service med mocket context og httpFactory
            var service = new CatalogMongoDBService(mockContext.Object, mockHttpFactory.Object);

            // ACT
            var result = await service.GetAllCatalogsAsync();

            // ASSERT
            CollectionAssert.AreEqual(fakeCatalogs, result);

            // VERIFY: Find blev kaldt
            mockCatalogCollection.Verify(c =>
                c.Find(
                    It.IsAny<Expression<Func<Catalog, bool>>>(),
                    (FindOptions<Catalog, Catalog>)null), // matcher overload din service faktisk bruger!
                Times.Once);
        }
    }
}
