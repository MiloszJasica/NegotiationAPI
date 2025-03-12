using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class NegotiationController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public NegotiationController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize]
    public IActionResult GetNegotiationOffers()
    {
        var offers = _context.NegotiationOffers
            .Where(o => o.Status == "Pending")
            .ToList();

        return Ok(offers);
    }

    [HttpPost("reject/{id}")]
    [Authorize]
    public IActionResult RejectOffer(int id)
    {
        var offer = _context.NegotiationOffers.Find(id);
        if (offer == null) return NotFound("Offer does not exist.");
        if (offer.Status != "Pending") return BadRequest("Offer has already been considered.");

        offer.Status = "Rejected";
        offer.RespondedAt = DateTime.UtcNow;
        _context.SaveChanges();

        return Ok("Offer rejected.");
    }

    [HttpPost("accept/{id}")]
    [Authorize]
    public IActionResult AcceptOffer(int id)
    {
        var offer = _context.NegotiationOffers.Find(id);
        if (offer == null) return NotFound("Oferta does not exist.");
        if (offer.Status != "Pending") return BadRequest("Offer has already been considered.");

        offer.Status = "Accepted";
        offer.RespondedAt = DateTime.UtcNow;
        _context.SaveChanges();

        return Ok("Offer accepted.");
    }
}
