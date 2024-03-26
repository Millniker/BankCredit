using CreditService.DAL.Enum;

namespace CreditService.Common.DTO;

public class MoneyDto
{
    public decimal Amount { get;  set; }
    public CurrencyType Currency { get;  set; }
    public MoneyDto(decimal amount, CurrencyType currency)
    {
        Amount = amount;
        Currency = currency;
    }
}