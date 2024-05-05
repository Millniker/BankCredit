using CreditService.Common.DTO;

namespace CreditService.Common.Interfaces;

public interface ILoanService
{
    public Task<List<LoanAppDto>> GetAllLoanAppDtoByUserId(int userId);
    public Task<LoanResultResponse> SendLoanApp(int id, ShortLoanAppDto sendLoanAppDto, string requestId, string deviceId);
    public Task<List<LoanDto>> GetAllLoanDtoByUserId(int userId);
    public Task<LoanAppDto> GetLoanAppDto(Guid loanAppId);


}