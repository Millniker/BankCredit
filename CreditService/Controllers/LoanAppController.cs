using CreditService.BL.Services;
using CreditService.Common.DTO;
using CreditService.Common.Exceptions;
using CreditService.Common.Interfaces;
using CreditService.Common.System;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[Route("loan/")]
[ApiController]
public class LoanAppController:ControllerBase
{
    private readonly ILoanService _loanService;
    private readonly LoggerService _logger;
    private readonly ThrowException _exception;



    public LoanAppController(ILoanService loanService, LoggerService logger, ThrowException exception)
    {
        _loanService = loanService;
        _logger = logger;
        _exception = exception;
    }

    [HttpGet("loanApp/{userId}/all")]
    public async Task<ActionResult<List<LoanAppDto>>> GetAllUserLoanApps(int userId)
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
        var loanApps = await _loanService.GetAllLoanAppDtoByUserId(userId);
        return Ok(loanApps);
    }
    
    [HttpGet("loanApp/{loanAppId}")]
    public async Task<ActionResult<List<LoanAppDto>>> GetLoanApp(Guid loanAppId)
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
        var loanApps = await _loanService.GetLoanAppDto(loanAppId);
        return Ok(loanApps);
    }
    
    [HttpGet("{userId}/all")]
    public async Task<ActionResult<List<LoanDto>>> GetAllUserLoan(int userId)
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
        var loan = await _loanService.GetAllLoanDtoByUserId(userId);
        return Ok(loan);
    }
    
    [HttpPost("send/{userId}")]
    public async Task<ActionResult<LoanResultResponse>> SendLoanApp(int userId, ShortLoanAppDto sendLoanAppDto)
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

        var http = HttpContext;

        var ansLoanAppDto = await _loanService.SendLoanApp(userId,sendLoanAppDto, requestId, deviceId);
        await _logger.Log(http);
        return Ok(ansLoanAppDto);
    }

}