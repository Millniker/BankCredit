using System.Text.Json.Serialization;

namespace CreditService.DAL.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CurrencyType
{
    RUB,
    USD,
    EUR
}