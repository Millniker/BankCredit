using CreditService.Common.DTO;
using CreditService.Common.DTO.Payment;

namespace CreditService.Common.Interfaces;

public interface IPaymentService
{
    public Task<Response> PaymentProcessing(SendPaymentDto paymentDto, string requestId, string deviceId);
    public Task<List<BillPaymentDTO>> GetBillPayment(int accountId);
    public Task<CreditScoreDto> GetCreditScore(int userId);


}