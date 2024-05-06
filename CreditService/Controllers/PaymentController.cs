using CreditService.BL.Services;
using CreditService.Common.DTO;
using CreditService.Common.DTO.Payment;
using CreditService.Common.Exceptions;
using CreditService.Common.Interfaces;
using CreditService.Common.System;
using Microsoft.AspNetCore.Mvc;

namespace CreditService.Controllers;

[Route("payment/")]
[ApiController]
public class PaymentController:ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ThrowException _exception;
    private readonly LoggerService _logger;


    public PaymentController(IPaymentService paymentService,LoggerService logger, ThrowException exception)
    {
        _paymentService = paymentService;
        _logger = logger;
        _exception = exception;
    }
    
    [HttpPost("")]
    public async Task<ActionResult<string>> SendLoanApp(SendPaymentDto paymentDto)
    {        
        if (!HttpContext.Request.Headers.TryGetValue("RequestId", out var requestId))
        {
            throw new IncorrectDataException("не передан requestId");
        }

        if (!HttpContext.Request.Headers.TryGetValue("DeviceId", out var deviceId))
        {
            throw new IncorrectDataException("не передан deviceId");
        }
        if (!_exception.GenerateRandomValue())  return StatusCode(500);

        await _paymentService.PaymentProcessing(paymentDto,  requestId,  deviceId);
        var http = HttpContext;
        await _logger.Log(http);
        return Ok("Success");
    }
    [HttpGet("/bills/{accountId}")]
    public async Task<ActionResult<List<BillPaymentDTO>>> GetBillPayment(int accountId)
    {
        if (!HttpContext.Request.Headers.TryGetValue("RequestId", out var requestId))
        {
            throw new IncorrectDataException("не передан requestId");
        }

        if (!HttpContext.Request.Headers.TryGetValue("DeviceId", out var deviceId))
        {
            throw new IncorrectDataException("не передан deviceId");
        }
        if (!_exception.GenerateRandomValue())  return StatusCode(500);

        return Ok(await _paymentService.GetBillPayment(accountId));
    }

    [HttpGet("/score/{userId}")]
    public async Task<ActionResult<CreditScoreDto>> GetCreditScore(int userId)
    {
        if (!HttpContext.Request.Headers.TryGetValue("RequestId", out var requestId))
        {
            throw new IncorrectDataException("не передан requestId");
        }

        if (!HttpContext.Request.Headers.TryGetValue("DeviceId", out var deviceId))
        {
            throw new IncorrectDataException("не передан deviceId");
        }
        if (!_exception.GenerateRandomValue())  return StatusCode(500);

        return Ok(await _paymentService.GetCreditScore(userId));
    }
}