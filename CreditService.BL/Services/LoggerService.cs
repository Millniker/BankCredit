using System.Text;
using CreditService.DAL;
using CreditService.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CreditService.BL.Services;

public class LoggerService
{
    private readonly AppDbContext _context;

    public LoggerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task  Log(HttpContext context)    
    {
        var requestId = context.Request.Headers["RequestId"];
        var logEntry = new HttpExchangeData
        {
            Id = Guid.NewGuid(),
            IdempotencyId = Guid.Parse(requestId),
            RequestDate = DateTime.UtcNow,
            ResponseCode = context.Response.StatusCode,
            ResponseBody = await GetResponseBody(context),
            RequestPath = context.Request.Path,
            RequestMethod = context.Request.Method,
            RequestBody = await GetRequestBody(context),
            RequestHeaders = GetRequestHeaders(context),
            ResponseHeaders = GetResponseHeaders(context)
        };
        await _context.SaveChangesAsync();

        _context.HttpExchangeData.Add(logEntry);
        _context.IdempotencyId.Add(new IdempotencyId
        {
            Id = Guid.NewGuid(),
            CreateAt = DateTime.UtcNow,
            IdempotencyKey = requestId,
            HttpExchangeDataId = logEntry.Id
        });
        await _context.SaveChangesAsync();

    }
    private async Task<string> GetRequestBody(HttpContext context)
    {
        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
        {
            return await reader.ReadToEndAsync();
        }
    }
    private async Task<string> GetResponseBody(HttpContext context)
    {
        using (var reader = new StreamReader(context.Response.Body, Encoding.UTF8))
        {
            return await reader.ReadToEndAsync();
        }
    }
    private string GetRequestHeaders(HttpContext context)
    {
        var headers = context.Request.Headers;
        StringBuilder headersString = new StringBuilder();

        foreach (var (key, value) in headers)
        {
            headersString.AppendLine($"{key}: {string.Join(",", value)}");
        }

        return headersString.ToString();
    }
    private string GetResponseHeaders(HttpContext context)
    {
        var headers = context.Response.Headers;
        StringBuilder headersString = new StringBuilder();

        foreach (var (key, value) in headers)
        {
            headersString.AppendLine($"{key}: {string.Join(",", value)}");
        }

        return headersString.ToString();
    }
}