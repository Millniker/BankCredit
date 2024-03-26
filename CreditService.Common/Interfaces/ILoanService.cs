using CreditService.Common.DTO;

namespace CreditService.Common.Interfaces;

public interface ILoanService
{
    public Task<List<LoanAppDto>> GetAllLoanAppDtoByUserId(Guid userId);
    public Task<AnsLoanAppDto> SendLoanApp(Guid id, ShortLoanAppDto sendLoanAppDto);
    public Task<Response> LoanProcessing(AnsLoanAppDto loanApp);
    public Task<List<LoanDto>> GetAllLoanDtoByUserId(Guid userId);
    public Task<LoanAppDto> GetLoanAppDto(Guid loanAppId);


}