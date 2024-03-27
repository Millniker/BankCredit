using CreditService.DAL.Enum;

namespace CreditService.DAL.Entities;

public class BillPayment
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public int AccountId { get; set; }
    public Guid LoanId { get; set; }
    public Money Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public PaymentStatus Status { get; set; }
}