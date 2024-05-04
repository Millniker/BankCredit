using System.Net;
using System.Net.Http.Json;
using System.Text;
using CreditService.Common.DTO;
using CreditService.Common.Exceptions;
using CreditService.Common.Interfaces;
using CreditService.Common.System;
using CreditService.DAL;
using CreditService.DAL.Entities;
using CreditService.DAL.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using CreditService.Common.DTO.LoanAppDtos;
using CreditService.Common.Http;
using Newtonsoft.Json;
using LoanResponse = CreditService.Common.DTO.LoanAppDtos.LoanResponse;

namespace CreditService.BL.Services;

public class LoanService : ILoanService
{
    private readonly AppDbContext _context;
    private readonly LoanServiceHttp _loanHttpClient;
    private readonly AccountHttp _accountHttpClient;

    public LoanService(AppDbContext context, LoanServiceHttp loanHttp, AccountHttp accountHttp)
    {
        _context = context;
        _loanHttpClient = loanHttp;
        _accountHttpClient = accountHttp;
    }

    public async Task<List<LoanAppDto>> GetAllLoanAppDtoByUserId(int userId)
    {
        return await _context.LoanApp.Where(x => x.UserId == userId).Select(p => new LoanAppDto
        {
            Id = p.Id,
            Status = p.LoanStatus,
            Date = p.Date,
            Description = p.Description,
            LoanId = p.LoanId,
            Loan = new ShortLoanDto()
            {
                Amount = p.Loan.Amount,
                InterestRate = p.Loan.InterestRate,
                CurrencyType = p.Loan.CurrencyType,
                CreditRules = p.Loan.CreditRulesId,
                Term = p.Term
            },
        }).ToListAsync();
    }
    

    public async Task<List<LoanDto>> GetAllLoanDtoByUserId(int userId)
    {
        return await _context.LoanApp.Where(x => x.UserId == userId && x.LoanStatus == LoanStatusType.Approved)
            .Select(p => new LoanDto
            {
                Id = p.Loan.Id,
                Amount = p.Loan.Amount,
                InterestRate = p.Loan.InterestRate,
                CurrencyType = p.Loan.CurrencyType,
                CreditRules = p.Loan.CreditRulesId,
                LoanAppId = p.Loan.LoanAppId,
            }).ToListAsync();
    }

    public async Task<LoanAppDto> GetLoanAppDto(Guid id)
    {
        var loanApp = await _context.LoanApp.FindAsync(id);

        if (loanApp == null)
        {
            throw new ItemNotFoundException($"Не найдена заявка на кредит с id={id}");
        }

        var loan = await _context.Loan.FindAsync(loanApp.LoanId);
        if (loan == null)
        {
            throw new ItemNotFoundException($"Не найдена заявка на кредит с id={id}");
        }

        return new LoanAppDto
        {
            Id = loanApp.Id,
            Status = loanApp.LoanStatus,
            Date = loanApp.Date,
            Description = loanApp.Description,
            LoanId = loanApp.LoanId,
            Loan = new ShortLoanDto
            {
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                CurrencyType = loan.CurrencyType,
                CreditRules = loan.CreditRulesId,
            }
        };
    }

    public async Task<LoanResultResponse> SendLoanApp(int userId, ShortLoanAppDto sendLoanAppDto)
    {
        var creditRules = await _context.CreditRules.FindAsync(sendLoanAppDto.CreditRules);

        if (creditRules == null)
        {
            throw new ItemNotFoundException($"Не найдены условия кредита с id={sendLoanAppDto.CreditRules}");
        }

        var creditScore = await _loanHttpClient.getCreditScore(userId);
        var toDesicionDto = new ToDesicionDto
        {
            Amount = sendLoanAppDto.Amount,
            InterestRate = sendLoanAppDto.InterestRate,
            CurrencyType = sendLoanAppDto.CurrencyType,
            CreditRules = sendLoanAppDto.CreditRules,
            CreditScore = creditScore.Score,
            AmountMax = new MoneyDto(creditRules.AmountMax.Amount, creditRules.AmountMax.Currency),
            AmountMin = new MoneyDto(creditRules.AmountMin.Amount, creditRules.AmountMin.Currency),
            InterestRateMax = creditRules.InterestRateMax,
            InterestRateMin = creditRules.InterestRateMin,
            Term = sendLoanAppDto.Term,
        };
        
        var loan = new Loan
        {
            Id = new Guid(),
            Amount = sendLoanAppDto.Amount,
            AccountId = 0,
            InterestRate = sendLoanAppDto.InterestRate,
            CurrencyType = sendLoanAppDto.CurrencyType,
            CreditRulesId = creditRules.Id,
            UserId = userId,
            Term = sendLoanAppDto.Term
        };
        
        var loanAppEntity = new LoanApp
        {
            Id = new Guid(),
            LoanStatus = LoanStatusType.Pending,
            Date = DateTime.UtcNow,
            UserId = userId,
            Term = sendLoanAppDto.Term
        };
        
        
      

        await _context.LoanApp.AddAsync(loanAppEntity);

        loan.LoanAppId = loanAppEntity.Id;
        await _context.Loan.AddAsync(loan);
        loanAppEntity.LoanStatus = LoanStatusType.Approved;
        loanAppEntity.LoanId = loan.Id;
        loanAppEntity.Loan = loan;

        creditRules.LoanIds.Add(loan.Id);

        _context.CreditRules.Attach(creditRules);
        _context.Entry(creditRules).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        var loanDesicionResponse = await _loanHttpClient.SendOnCheckLoan(toDesicionDto);
        if (loanDesicionResponse is not { Result: true }){
	     throw new IncorrectDataException ("В кредите отказано по следующей причине " +loanDesicionResponse?.Message);
         }

        var loanResult = new AnsLoanAppDto
        {
            InitialDeposit = (int)loan.Amount,
            CurrencyType = loan.CurrencyType,
            AccountType = AccountType.LOAN_TYPE,
            InterestRate = loan.InterestRate,
            UserId = userId,
        };
        loanAppEntity.LoanStatus = LoanStatusType.Approved;
        loanAppEntity.Description = "Одобрен";
        var accountCreateResponse = await LoanProcessing(loanResult);
        loan.AccountId = accountCreateResponse.AccountId;
        _context.Loan.Attach(loan);
        _context.Entry(loan).State = EntityState.Modified;
        _context.LoanApp.Attach(loanAppEntity);
        _context.Entry(loanAppEntity).State = EntityState.Modified;
        _context.LoanApp.Attach(loanAppEntity);
        _context.Entry(loanAppEntity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        var loanResponse = new LoanResultResponse
        {
            LoanStatus = "Результат по кредиту Одобрен",
            AccountCode = accountCreateResponse.Code,
            AccountStatus = "Результат создания кредитного счета " + accountCreateResponse.Message
        };
        return loanResponse;

    }

    public async Task<AccountCreateResponse> LoanProcessing(AnsLoanAppDto loanApp)
    {
        var createAccountDTO = new CreateAccountDto
        {
            InitialDeposit = loanApp.InitialDeposit,
            CurrencyType = loanApp.CurrencyType,
            AccountType = AccountType.LOAN_TYPE,
            InterestRate = loanApp.InterestRate
        };


        var accountResponse = await _accountHttpClient.openAccount(loanApp.UserId, createAccountDTO);
        return accountResponse;
    }
}
