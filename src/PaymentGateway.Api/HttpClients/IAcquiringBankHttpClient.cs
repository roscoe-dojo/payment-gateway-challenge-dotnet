using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.HttpClients;

public interface IAcquiringBankHttpClient
{
    Task<AcquiringBankPostPaymentResponse?> PostAcquiringBankPaymentAsync(AcquiringBankPostPaymentRequest request);
}