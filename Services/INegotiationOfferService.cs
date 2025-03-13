using Microsoft.AspNetCore.Mvc;

public interface INegotiationOfferService
{
    Task<IActionResult> SendOfferAsync(NegotiationOffer offer);
    Task<IActionResult> RejectOfferAsync(int id);
    Task<IActionResult> AcceptOfferAsync(int id);
}
