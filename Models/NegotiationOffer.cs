public class NegotiationOffer
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public decimal ProposedPrice { get; set; }
    public string OfferDetails { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
    public string SenderUsername { get; set; }
    public string Status { get; set; }
}
