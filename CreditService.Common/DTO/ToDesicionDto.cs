using System.ComponentModel.DataAnnotations;
using CreditService.DAL.Enum;

namespace CreditService.Common.DTO;

public class ToDesicionDto
{
    [Required]
    public Decimal Amount { get; set;}
    [Required]
    public Double InterestRate { get; set;}
    [Required]
    public CurrencyType CurrencyType { get; set;}
    
    public Guid CreditRules { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public double CreditScore { get; set; }
    
    public MoneyDto AmountMax { get; set;}
    [Required]
    public MoneyDto AmountMin { get; set;}
    
    [Required]
    public Double InterestRateMax { get; set;}
    
    [Required]
    public Double InterestRateMin { get; set;}
    
    [Required]
    public String Name { get; set;}
    
    [Required]
    public Int32 Term { get; set;}
    public Decimal MasterAccountAmount {get; set; }
}