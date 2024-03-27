using CreditService.DAL.Enum;

namespace CreditService.Common.DTO;

public class OpenAccountDto
{
    public Decimal InitialDeposit;
    public string CurrencyType;
    public string AccountType = "LOAN_TYPE";
    public double InterestRate;
}