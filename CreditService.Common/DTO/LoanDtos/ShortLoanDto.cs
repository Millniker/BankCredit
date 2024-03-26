using System.ComponentModel.DataAnnotations;
using CreditService.DAL.Enum;

namespace CreditService.Common.DTO;

public class ShortLoanDto
{
    [Required]
    public Decimal Amount { get; set;}

    [Required]
    public Double InterestRate { get; set;}
    
    [Required]
    public CurrencyType CurrencyType { get; set;}
    
    [Required]
    public Guid CreditRules { get; set; }
}