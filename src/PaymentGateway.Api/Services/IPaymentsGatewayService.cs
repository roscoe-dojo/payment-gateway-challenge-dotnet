using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public interface IPaymentsGatewayService
{
    GetPaymentResponse? GetPayment(Guid id);
    Task<PostPaymentResponse> PostPaymentAsync(PostPaymentRequest paymentRequest);
}