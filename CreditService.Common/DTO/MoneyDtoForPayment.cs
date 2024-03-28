using CreditService.DAL.Enum;

namespace CreditService.Common.DTO;

public class MoneyDtoForPayment
{
    public String Amount { get;  set; }
    public CurrencyType Currency { get;  set; }
    public MoneyDtoForPayment(String amount, CurrencyType currency)
    {
        Amount = amount;
        Currency = currency;
    }
}