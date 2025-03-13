using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class NegotiationController : ControllerBase
{
    private readonly INegotiationService _negotiationService;

    public NegotiationController(INegotiationService negotiationService)
    {
        _negotiationService = negotiationService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetNegotiationOffers()
    {
        return await _negotiationService.GetNegotiationOffersAsync();
    }

    [HttpPost("reject/{id}")]
    [Authorize]
    public async Task<IActionResult> RejectOffer(int id)
    {
        return await _negotiationService.RejectOfferAsync(id);
    }

    [HttpPost("accept/{id}")]
    [Authorize]
    public async Task<IActionResult> AcceptOffer(int id)
    {
        return await _negotiationService.AcceptOfferAsync(id);
    }
}
