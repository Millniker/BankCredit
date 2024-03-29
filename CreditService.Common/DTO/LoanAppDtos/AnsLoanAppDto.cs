using System.ComponentModel.DataAnnotations;
using CreditService.DAL.Enum;

namespace CreditService.Common.DTO;

public class AnsLoanAppDto
{
    public Guid Id { get; set; }

    [Required]
    public LoanStatusType Status { get; set; }
    public int UserId { get; set; }
    
    public int InitialDeposit { get; set; } = 0;

    public CurrencyType CurrencyType { get; set; } = CurrencyType.RUB;

    public AccountType AccountType { get; set; } = AccountType.LOAN_TYPE;

    public double InterestRate { get; set; } = 10;
}