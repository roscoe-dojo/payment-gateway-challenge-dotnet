using FluentAssertions;

using Moq;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.HttpClients;
using PaymentGateway.Api.Models.Repositories;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.UnitTests.ServiceTests;

public class PaymentsGatewayServiceTests
{
    private readonly Mock<IPaymentsRepository> _paymentsRepositoryMock = new();
    private readonly Mock<IAcquiringBankHttpClient> _acquiringBankHttpClientMock = new();
    private readonly PaymentsGatewayService _service;

    public PaymentsGatewayServiceTests()
    {
        _service = new PaymentsGatewayService(
            _paymentsRepositoryMock.Object,
            _acquiringBankHttpClientMock.Object);
    }
    
    [Fact]
    public void GetPayment_ReturnsMappedResponse_WhenPaymentExists()
    {
        var id = Guid.NewGuid();
        var doc = new Payment
        {
            Id = id,
            Status = PaymentStatus.Declined,
            CardNumberLastFour = "5678",
            ExpiryMonth = 4,
            ExpiryYear = 2025,
            Currency = "GBP",
            Amount = 1000
        };

        _paymentsRepositoryMock.Setup(r => r.Get(id)).Returns(doc);

        var result = _service.GetPayment(id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Status.Should().Be(PaymentStatus.Declined);
        result.CardNumberLastFour.Should().Be("5678");
        result.ExpiryMonth.Should().Be(4);
        result.ExpiryYear.Should().Be(2025);
        result.Currency.Should().Be("GBP");
        result.Amount.Should().Be(1000);
    }

    [Fact]
    public void GetPayment_ReturnsNull_WhenPaymentNotFound()
    {
        var id = Guid.NewGuid();
        _paymentsRepositoryMock.Setup(r => r.Get(id)).Returns((Payment)null);

        var result = _service.GetPayment(id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task PostPaymentAsync_ThrowsException_WhenBankResponseIsNull()
    {
        var request = new PostPaymentRequest
        {
            CardNumber = "1234567890123456",
            ExpiryMonth = 4,
            ExpiryYear = 2025,
            Currency = "EUR",
            Amount = 500,
            Cvv = "123"
        };

        _acquiringBankHttpClientMock
            .Setup(c => c.PostAcquiringBankPaymentAsync(It.IsAny<AcquiringBankPostPaymentRequest>()))
            .ReturnsAsync((AcquiringBankPostPaymentResponse)null);

        await Assert.ThrowsAsync<Exception>(() => _service.PostPaymentAsync(request));
    }

    [Theory]
    [InlineData(null, false, PaymentStatus.Rejected)]
    [InlineData("00000000-0000-0000-0000-000000000001", true, PaymentStatus.Authorized)]
    [InlineData("00000000-0000-0000-0000-000000000002", false, PaymentStatus.Declined)]
    public async Task PostPaymentAsync_ReturnsExpectedStatusBasedOnBankResponse(
        string? authCodeString,
        bool authorized,
        PaymentStatus expectedStatus)
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "9999888877776666",
            ExpiryMonth = 11,
            ExpiryYear = 2030,
            Currency = "USD",
            Amount = 750,
            Cvv = "000"
        };

        Guid? authCode = authCodeString != null ? Guid.Parse(authCodeString) : (Guid?)null;

        var bankResponse = new AcquiringBankPostPaymentResponse
        {
            AuthorizationCode = authCode,
            Authorized = authorized
        };

        _acquiringBankHttpClientMock
            .Setup(c => c.PostAcquiringBankPaymentAsync(It.IsAny<AcquiringBankPostPaymentRequest>()))
            .ReturnsAsync(bankResponse);

        // Act
        var result = await _service.PostPaymentAsync(request);

        // Assert
        result.Status.Should().Be(expectedStatus);
        result.Id.Should().Be(authCode);
        result.CardNumberLastFour.Should().Be("6666");
        result.ExpiryMonth.Should().Be(11);
        result.ExpiryYear.Should().Be(2030);
        result.Currency.Should().Be("USD");
        result.Amount.Should().Be(750);
    }
}
