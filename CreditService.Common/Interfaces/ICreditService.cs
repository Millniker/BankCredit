using CreditService.Common.DTO;

namespace CreditService.Common.Interfaces;

public interface ICreditService
{
    public Task<List<ShortCreditDto>> GetAllCreditRules();
    public Task<CreditDto> GetCreditRule(Guid id);
    public Task EditCreditRule(Guid id, AddCreditRuleDto creditRule);
    public Task DeleteCreditRule(Guid id);
    public Task AddCreditRule(AddCreditRuleDto creditRule);


}