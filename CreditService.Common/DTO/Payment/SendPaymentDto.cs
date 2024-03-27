namespace CreditService.Common.DTO.Payment;

public class SendPaymentDto
{
    public string BillPaymentId { get; set; }
    public string UserId { get; set; }
    public int AccountId { get; set; }
    public Guid LoanId { get; set; }
    public MoneyDto Amount { get; set; }
}