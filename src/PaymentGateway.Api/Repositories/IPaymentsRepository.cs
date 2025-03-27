using PaymentGateway.Api.Models.Repositories;

namespace PaymentGateway.Api.Repositories;

public interface IPaymentsRepository
{
    public Payment? Get(Guid id);
    public void Add(Payment payment);
}