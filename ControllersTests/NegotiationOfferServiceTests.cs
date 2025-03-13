using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class NegotiationOfferServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly NegotiationOfferService _service;

    public NegotiationOfferServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "NegotiationOfferServiceTests")
            .Options;
        _context = new ApplicationDbContext(options);
        _service = new NegotiationOfferService(_context);
    }

    [Fact]
    public async Task SendOfferAsync_ProductDoesNotExist_ReturnsNotFound()
    {
        var offer = new NegotiationOffer { Id = 1, Status = "Pending", OfferDetails = "testOffer", SenderUsername = "testUser" };
        var result = await _service.SendOfferAsync(offer);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Product does not exist.", notFoundResult.Value);
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
        var existingOffer = new NegotiationOffer
        {
            Status = "Accepted",
            OfferDetails = "testOffer",
            SenderUsername = "testUser"
        };

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
}
