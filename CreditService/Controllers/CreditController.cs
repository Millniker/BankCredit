using CreditService.Common.DTO;
using CreditService.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;


[Route("credit/")]
[ApiController]
public class CreditController:ControllerBase
{

    private readonly ICreditService _creditService;

    public CreditController(ICreditService creditService)
    {
        _creditService = creditService;
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<CreditDto>>> GetAllCreditRules()
    {
        var creditRules = await _creditService.GetAllCreditRules();

        return Ok(creditRules);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<CreditDto>> GetCreditRule(Guid id)
    {
        var creditRule = await _creditService.GetCreditRule(id);

        return Ok(creditRule);
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<string>> EditCreditRule(Guid id, AddCreditRuleDto updateCreditDto)
    {
        await _creditService.EditCreditRule(id, updateCreditDto);

        return Ok("Success update");
    }
    
    [HttpPost("")]
    public async Task<ActionResult<string>> AddCreditRule(AddCreditRuleDto addCreditDto)
    {
        await _creditService.AddCreditRule(addCreditDto);

        return Ok("Success created");
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult<string>> DeleteCreditRule(Guid id)
    {
        await _creditService.DeleteCreditRule(id);

        return Ok("Success deleted");
    }
}