using FluentValidation;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly IValidator<PostPaymentRequest> _postPaymentRequestValidator;
    private readonly IPaymentsGatewayService _paymentsGatewayService;

    public PaymentsController(
        IValidator<PostPaymentRequest> postPaymentRequestValidator,
        IPaymentsGatewayService paymentsGatewayService)
    {
        _postPaymentRequestValidator = postPaymentRequestValidator;
        _paymentsGatewayService = paymentsGatewayService;
    }
    
    [HttpGet("{id:guid}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<GetPaymentResponse?> GetPaymentAsync(
        [FromRoute] Guid id)
    {
        var getPaymentResponse = _paymentsGatewayService.GetPayment(id);
        
        if (getPaymentResponse == null) return NotFound("Payment not found");

        return new OkObjectResult(getPaymentResponse);
    }
    
    [HttpPost]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PostPaymentResponse>> PostPaymentAsync(
        [FromBody] PostPaymentRequest postPaymentRequest)
    {
        var validationResult = await _postPaymentRequestValidator.ValidateAsync(postPaymentRequest);
        
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

        var postPaymentResponse = await _paymentsGatewayService.PostPaymentAsync(postPaymentRequest);
        
        return new OkObjectResult(postPaymentResponse);
    }
}