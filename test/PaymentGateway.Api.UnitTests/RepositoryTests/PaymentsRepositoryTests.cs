using FluentAssertions;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Repositories;
using PaymentGateway.Api.Repositories;

namespace PaymentGateway.Api.UnitTests.RepositoryTests;

public class PaymentsRepositoryTests
{
    private readonly PaymentsRepository _repository;

    public PaymentsRepositoryTests()
    {
        _repository = new PaymentsRepository();
    }
    
    [Fact]
    public void Get_ReturnsPayment_WhenPaymentExists()
    {
        // Arrange
        var payment = new Payment { Id = Guid.NewGuid() };
        _repository.Payments.Add(payment);

        // Act
        var result = _repository.Get(payment.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(payment.Id);
    }

    [Fact]
    public void Get_ReturnsNull_WhenPaymentDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = _repository.Get(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Add_AddsPaymentToList()
    {
        // Arrange
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            CardNumberLastFour = "1234",
            Amount = 1000,
            Currency = "GBP",
            ExpiryMonth = 12,
            ExpiryYear = 2025,
            Status = PaymentStatus.Authorized
        };

        // Act
        _repository.Add(payment);

        // Assert
        _repository.Payments.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(payment);
    }
}