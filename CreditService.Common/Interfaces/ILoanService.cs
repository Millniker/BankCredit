using CreditService.Common.DTO;

namespace CreditService.Common.Interfaces;

public interface ILoanService
{
    public Task<List<LoanAppDto>> GetAllLoanAppDtoByUserId(string userId);
    public Task<LoanResultResponse> SendLoanApp(string id, ShortLoanAppDto sendLoanAppDto);
    public Task<List<LoanDto>> GetAllLoanDtoByUserId(string userId);
    public Task<LoanAppDto> GetLoanAppDto(Guid loanAppId);


}