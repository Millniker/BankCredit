using System.Text.Json.Serialization;

namespace CreditService.DAL.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AccountType
{
    CURRENT_ACCOUNT, SAVINGS_ACCOUNT, FOREIGN_CURRENCY_ACCOUNT, LOAN_TYPE
}