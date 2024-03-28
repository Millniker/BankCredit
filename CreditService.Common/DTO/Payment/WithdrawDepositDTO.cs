using CreditService.DAL.Enum;

namespace CreditService.Common.DTO.Payment;

public class WithdrawDepositDTO
{
    public int AccountId { get; set; }
    public String Amount { get; set; }
    public CurrencyType CurrencyType { get; set; }
}