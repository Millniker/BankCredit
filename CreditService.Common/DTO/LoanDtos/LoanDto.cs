using System.ComponentModel.DataAnnotations;
using CreditService.DAL.Enum;

namespace CreditService.Common.DTO;

public class LoanDto
{
    public Guid Id { get; set; }
    
    [Required]
    public Decimal Amount { get; set;}
    [Required]
    public Double InterestRate { get; set;}
    [Required]
    public CurrencyType CurrencyType { get; set;}
    
    public Guid CreditRules { get; set; }
    
    public Guid LoanAppId { get; set; }
}