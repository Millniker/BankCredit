using CreditService.Common.DTO;

namespace CreditService.Common.Interfaces;

public interface IIdempotentService
{
    public HttpExchangeDataDTO? AddIdempotent(String requestId);

}