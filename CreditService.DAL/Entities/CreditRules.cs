using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditService.DAL.Entities;

public class CreditRules
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public Money AmountMax { get; set;}
    [Required]
    public Money AmountMin { get; set;}
    
    [Required]
    public Double InterestRateMax { get; set;}
    
    [Required]
    public Double InterestRateMin { get; set;}
    
    [Required]
    public String Name { get; set;}
    
    [Required]
    public Int32 Term { get; set;}
    
    public List<Guid> LoanIds { get; set; }

}