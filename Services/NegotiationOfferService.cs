using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class NegotiationOfferService : INegotiationOfferService
{
    private readonly ApplicationDbContext _context;

    public NegotiationOfferService(ApplicationDbContext context)
    {
        _context = context;
    }

    public NegotiationOfferService()
    {
    }

    public async Task<IActionResult> SendOfferAsync(NegotiationOffer offer)
    {
        var senderUsername = offer.SenderUsername;
        var productId = offer.ProductId;

        var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
        if (!productExists)
        {
            return new NotFoundObjectResult("Product does not exist.");
        }

        var lastRejectedOffer = await _context.NegotiationOffers
            .Where(o => o.SenderUsername == senderUsername && o.ProductId == productId && o.Status == "Rejected")
            .OrderByDescending(o => o.RespondedAt)
            .FirstOrDefaultAsync();

        if (lastRejectedOffer != null && lastRejectedOffer.RespondedAt.HasValue &&
            lastRejectedOffer.RespondedAt.Value.AddDays(7) < DateTime.UtcNow)
        {
            return new BadRequestObjectResult("Negotiation for this product has expired. You cannot propose a new offer.");
        }

        var existingOffers = await _context.NegotiationOffers
            .Where(o => o.SenderUsername == senderUsername && o.ProductId == productId && o.CreatedAt > DateTime.UtcNow.AddDays(-7))
            .ToListAsync();

        if (existingOffers.Count >= 3)
        {
            return new BadRequestObjectResult("You can send a maximum of 3 offers for this product within 7 days.");
        }

        offer.Status = "Pending";
        offer.CreatedAt = DateTime.UtcNow;
        _context.NegotiationOffers.Add(offer);
        await _context.SaveChangesAsync();

        return new OkObjectResult("Offer has been sent.");
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
