using PaymentGateway.Api.HttpClients;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.IntegrationTests.TestInfrastructure.MockedHttpClients;

public class MockAcquiringBankHttpClient : IAcquiringBankHttpClient
{
    public Task<AcquiringBankPostPaymentResponse?> PostAcquiringBankPaymentAsync(AcquiringBankPostPaymentRequest request)
    {
        return Task.FromResult<AcquiringBankPostPaymentResponse?>(new AcquiringBankPostPaymentResponse
        {
            Authorized = true,
            AuthorizationCode = Guid.NewGuid()
        });
    }
}
