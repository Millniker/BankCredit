using System.Security.AccessControl;
using CreditService.Common.DTO;
using CreditService.Common.Exceptions;
using CreditService.Common.Interfaces;
using CreditService.DAL;
using CreditService.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CreditService.BL.Services;

public class CreditRulesService: ICreditService
{
    private readonly AppDbContext _context;

    public CreditRulesService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ShortCreditDto>> GetAllCreditRules()
    {
        return await _context.CreditRules.Select(e => new ShortCreditDto()
        {
            Id = e.Id,
            AmountMax = new MoneyDto(e.AmountMax.Amount, e.AmountMax.Currency),
            InterestRateMin = e.InterestRateMin,
            Name = e.Name,
            Term = e.Term,
        }).ToListAsync();
    }

    public async Task<CreditDto> GetCreditRule(Guid id)
    {
        var credit = await _context.CreditRules.FindAsync(id);
        if (credit == null)
        {
            throw new ItemNotFoundException();
        }

        return new CreditDto
        {
            Id = credit.Id,
            AmountMax = new MoneyDto(credit.AmountMax.Amount, credit.AmountMax.Currency),
            AmountMin = new MoneyDto(credit.AmountMin.Amount,credit.AmountMin.Currency),
            InterestRateMax = credit.InterestRateMax,
            InterestRateMin = credit.InterestRateMin,
            Name = credit.Name,
            Term = credit.Term,
        };
    }

    public async Task AddCreditRule(AddCreditRuleDto creditRule)
    {

        var credit = new CreditRules
        {
            Id = new Guid(),
            AmountMax = new Money(creditRule.AmountMax.Amount, creditRule.AmountMax.Currency),
            AmountMin = new Money(creditRule.AmountMin.Amount, creditRule.AmountMin.Currency),
            InterestRateMax = creditRule.InterestRateMax,
            InterestRateMin = creditRule.InterestRateMin,
            Name = creditRule.Name,
            Term = creditRule.Term,
            LoanIds = new List<Guid>()
        };

        await _context.CreditRules.AddAsync(credit);
        await _context.SaveChangesAsync();
    }

    public async Task EditCreditRule(Guid id,AddCreditRuleDto creditRule)
    {
        var credit = await _context.CreditRules.FindAsync(id);
        if (credit == null)
        {
            throw new ItemNotFoundException($"Не найдет кредит с id={id}");
        }

        credit.Name = creditRule.Name;
        credit.AmountMax = new Money(creditRule.AmountMax.Amount,creditRule.AmountMax.Currency);
        credit.AmountMin = new Money(creditRule.AmountMin.Amount,creditRule.AmountMin.Currency);
        credit.InterestRateMax = creditRule.InterestRateMax;
        credit.Term = creditRule.Term;
        credit.InterestRateMin = creditRule.InterestRateMin;
        
        _context.CreditRules.Attach(credit);
        _context.Entry(credit).State = EntityState.Modified;
        
        await _context.SaveChangesAsync();
        
    }
    
    public async Task DeleteCreditRule(Guid id)
    {
        var credit = await _context.CreditRules.FindAsync(id);
        if (credit == null)
        {
            throw new ItemNotFoundException($"Не найдет кредит с id={id}");
        }
        _context.CreditRules.Remove(credit);
        await _context.SaveChangesAsync();
        
    }
}