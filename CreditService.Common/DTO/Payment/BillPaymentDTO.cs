using CreditService.DAL.Enum;

namespace CreditService.Common.DTO.Payment;

public class BillPaymentDTO
{
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public int AccountId { get; set; }
    public Guid LoanId { get; set; }
    public MoneyDto Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public PaymentStatus Status { get; set; }
}