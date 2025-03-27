using FluentValidation.TestHelper;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Validators;

namespace PaymentGateway.Api.UnitTests.ValidatorTests;

public class PostPaymentRequestValidatorTests
{
    private readonly PostPaymentRequestValidator _validator = new();

    private PostPaymentRequest CreateValidRequest(Action<PostPaymentRequest>? overrides = null)
    {
        var request = new PostPaymentRequest
        {
            CardNumber = "12345678901234",
            ExpiryMonth = 12,
            ExpiryYear = DateTime.UtcNow.Year + 1,
            Currency = "GBP",
            Amount = 100,
            Cvv = "123"
        };

        overrides?.Invoke(request);
        return request;
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("abcd567890")]
    public void Invalid_CardNumber_Should_Fail(string cardNumber)
    {
        var request = CreateValidRequest(r => r.CardNumber = cardNumber);
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.CardNumber);
    }

    [Theory]
    [InlineData("12345678901234")]
    [InlineData("1234567890123456789")]
    public void Valid_CardNumber_Should_Pass(string cardNumber)
    {
        var request = CreateValidRequest(r => r.CardNumber = cardNumber);
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.CardNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void Invalid_ExpiryMonth_Should_Fail(int expiryMonth)
    {
        var request = CreateValidRequest(r => r.ExpiryMonth = expiryMonth);
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ExpiryMonth);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(12)]
    public void Valid_ExpiryMonth_Should_Pass(int expiryMonth)
    {
        var request = CreateValidRequest(r => r.ExpiryMonth = expiryMonth);
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.ExpiryMonth);
    }

    [Fact]
    public void ExpiryYear_Should_Fail_When_In_Past()
    {
        var now = DateTime.UtcNow;
        var request = CreateValidRequest(r =>
        {
            r.ExpiryMonth = now.Month;
            r.ExpiryYear = now.Year - 1;
        });

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ExpiryYear);
    }

    [Fact]
    public void ExpiryYear_Should_Pass_When_In_Future()
    {
        var request = CreateValidRequest(r =>
        {
            r.ExpiryMonth = 1;
            r.ExpiryYear = DateTime.UtcNow.Year + 1;
        });

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.ExpiryYear);
    }

    [Theory]
    [InlineData("USD")]
    [InlineData("")]
    [InlineData("XYZ")]
    public void Invalid_Currency_Should_Fail(string currency)
    {
        var request = CreateValidRequest(r => r.Currency = currency);
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Theory]
    [InlineData("GBP")]
    [InlineData("EUR")]
    [InlineData("HUF")]
    public void Valid_Currency_Should_Pass(string currency)
    {
        var request = CreateValidRequest(r => r.Currency = currency);
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Currency);
    }

    [Fact]
    public void Amount_Should_Fail_When_Zero()
    {
        var request = CreateValidRequest(r => r.Amount = 0);
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Amount_Should_Pass_When_GreaterThanZero()
    {
        var request = CreateValidRequest(r => r.Amount = 100);
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Amount);
    }

    [Theory]
    [InlineData("ab1")]
    [InlineData("12")]
    [InlineData("12345")]
    public void Invalid_Cvv_Should_Fail(string cvv)
    {
        var request = CreateValidRequest(r => r.Cvv = cvv);
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Cvv);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("000")]
    [InlineData("1234")]
    public void Valid_Cvv_Should_Pass(string cvv)
    {
        var request = CreateValidRequest(r => r.Cvv = cvv);
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Cvv);
    }
}
