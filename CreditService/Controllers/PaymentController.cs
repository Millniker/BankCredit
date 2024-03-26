using CreditService.Common.DTO;
using CreditService.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[Route("payment/")]
[ApiController]
public class PaymentController:ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    
    [HttpPost("")]
    public async Task<ActionResult<string>> SendLoanApp(PaymentDto paymentDto)
    {
        await _paymentService.PaymentProcessing(paymentDto);

        return Ok("Success");
    }
}