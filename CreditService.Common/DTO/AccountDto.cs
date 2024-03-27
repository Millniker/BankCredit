using CreditService.DAL.Enum;

namespace CreditService.Common.DTO;

public class AccountDto
{
    public int Id { get; set; }
    public int AccountNumber { get; set; }
    public string UserId { get; set; }
    public decimal Balance { get; set; }
    public CurrencyType CurrencyType { get; set; }
    public string AccountType { get; set; }
    public string AccountStatus { get; set; } 
    public DateTime OpeningDate { get; set; }
    public decimal InterestRate { get; set; }
}