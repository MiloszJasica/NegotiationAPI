using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace NegotiationAPI.ControllersTests
{
    public class NegotiationControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly NegotiationService _service;

        public NegotiationControllerTests()
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
            var pendingOffer = new NegotiationOffer { Id = 1, Status = "Pending", OfferDetails = "testOffer", SenderUsername = "testUser" };
            _context.NegotiationOffers.Add(pendingOffer);
            await _context.SaveChangesAsync();
            var result = await _service.AcceptOfferAsync(pendingOffer.Id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Offer accepted.", okResult.Value);
        }

        [Fact]
        public async Task GetNegotiationOffersAsync_ReturnsOffers()
        {
            _context.NegotiationOffers.AddRange(new List<NegotiationOffer>
            {
                new NegotiationOffer { Id = 1, Status = "Pending", OfferDetails = "testOffer1", SenderUsername = "testUser1" },
                new NegotiationOffer { Id = 2, Status = "Pending", OfferDetails = "testOffer2", SenderUsername = "testUser2" }
            });
            await _context.SaveChangesAsync();
            var result = await _service.GetNegotiationOffersAsync();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var offers = Assert.IsType<List<NegotiationOffer>>(okResult.Value);
            Assert.Equal(2, offers.Count);
        }

        [Fact]
        public async Task GetNegotiationOffersAsync_NoOffers_ReturnsEmptyList()
        {
            var result = await _service.GetNegotiationOffersAsync();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var offers = Assert.IsType<List<NegotiationOffer>>(okResult.Value);
            Assert.Empty(offers);
        }

        [Fact]
        public async Task GetNegotiationOffersAsync_NoPendingOffers_ReturnsEmptyList()
        {
            _context.NegotiationOffers.AddRange(new List<NegotiationOffer>
            {
                new NegotiationOffer { Id = 1, Status = "Accepted", OfferDetails = "testOffer1", SenderUsername = "testUser1" },
                new NegotiationOffer { Id = 2, Status = "Rejected", OfferDetails = "testOffer2", SenderUsername = "testUser2" }
            });
            await _context.SaveChangesAsync();
            var result = await _service.GetNegotiationOffersAsync();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var offers = Assert.IsType<List<NegotiationOffer>>(okResult.Value);
            Assert.Empty(offers);
        }

        [Fact]
        public async Task RejectOfferAsync_OfferNotFound_ReturnsNotFound()
        {
            var result = await _service.RejectOfferAsync(1);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Offer does not exist.", notFoundResult.Value);
        }

        [Fact]
        public async Task AcceptOfferAsync_OfferNotFound_ReturnsNotFound()
        {
            var result = await _service.AcceptOfferAsync(1);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Offer does not exist.", notFoundResult.Value);
        }

        [Fact]
        public async Task RejectOfferAsync_OfferAlreadyRejected_ReturnsBadRequest()
        {
            var rejectedOffer = new NegotiationOffer { Id = 1, Status = "Rejected", OfferDetails = "testOffer", SenderUsername = "testUser" };
            _context.NegotiationOffers.Add(rejectedOffer);
            await _context.SaveChangesAsync();
            var result = await _service.RejectOfferAsync(rejectedOffer.Id);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Offer has already been considered.", badRequestResult.Value);
        }

        [Fact]
        public async Task AcceptOfferAsync_OfferAlreadyAccepted_ReturnsBadRequest()
        {
            var acceptedOffer = new NegotiationOffer { Id = 1, Status = "Accepted", OfferDetails = "testOffer", SenderUsername = "testUser" };
            _context.NegotiationOffers.Add(acceptedOffer);
            await _context.SaveChangesAsync();
            var result = await _service.AcceptOfferAsync(acceptedOffer.Id);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Offer has already been considered.", badRequestResult.Value);
        }
    }
}
