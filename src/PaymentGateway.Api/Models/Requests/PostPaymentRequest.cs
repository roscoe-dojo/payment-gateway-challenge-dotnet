using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Requests;

public class PostPaymentRequest
{
    [JsonPropertyName("card_number")]
    public string CardNumber { get; set; }
    [JsonPropertyName("expiry_month")]
    public int ExpiryMonth { get; set; }
    [JsonPropertyName("expiry_year")]
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
    public string Cvv { get; set; }
}