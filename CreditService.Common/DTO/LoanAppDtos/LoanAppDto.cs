using System.ComponentModel.DataAnnotations;
using CreditService.DAL.Enum;

namespace CreditService.Common.DTO;

public class LoanAppDto
{
    public Guid Id { get; set; }
    
    [Required]
    public LoanStatusType Status { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    public string Description { get; set; }
    
    public Guid LoanId { get; set; }
    
    public ShortLoanDto Loan { get; set; }
}