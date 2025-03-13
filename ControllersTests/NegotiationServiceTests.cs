using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;


namespace NegotiationAPI.ControllersTests
{
    public class NegotiationServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly NegotiationService _service;

        public NegotiationServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new NegotiationService(_context);
        }

        [Fact]
        public async Task RejectOfferAsync_OfferDoesNotExist_ReturnsNotFound()
        {
            int nonExistentOfferId = 999;

            var result = await _service.RejectOfferAsync(nonExistentOfferId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Offer does not exist.", notFoundResult.Value);
        }

        [Fact]
        public async Task RejectOfferAsync_OfferAlreadyConsidered_ReturnsBadRequest()
        {
            var existingOffer = new NegotiationOffer { Id = 1, Status = "Accepted", OfferDetails = "testOffer", SenderUsername = "testUser" };
            _context.NegotiationOffers.Add(existingOffer);
            await _context.SaveChangesAsync();

            var result = await _service.RejectOfferAsync(existingOffer.Id);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Offer has already been considered.", badRequestResult.Value);
        }

        [Fact]
        public async Task AcceptOfferAsync_OfferAlreadyConsidered_ReturnsBadRequest()
        {
            var existingOffer = new NegotiationOffer { Id = 1, Status = "Accepted", OfferDetails = "testOffer", SenderUsername = "testUser" };
            _context.NegotiationOffers.Add(existingOffer);
            await _context.SaveChangesAsync();
            var result = await _service.AcceptOfferAsync(existingOffer.Id);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Offer has already been considered.", badRequestResult.Value);
        }

        [Fact]
        public async Task AcceptOfferAsync_OfferPending_AcceptOffer()
        {
            var existingOffer = new NegotiationOffer { Id = 1, Status = "Pending", OfferDetails = "testOffer", SenderUsername = "testUser" };
            _context.NegotiationOffers.Add(existingOffer);
            await _context.SaveChangesAsync();
            var result = await _service.AcceptOfferAsync(existingOffer.Id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Offer accepted.", okResult.Value);
        }

        [Fact]
        public async Task GetNegotiationOffersAsync_ReturnsOffers()
        {
            var offers = new List<NegotiationOffer>
            {
                new NegotiationOffer { Id = 1, Status = "Pending", OfferDetails = "testOffer", SenderUsername = "testUser" },
                new NegotiationOffer { Id = 2, Status = "Pending", OfferDetails = "testOffer", SenderUsername = "testUser" },
                new NegotiationOffer { Id = 3, Status = "Pending", OfferDetails = "testOffer", SenderUsername = "testUser" }
            };
            _context.NegotiationOffers.AddRange(offers);
            await _context.SaveChangesAsync();
            var result = await _service.GetNegotiationOffersAsync();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultOffers = Assert.IsType<List<NegotiationOffer>>(okResult.Value);
            Assert.Equal(offers.Count, resultOffers.Count);
        }
        }
}
