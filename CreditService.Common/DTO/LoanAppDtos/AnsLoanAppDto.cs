using System.ComponentModel.DataAnnotations;
using CreditService.DAL.Enum;

namespace CreditService.Common.DTO;

public class AnsLoanAppDto
{
    public Guid Id { get; set; }

    [Required]
    public LoanStatusType Status { get; set; }
    
    public string Description { get; set; }
}