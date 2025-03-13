using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class NegotiationService : INegotiationService
{
    private readonly ApplicationDbContext _context;

    public NegotiationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> GetNegotiationOffersAsync()
    {
        var offers = await _context.NegotiationOffers
            .Where(o => o.Status == "Pending")
            .ToListAsync();

        return new OkObjectResult(offers);
    }

    public async Task<IActionResult> RejectOfferAsync(int id)
    {
        var offer = await _context.NegotiationOffers.FindAsync(id);
        if (offer == null) return new NotFoundObjectResult("Offer does not exist.");
        if (offer.Status != "Pending") return new BadRequestObjectResult("Offer has already been considered.");

        offer.Status = "Rejected";
        offer.RespondedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new OkObjectResult("Offer rejected.");
    }

    public async Task<IActionResult> AcceptOfferAsync(int id)
    {
        var offer = await _context.NegotiationOffers.FindAsync(id);
        if (offer == null) return new NotFoundObjectResult("Offer does not exist.");
        if (offer.Status != "Pending") return new BadRequestObjectResult("Offer has already been considered.");

        offer.Status = "Accepted";
        offer.RespondedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new OkObjectResult("Offer accepted.");
    }
}
