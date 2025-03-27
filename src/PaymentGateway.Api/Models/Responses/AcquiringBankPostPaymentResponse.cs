using Newtonsoft.Json;

namespace PaymentGateway.Api.Models.Responses;

public class AcquiringBankPostPaymentResponse
{
    [JsonProperty("authorized")]
    public bool Authorized { get; set; }
    [JsonProperty("authorization_code")]
    public Guid? AuthorizationCode { get; set; }
}