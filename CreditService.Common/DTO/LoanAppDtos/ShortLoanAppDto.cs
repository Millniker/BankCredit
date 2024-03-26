using System.ComponentModel.DataAnnotations;
using CreditService.DAL.Enum;

namespace CreditService.Common.DTO;

public class ShortLoanAppDto
{
    [Required]
    public Decimal Amount { get; set;}
    [Required]
    public Double InterestRate { get; set;}
    [Required]
    public CurrencyType CurrencyType { get; set;}
    
    public Guid CreditRules { get; set; }
}