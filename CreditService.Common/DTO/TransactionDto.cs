namespace CreditService.Common.DTO;

public class TransactionDto
{
    public int Id { get; set; }
    public string Amount { get; set; }
    public int? FromAccountId { get; set; }
    public int ToAccountId { get; set; }
    public string TransactionType { get; set; }
    public string TransactionDate { get; set; }
    public string CurrencyType { get; set; }
}