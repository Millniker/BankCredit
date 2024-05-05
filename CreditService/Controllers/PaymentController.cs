using CreditService.Common.DTO;
using CreditService.Common.DTO.Payment;
using CreditService.Common.Exceptions;
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
    {        if (!HttpContext.Request.Headers.TryGetValue("RequestId", out var requestId))
        {
            throw new IncorrectDataException("не передан requestId");
        }

        if (!HttpContext.Request.Headers.TryGetValue("DeviceId", out var deviceId))
        {
            throw new IncorrectDataException("не передан deviceId");
        }
        await _paymentService.PaymentProcessing(paymentDto,  requestId,  deviceId);

        return Ok("Success");
    }
    [HttpGet("/bills/{accountId}")]
    public async Task<ActionResult<List<BillPaymentDTO>>> GetBillPayment(int accountId)
    {
        return Ok(await _paymentService.GetBillPayment(accountId));
    }

    [HttpGet("/score/{userId}")]
    public async Task<ActionResult<CreditScoreDto>> GetCreditScore(int userId)
    {
        return Ok(await _paymentService.GetCreditScore(userId));
    }
}