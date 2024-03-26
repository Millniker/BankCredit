using System.Net;
using System.Net.Http.Json;
using System.Security.AccessControl;
using CreditService.Common.DTO;
using CreditService.Common.Exceptions;
using CreditService.Common.Interfaces;
using CreditService.Common.System;
using CreditService.DAL;
using CreditService.DAL.Entities;
using CreditService.DAL.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;
using CreditService.Common.DTO.LoanAppDtos;
using Newtonsoft.Json;

namespace CreditService.BL.Services;

public class LoanService : ILoanService
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;

    public LoanService(AppDbContext context, IOptions<HttpClient> httpClient)
    {
        _context = context;
        _httpClient = httpClient.Value;
    }

    public async Task<List<LoanAppDto>> GetAllLoanAppDtoByUserId(Guid userId)
    {
        return await _context.LoanApp.Where(x => x.UserId == userId).Select(p => new LoanAppDto()
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
            },
        }).ToListAsync();
    }

    public async Task<List<LoanDto>> GetAllLoanDtoByUserId(Guid userId)
    {
        return await _context.LoanApp.Where(x => x.UserId == userId && x.LoanStatus == LoanStatusType.Approved)
            .Select(p => new LoanDto()
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
            Loan = new ShortLoanDto()
            {
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                CurrencyType = loan.CurrencyType,
                CreditRules = loan.CreditRulesId,
            }
        };
    }

    public async Task<AnsLoanAppDto> SendLoanApp(Guid userId, ShortLoanAppDto sendLoanAppDto)
    {
        var creditRules = await _context.CreditRules.FindAsync(sendLoanAppDto.CreditRules);
        var masterAccount =  _context.MasterAccount
            .SingleOrDefault(x => x.Money.Any(y => y.Currency == sendLoanAppDto.CurrencyType));
        if (creditRules == null)
        {
            throw new ItemNotFoundException($"Не найдены условия кредита с id={sendLoanAppDto.CreditRules}");
        }
        var toDesicionDto = new ToDesicionDto
        {
            Amount = sendLoanAppDto.Amount,
            InterestRate = sendLoanAppDto.InterestRate,
            CurrencyType = sendLoanAppDto.CurrencyType,
            CreditRules = sendLoanAppDto.CreditRules,
            Email = "cdfvjfjvnfj@gmail.com",
            Username = "Petr",
            Password = "vfnvfjnvj",
            Role = "CLIENT",
            CreditScore = 16,
            AmountMax = new MoneyDto(creditRules.AmountMax.Amount, creditRules.AmountMax.Currency),
            AmountMin = new MoneyDto(creditRules.AmountMin.Amount, creditRules.AmountMin.Currency),
            InterestRateMax = creditRules.InterestRateMax,
            InterestRateMin = creditRules.InterestRateMin,
            Name = creditRules.Name,
            Term = creditRules.Term,
        };
        var response = await _httpClient.PostAsJsonAsync(ApiConstants.SendOnCheckLoanBaseUrl, toDesicionDto);
        var loan = new Loan
        {
            Id = new Guid(),
            Amount = sendLoanAppDto.Amount,
            InterestRate = sendLoanAppDto.InterestRate,
            CurrencyType = sendLoanAppDto.CurrencyType,
            CreditRulesId = creditRules.Id,
            AccountId = userId
        };
        var loanAppEntity = new LoanApp
        {
            Id = new Guid(),
            LoanStatus = LoanStatusType.Pending,
            Date = DateTime.UtcNow,
            UserId = userId
        };
        if (response.StatusCode == (HttpStatusCode)200)
        {
            LoanResponse? responseObject = JsonConvert.DeserializeObject<LoanResponse>(response.Content.ReadAsStringAsync().Result);
            if (responseObject is { Result: true })
            {
                
                await _context.LoanApp.AddAsync(loanAppEntity);

                loan.LoanAppId = loanAppEntity.Id;
                await _context.Loan.AddAsync(loan);

                loanAppEntity.LoanId = loan.Id;
                loanAppEntity.Loan = loan;

                creditRules.LoanIds.Add(loan.Id);

                _context.CreditRules.Attach(creditRules);
                _context.Entry(creditRules).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return new AnsLoanAppDto
                {
                    Id = loanAppEntity.Id,
                    Status = LoanStatusType.Approved,
                    Description = "Одобрен"
                };
            }

            return new AnsLoanAppDto
            {
                Id = loanAppEntity.Id,
                Status = LoanStatusType.Rejected,
                Description = "В кредите отказано по следующей причине: " + responseObject.Message
            };

        }

        throw new ErrorReceivingDecisionException("Ошибка получения ответа по заявке на кредит");

    }

    public async Task<Response> LoanProcessing(AnsLoanAppDto loanApp)
    {
        var loanAppEntity = await _context.LoanApp.FindAsync(loanApp.Id);
        if (loanAppEntity == null)
        {
            throw new ItemNotFoundException($"Не найдена заявка на кредит с id={loanApp.Id}");
        }

        loanAppEntity.LoanStatus = loanApp.Status;
        loanAppEntity.Description = loanApp.Description;

        _context.LoanApp.Attach(loanAppEntity);
        _context.Entry(loanAppEntity).State = EntityState.Modified;

        var openAccountDto = new OpenAccountDto
        {
            UserId = loanAppEntity.UserId,
            InitialDeposit = loanAppEntity.Loan.Amount,
            CurrencyType = loanAppEntity.Loan.CurrencyType,
            InterestRate = loanAppEntity.Loan.InterestRate,
        };
        //var apiUrl = ApiConstants.OpenAccountBaseUrl;
        //var response = await _httpClient.PostAsJsonAsync(apiUrl, openAccountDto);

        await _context.SaveChangesAsync();
        return new Response
        {
            Code = "200",
            Message = "Кредит оформлен"
        };
    }
}