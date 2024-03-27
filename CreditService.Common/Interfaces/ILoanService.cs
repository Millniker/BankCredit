using CreditService.Common.DTO;

namespace CreditService.Common.Interfaces;

public interface ILoanService
{
    public Task<List<LoanAppDto>> GetAllLoanAppDtoByUserId(string userId);
    public Task<AnsLoanAppDto> SendLoanApp(string id, ShortLoanAppDto sendLoanAppDto);
    public Task<Response> LoanProcessing(AnsLoanAppDto loanApp);
    public Task<List<LoanDto>> GetAllLoanDtoByUserId(string userId);
    public Task<LoanAppDto> GetLoanAppDto(Guid loanAppId);


}