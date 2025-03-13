using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public interface INegotiationService
{
    Task<IActionResult> GetNegotiationOffersAsync();
    Task<IActionResult> RejectOfferAsync(int id);
    Task<IActionResult> AcceptOfferAsync(int id);
}
