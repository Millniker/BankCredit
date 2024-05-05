using System.Text.Json.Serialization;

namespace CreditService.DAL.Enum;

[Newtonsoft.Json.JsonConverter(typeof(JsonStringEnumConverter))]

public enum CircuitBreakerStatus
{
    Open,
    Closed,
    HalfOpen
}