using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CreditService.DAL.Enum;

namespace CreditService.DAL.Entities;

public class LoanApp
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public LoanStatusType LoanStatus { get; set; }
    
    [Required]
    public DateTime Date { get; set; }

    public string Description { get; set; } = "Причина не указана";
    
    [ForeignKey("Loan")]
    public Guid LoanId { get; set; }
    
    public Loan Loan { get; set; }
    
    public Guid UserId { get; set; }
}