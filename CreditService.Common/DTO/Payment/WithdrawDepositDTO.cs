using CreditService.DAL.Enum;

namespace CreditService.Common.DTO.Payment;

public class WithdrawDepositDTO
{
    public int AccountId { get; set; }
    public decimal Amount { get; set; }
    public CurrencyType CurrencyType { get; set; }
}