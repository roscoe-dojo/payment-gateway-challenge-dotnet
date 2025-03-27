using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Mvc;

using Moq;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.UnitTests.ControllerTests
{
    public class PaymentsControllerTests
    {
        private readonly Mock<IValidator<PostPaymentRequest>> _validatorMock;
        private readonly Mock<IPaymentsGatewayService> _paymentsServiceMock;
        private readonly PaymentsController _controller;

        public PaymentsControllerTests()
        {
            _validatorMock = new Mock<IValidator<PostPaymentRequest>>();
            _paymentsServiceMock = new Mock<IPaymentsGatewayService>();
            _controller = new PaymentsController(_validatorMock.Object, _paymentsServiceMock.Object);
        }

        [Fact]
        public void GetPaymentAsync_ReturnsOk_WhenPaymentExists()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var expectedResponse = new GetPaymentResponse { Id = paymentId };

            _paymentsServiceMock.Setup(s => s.GetPayment(paymentId)).Returns(expectedResponse);

            // Act
            var result = _controller.GetPaymentAsync(paymentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedResponse, okResult.Value);
        }

        [Fact]
        public void GetPaymentAsync_ReturnsNotFound_WhenPaymentDoesNotExist()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            _paymentsServiceMock.Setup(s => s.GetPayment(paymentId)).Returns((GetPaymentResponse)null);

            // Act
            var result = _controller.GetPaymentAsync(paymentId);

            // Assert
            var actionResult = result.Result;
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
            Assert.Equal("Payment not found", notFoundResult.Value);
        }

        [Fact]
        public async Task PostPaymentAsync_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var request = new PostPaymentRequest();
            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("CardNumber", "Card number is required")
            });

            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.PostPaymentAsync(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(validationResult.Errors, badRequestResult.Value);
        }

        [Fact]
        public async Task PostPaymentAsync_ReturnsOk_WhenValidationPassesAndPaymentProcessed()
        {
            // Arrange
            var request = new PostPaymentRequest { CardNumber = "1234567812345678" };
            var validationResult = new ValidationResult();
            var authorizationId = Guid.NewGuid().ToString();
            var response = new PostPaymentResponse { Id = authorizationId, Status = PaymentStatus.Authorized };

            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            _paymentsServiceMock.Setup(s => s.PostPaymentAsync(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.PostPaymentAsync(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, okResult.Value);
        }
    }
}
