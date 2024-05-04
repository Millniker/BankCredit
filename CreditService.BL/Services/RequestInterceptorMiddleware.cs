using System.Text;
using CreditService.Common.DTO;
using CreditService.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CreditService.BL.Services;

public class RequestInterceptorMiddleware
{  
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;
    public RequestInterceptorMiddleware(RequestDelegate next,IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey("RequestId"))
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                // Получаем идентификатор запроса (RequestId) из заголовка
                var requestId = context.Request.Headers["RequestId"];
                AppDbContext _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var iId = Guid.Parse(requestId);
                var exchangeData = await _dbContext.IdempotencyId.Where(x => x.IdempotencyKey == iId.ToString()).FirstOrDefaultAsync();
                if (exchangeData != null)
                {
                var http = await _dbContext.HttpExchangeData.Where(x => x.Id == exchangeData.HttpExchangeDataId).FirstAsync();
                if (http != null)
                {
                    var responseDto = new HttpExchangeDataDTO
                    {
                        Id = http.Id,
                        RequestDate = http.RequestDate,
                        RequestPath = http.RequestPath,
                        RequestMethod = http.RequestMethod,
                        RequestBody = http.RequestBody,
                        RequestHeaders = http.RequestHeaders,
                        ResponseCode = http.ResponseCode,
                        ResponseBody = http.ResponseBody,
                        ResponseHeaders = http.ResponseBody
                    };
                    
                    var jsonResponse = JsonConvert.SerializeObject(responseDto);
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(jsonResponse);
                }
                }
            }
        }
    }

}