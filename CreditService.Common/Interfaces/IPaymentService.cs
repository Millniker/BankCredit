using CreditService.Common.DTO;

namespace CreditService.Common.Interfaces;

public interface IPaymentService
{
    public Task<Response> PaymentProcessing(PaymentDto paymentDto);
    

}