using CreditService.Common.DTO;
using CreditService.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[Route("loan/")]
[ApiController]
public class LoanAppController:ControllerBase
{
    private readonly ILoanService _loanService;

    public LoanAppController(ILoanService loanService)
    {
        _loanService = loanService;
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
        var ansLoanAppDto = await _loanService.SendLoanApp(userId,sendLoanAppDto);
        
        return Ok(ansLoanAppDto);
    }

}