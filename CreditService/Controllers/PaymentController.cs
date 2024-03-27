using CreditService.Common.DTO.Payment;
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
    public async Task<ActionResult<string>> SendLoanApp(SendPaymentDto paymentDto)
    {
        await _paymentService.PaymentProcessing(paymentDto);

        return Ok("Success");
    }
    [HttpGet("/bills/{accountId}")]
    public async Task<ActionResult<List<BillPaymentDTO>>> GetBillPayment(int accountId)
    {
        return Ok(await _paymentService.GetBillPayment(accountId));
    }

    [HttpGet("/score/{userId}")]
    public async Task<ActionResult<Int32>> GetCreditScore(string userId)
    {
        return Ok(await _paymentService.GetCreditScore(userId));
    }
}