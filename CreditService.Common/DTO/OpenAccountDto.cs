using CreditService.DAL.Enum;

namespace CreditService.Common.DTO;

public class OpenAccountDto
{
    public Guid UserId;
    public Decimal InitialDeposit;
    public CurrencyType CurrencyType;
    public string AccountType = "LOAN_ACCOUNT";
    public double InterestRate;
}