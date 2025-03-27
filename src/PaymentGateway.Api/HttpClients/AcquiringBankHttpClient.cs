using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.HttpClients;

public class AcquiringBankHttpClient : IAcquiringBankHttpClient
{
    private readonly HttpClient _httpClient;
    private const string postPaymentEndpoint = "payments";

    public AcquiringBankHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AcquiringBankPostPaymentResponse?> PostAcquiringBankPaymentAsync(AcquiringBankPostPaymentRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(postPaymentEndpoint, request);

        if (!response.IsSuccessStatusCode)
        {
            return new AcquiringBankPostPaymentResponse
            {
                Authorized = false,
                AuthorizationCode = null
            };
        }
        
        return await response.Content.ReadFromJsonAsync<AcquiringBankPostPaymentResponse>();
    }
}