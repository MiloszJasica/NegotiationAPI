using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class NegotiationOfferController : ControllerBase
{
    private readonly INegotiationOfferService _negotiationOfferService;

    public NegotiationOfferController(INegotiationOfferService negotiationOfferService)
    {
        _negotiationOfferService = negotiationOfferService;
    }

    [HttpPost]
    public async Task<IActionResult> SendOffer([FromBody] NegotiationOffer offer)
    {
        return await _negotiationOfferService.SendOfferAsync(offer);
    }
}

