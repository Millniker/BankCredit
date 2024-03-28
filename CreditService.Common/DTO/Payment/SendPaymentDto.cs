namespace CreditService.Common.DTO.Payment;

public class SendPaymentDto
{
    public Guid BillPaymentId { get; set; }
    public int AccountId { get; set; }
    public Guid LoanId { get; set; }
    public MoneyDto Amount { get; set; }
}