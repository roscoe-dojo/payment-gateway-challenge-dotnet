using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using Moq;
using Moq.Protected;

using PaymentGateway.Api.HttpClients;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.UnitTests.HttpClientTests;

public class AcquiringBankHttpClientTests
{
    private HttpClient CreateHttpClient(HttpResponseMessage mockResponse)
    {
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(mockResponse);

        return new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://test-bank.com/")
        };
    }

    [Fact]
    public async Task PostAcquiringBankPaymentAsync_ReturnsDeserializedResponse_WhenSuccessful()
    {
        // Arrange
        var expectedResponse = new AcquiringBankPostPaymentResponse
        {
            Authorized = true,
            AuthorizationCode = Guid.NewGuid().ToString()
        };

        var httpClient = CreateHttpClient(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(expectedResponse)
        });

        var client = new AcquiringBankHttpClient(httpClient);

        var request = new AcquiringBankPostPaymentRequest
        {
            CardNumber = "4111111111111111",
            ExpiryDate = "12/2026",
            Currency = "GBP",
            Amount = 1000,
            Cvv = "123"
        };

        // Act
        var result = await client.PostAcquiringBankPaymentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Authorized.Should().BeTrue();
        result.AuthorizationCode.Should().Be(expectedResponse.AuthorizationCode);
    }

    [Fact]
    public async Task PostAcquiringBankPaymentAsync_ReturnsRejectedResponse_WhenNotSuccessful()
    {
        // Arrange
        var httpClient = CreateHttpClient(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest
        });

        var client = new AcquiringBankHttpClient(httpClient);

        var request = new AcquiringBankPostPaymentRequest
        {
            CardNumber = "4111111111111111",
            ExpiryDate = "12/2026",
            Currency = "GBP",
            Amount = 1000,
            Cvv = "123"
        };

        // Act
        var result = await client.PostAcquiringBankPaymentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Authorized.Should().BeFalse();
        result.AuthorizationCode.Should().BeNull();
    }
}
