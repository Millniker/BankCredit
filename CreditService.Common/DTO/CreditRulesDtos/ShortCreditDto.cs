using System.ComponentModel.DataAnnotations;

namespace CreditService.Common.DTO;

public class ShortCreditDto
{
    public Guid Id { get; set; }
    
    [Required]
    public MoneyDto AmountMax { get; set;}
    
    [Required]
    public Double InterestRateMin { get; set;}
    
    [Required]
    public String Name { get; set;}
    
    [Required]
    public Int32 Term { get; set;}
}