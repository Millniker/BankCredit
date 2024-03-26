using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CreditService.DAL.Enum;

namespace CreditService.DAL.Entities;

public class Loan
{ 
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public Decimal Amount { get; set;}
    [Required]
    public Double InterestRate { get; set;}
    [Required]
    public CurrencyType CurrencyType { get; set;}
    
    [ForeignKey("CreditRules")]
    public Guid CreditRulesId { get; set; }
    
    public Guid LoanAppId { get; set; }
    public Guid AccountId { get; set; }
    
}