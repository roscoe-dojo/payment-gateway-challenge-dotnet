using PaymentGateway.Api.Enums;
using PaymentGateway.Api.HttpClients;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repositories;

namespace PaymentGateway.Api.Services;

public class PaymentsGatewayService : IPaymentsGatewayService
{
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly IAcquiringBankHttpClient _acquiringBankHttpClient;

    public PaymentsGatewayService(IPaymentsRepository paymentsRepository, IAcquiringBankHttpClient acquiringBankHttpClient)
    {
        _paymentsRepository = paymentsRepository;
        _acquiringBankHttpClient = acquiringBankHttpClient;
    }
    
    public GetPaymentResponse? GetPayment(Guid id)
    {
        var paymentDoc = _paymentsRepository.Get(id);
        
        if (paymentDoc == null) return null;

        return new GetPaymentResponse
        {
            Id = paymentDoc.Id,
            Status = paymentDoc.Status,
            CardNumberLastFour = paymentDoc.CardNumberLastFour,
            ExpiryMonth = paymentDoc.ExpiryMonth,
            ExpiryYear = paymentDoc.ExpiryYear,
            Currency = paymentDoc.Currency,
            Amount = paymentDoc.Amount
        };
    }

    public async Task<PostPaymentResponse> PostPaymentAsync(PostPaymentRequest request)
    {
        var acquiringBankPaymentRequest = new AcquiringBankPostPaymentRequest
        {
            CardNumber = request.CardNumber,
            ExpiryDate = $"{request.ExpiryMonth:D2}/{request.ExpiryYear:D4}",
            Currency = request.Currency,
            Amount = request.Amount,
            Cvv = request.Cvv,
        };
        
        var acquiringBankresponse = await _acquiringBankHttpClient.PostAcquiringBankPaymentAsync(acquiringBankPaymentRequest);
        
        if (acquiringBankresponse == null)
        {
            throw new Exception("ReadFromJson failure in AcquiringBankClient");
        }
        
        var status = acquiringBankresponse.AuthorizationCode is null
            ? PaymentStatus.Rejected
            : acquiringBankresponse.Authorized
                ? PaymentStatus.Authorized
                : PaymentStatus.Declined;

        var postPaymentResponse = new PostPaymentResponse
        {
            Id = acquiringBankresponse.AuthorizationCode,
            Status = status,
            CardNumberLastFour = request.CardNumber[^4..],
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Currency = request.Currency,
            Amount = request.Amount
        };
        
        return postPaymentResponse;
    }
}