using Newtonsoft.Json;

namespace PaymentGateway.Api.Models.Requests;

public class AcquiringBankPostPaymentRequest
{
    [JsonProperty("card_number")]
    public string CardNumber { get; set; }
    [JsonProperty("expiry_date")]
    public string ExpiryDate { get; set; }
    [JsonProperty("currency")]
    public string Currency  { get; set; }
    [JsonProperty("amount")]
    public int Amount  { get; set; }
    [JsonProperty("cvv")]
    public string Cvv { get; set; }
}