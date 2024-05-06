using CreditService.BL.Http;
using CreditService.BL.Services;
using CreditService.Common.DTO;
using CreditService.Common.Exceptions;
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
    private readonly ThrowException _exception;

    public CreditController(ICreditService creditService,LoggerService logger, ThrowException exception)
    {
        _creditService = creditService;
        _logger = logger;
        _exception = exception;

    }
    
    [HttpGet("all")]
    public async Task<ActionResult<List<CreditDto>>> GetAllCreditRules()
    {
        if (!_exception.GenerateRandomValue())   return StatusCode(500);
        if (!HttpContext.Request.Headers.TryGetValue("RequestId", out var requestId))
        {
            throw new IncorrectDataException("не передан requestId");
        }

        if (!HttpContext.Request.Headers.TryGetValue("DeviceId", out var deviceId))
        {
            throw new IncorrectDataException("не передан deviceId");
        }
        var creditRules = await _creditService.GetAllCreditRules();
        return Ok(creditRules);

    }
    [HttpGet("{id}")]
    public async Task<ActionResult<CreditDto>> GetCreditRule(Guid id)
    {
        if (!_exception.GenerateRandomValue())  return StatusCode(500);
        if (!HttpContext.Request.Headers.TryGetValue("RequestId", out var requestId))
        {
            throw new IncorrectDataException("не передан requestId");
        }

        if (!HttpContext.Request.Headers.TryGetValue("DeviceId", out var deviceId))
        {
            throw new IncorrectDataException("не передан deviceId");
        }
        var creditRule = await _creditService.GetCreditRule(id);
        return Ok(creditRule);

    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<string>> EditCreditRule(Guid id, AddCreditRuleDto updateCreditDto)
    {
        if (!_exception.GenerateRandomValue()) return StatusCode(500);
        if (!HttpContext.Request.Headers.TryGetValue("RequestId", out var requestId))
        {
            throw new IncorrectDataException("не передан requestId");
        }

        if (!HttpContext.Request.Headers.TryGetValue("DeviceId", out var deviceId))
        {
            throw new IncorrectDataException("не передан deviceId");
        }
        await _creditService.EditCreditRule(id, updateCreditDto);

        return Ok("Success update");

    }
    
    [HttpPost("")]
    public async Task<ActionResult<string>> AddCreditRule(AddCreditRuleDto addCreditDto)
    {
        if (!_exception.GenerateRandomValue())  return StatusCode(500);
        if (!HttpContext.Request.Headers.TryGetValue("RequestId", out var requestId))
        {
            throw new IncorrectDataException("не передан requestId");
        }

        if (!HttpContext.Request.Headers.TryGetValue("DeviceId", out var deviceId))
        {
            throw new IncorrectDataException("не передан deviceId");
        }
        var http = HttpContext;
        await _creditService.AddCreditRule(addCreditDto);
        await _logger.Log(http);
        return Ok("Success created");

    }    
    [HttpDelete("{id}")]
    public async Task<ActionResult<string>> DeleteCreditRule(Guid id)
    {
        if (!_exception.GenerateRandomValue()) return StatusCode(500);
        if (!HttpContext.Request.Headers.TryGetValue("RequestId", out var requestId))
        {
            throw new IncorrectDataException("не передан requestId");
        }

        if (!HttpContext.Request.Headers.TryGetValue("DeviceId", out var deviceId))
        {
            throw new IncorrectDataException("не передан deviceId");
        }
        await _creditService.DeleteCreditRule(id);
        return Ok("Success deleted");

    }
}