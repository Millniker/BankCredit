using CreditService.DAL;
using CreditService.DAL.Entities;
using CreditService.DAL.Enum;
using Microsoft.EntityFrameworkCore;

namespace CreditService.BL.Services;

public class RetryService
{
    private readonly AppDbContext _context;

    public RetryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddException()
    {
        var circuitBreaker = await _context.CircuitBreaker.FirstOrDefaultAsync();
        circuitBreaker.ErrorCount ++;
        await _context.SaveChangesAsync();
    }
    public async Task AddRequest()
    {
        var circuitBreaker = await _context.CircuitBreaker.FirstOrDefaultAsync();
        circuitBreaker.RequestCount ++;
        await _context.SaveChangesAsync();
    }

    public async Task ChangStatus()
    {
        var circuitBreaker = await _context.CircuitBreaker.FirstOrDefaultAsync();
        if (circuitBreaker == null)
        {
            _context.CircuitBreaker.Add(new CircuitBreaker
            {
                Id = Guid.NewGuid(),
                Name = "Credit",
                CircuitBreakerStatus = CircuitBreakerStatus.Open,
                ErrorCount = 0,
                RequestCount = 0,
                OpenTime = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }
        if (circuitBreaker.CircuitBreakerStatus != CircuitBreakerStatus.Open && circuitBreaker.ErrorCount>0)
        {
            if (((float)circuitBreaker.ErrorCount / (float)circuitBreaker.RequestCount) * 100 > 70)
            {
                circuitBreaker.CircuitBreakerStatus = CircuitBreakerStatus.Open;
                circuitBreaker.RequestCount = 0;
                circuitBreaker.ErrorCount = 0;
                circuitBreaker.OpenTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
        else if (circuitBreaker.CircuitBreakerStatus == CircuitBreakerStatus.HalfOpen && circuitBreaker.RequestCount / circuitBreaker.ErrorCount * 100 < 20)
        {
            if (circuitBreaker.OpenTime.AddMinutes(10) <= DateTime.UtcNow)
            {
                circuitBreaker.CircuitBreakerStatus = CircuitBreakerStatus.Closed;
                circuitBreaker.OpenTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();

            } 
        }
       if (circuitBreaker.CircuitBreakerStatus == CircuitBreakerStatus.Open)
        {
            if (circuitBreaker.OpenTime.AddMinutes(5) <= DateTime.UtcNow)
            {
                circuitBreaker.CircuitBreakerStatus = CircuitBreakerStatus.HalfOpen;
                await _context.SaveChangesAsync();
            }
        }
    }
}