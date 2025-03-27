using PaymentGateway.Api.Models.Repositories;

namespace PaymentGateway.Api.Repositories;

public class PaymentsRepository : IPaymentsRepository
{
    public List<Payment> Payments { get; } = new();

    public Payment? Get(Guid id)
    {
        return Payments.FirstOrDefault(p => p.Id == id);
    }

    public void Add(Payment payment)
    {
        Payments.Add(payment);
    }
}
