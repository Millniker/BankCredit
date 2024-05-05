using CreditService.BL.Http;
using CreditService.BL.Services;
using CreditService.Common.DTO;
using CreditService.Common.Interfaces;
using CreditService.Common.System;
using CreditService.DAL;
using Microsoft.AspNetCore.Mvc;

namespace CreditService.Controllers;


[Route("credit/")]
[ApiController]
public class CreditController:ControllerBase
{

    private readonly ICreditService _creditService;
    private readonly LoggerService _logger;
    private readonly MetricHttp _metric;

    public CreditController(ICreditService creditService,LoggerService logger, MetricHttp metric)
    {
        _creditService = creditService;
        _logger = logger;
        _metric = metric;
        
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
        var http = HttpContext;
        await _creditService.AddCreditRule(addCreditDto);
            _logger.Log(http);
            return Ok("Success created");
    }    
    [HttpDelete("{id}")]
    public async Task<ActionResult<string>> DeleteCreditRule(Guid id)
    {
        await _creditService.DeleteCreditRule(id);
        return Ok("Success deleted");
    }
}