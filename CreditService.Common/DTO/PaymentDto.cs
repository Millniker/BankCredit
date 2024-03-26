using CreditService.DAL.Enum;

namespace CreditService.Common.DTO;

public class PaymentDto
{
    public CurrencyType CurrencyType { get; set;}
    public Guid LoanId { get; set;}
    public Decimal Amount { get; set; }
}