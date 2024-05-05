using CreditService.DAL.Enum;

namespace CreditService.DAL.Entities;

public class CircuitBreaker : IIdentity<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public CircuitBreakerStatus CircuitBreakerStatus { get; set; }
    public int ErrorCount { get; set; }
    public int RequestCount { get; set; }
    public DateTime OpenTime { get; set; }
}