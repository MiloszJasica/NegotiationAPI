using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class NegotiationOfferController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public NegotiationOfferController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("send")]
    public IActionResult SendOffer([FromBody] NegotiationOffer offer)
    {
        var product = _context.Products.Find(offer.ProductId);
        if (product == null)
        {
            return NotFound("Product does not exist.");
        }

        var existingOffers = _context.NegotiationOffers
            .Where(o => o.SenderUsername == offer.SenderUsername && o.CreatedAt > DateTime.UtcNow.AddDays(-7))
            .ToList();

        if (existingOffers.Count >= 3)
        {
            return BadRequest("You can send a maximum of 3 offers within 7 days.");
        }

        offer.Status = "Pending";
        offer.CreatedAt = DateTime.UtcNow;
        _context.NegotiationOffers.Add(offer);
        _context.SaveChanges();

        return Ok("Offer has been sent.");
    }

}
