using System.Net;
using System.Net.Http.Json;
using System.Reflection;

using FluentAssertions;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.IntegrationTests.TestInfrastructure;
using PaymentGateway.Api.Models.Repositories;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.IntegrationTests.Scenarios;

public class PaymentsControllerTests : IClassFixture<TestServer>
{
    private readonly TestServer _testServer;

    public PaymentsControllerTests(TestServer testServer)
    {
        _testServer = testServer;
        ResetInMemoryRepository();
    }

    [Fact]
    public async Task PostPayment_Should_Return200AndCreatePayment()
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "12345678901234",
            ExpiryMonth = 12,
            ExpiryYear = DateTime.UtcNow.Year + 1,
            Currency = "GBP",
            Amount = 500,
            Cvv = "123"
        };

        // Act
        var response = await _testServer.Client.PostAsJsonAsync("/api/payments", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        result.Should().NotBeNull();
        result!.Status.Should().Be(PaymentStatus.Authorized);
        result.CardNumberLastFour.Should().Be("1234");
    }

    [Fact]
    public async Task GetPayment_Should_Return200_When_ValidId()
    {
        // Arrange
        var paymentId = Guid.NewGuid();

        _testServer.PaymentsRepository.Add(new Payment
        {
            Id = paymentId,
            CardNumberLastFour = "8888",
            ExpiryMonth = 10,
            ExpiryYear = DateTime.UtcNow.Year + 1,
            Currency = "EUR",
            Amount = 200,
            Status = PaymentStatus.Authorized
        });

        // Act
        var getResponse = await _testServer.Client.GetAsync($"/api/payments/{paymentId}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResult = await getResponse.Content.ReadFromJsonAsync<GetPaymentResponse>();
        getResult.Should().NotBeNull();
        getResult!.Id.Should().Be(paymentId);
        getResult.CardNumberLastFour.Should().Be("8888");
        getResult.ExpiryMonth.Should().Be(10);
        getResult.ExpiryYear.Should().Be(DateTime.UtcNow.Year + 1);
        getResult.Currency.Should().Be("EUR");
        getResult.Amount.Should().Be(200);
    }

    [Fact]
    public async Task GetPayment_Should_Return404_When_NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _testServer.Client.GetAsync($"/api/payments/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Payment not found");
    }
    
    private void ResetInMemoryRepository()
    {
        var repo = _testServer.PaymentsRepository;

        var field = repo.GetType()
            .GetField("_payments", BindingFlags.NonPublic | BindingFlags.Instance);

        if (field?.GetValue(repo) is List<Payment> payments)
        {
            payments.Clear();
        }
    }
}