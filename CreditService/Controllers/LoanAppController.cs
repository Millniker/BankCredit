using CreditService.BL.Services;
using CreditService.Common.DTO;
using CreditService.Common.Exceptions;
using CreditService.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[Route("loan/")]
[ApiController]
public class LoanAppController:ControllerBase
{
    private readonly ILoanService _loanService;
    private readonly LoggerService _logger;


    public LoanAppController(ILoanService loanService, LoggerService logger)
    {
        _loanService = loanService;
        _logger = logger;
    }

    [HttpGet("loanApp/{userId}/all")]
    public async Task<ActionResult<List<LoanAppDto>>> GetAllUserLoanApps(int userId)
    {
        var loanApps = await _loanService.GetAllLoanAppDtoByUserId(userId);
        return Ok(loanApps);
    }
    
    [HttpGet("loanApp/{loanAppId}")]
    public async Task<ActionResult<List<LoanAppDto>>> GetLoanApp(Guid loanAppId)
    {
        var loanApps = await _loanService.GetLoanAppDto(loanAppId);
        return Ok(loanApps);
    }
    
    [HttpGet("{userId}/all")]
    public async Task<ActionResult<List<LoanDto>>> GetAllUserLoan(int userId)
    {
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
        var http = HttpContext;

        var ansLoanAppDto = await _loanService.SendLoanApp(userId,sendLoanAppDto, requestId, deviceId);
        _logger.Log(http);
        return Ok(ansLoanAppDto);
    }

}