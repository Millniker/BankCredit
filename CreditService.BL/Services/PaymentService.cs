using System.Net.Http.Json;
using CreditService.Common.DTO;
using CreditService.Common.DTO.Payment;
using CreditService.Common.Exceptions;
using CreditService.Common.Interfaces;
using CreditService.Common.System;
using CreditService.DAL;
using CreditService.DAL.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CreditService.BL.Services;

public class PaymentService:IPaymentService
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;
    public PaymentService(AppDbContext context, IOptions<HttpClient> httpClient)
    {
        _context = context;
        _httpClient = httpClient.Value;
    }

    public async Task<Response> PaymentProcessing(SendPaymentDto paymentDto)
    {
        var loan = await _context.Loan.FindAsync(paymentDto.LoanId);
        var billPayment = await _context.BillPayment.FindAsync(paymentDto.BillPaymentId);
        if (billPayment is { Status: PaymentStatus.Paid })
        {
            throw new IncorrectDataException("Счет уже оплачен");
        }
        if (loan == null)
        {
            throw new ItemNotFoundException($"не найден кредит с id={paymentDto.LoanId}");
        }
        if (billPayment == null)
        {
            throw new ItemNotFoundException($"не платеж с id={paymentDto.BillPaymentId}");
        }

        var pay = (double)paymentDto.Amount.Amount / (1 + loan.InterestRate);
        var interest = (double)paymentDto.Amount.Amount - pay;
        var responsePay = await _httpClient.PostAsJsonAsync(ApiConstants.WithdrawBaseUrl, new WithdrawDepositDTO
        {
            AccountId = paymentDto.AccountId,
            Amount = interest.ToString().Replace(",", "."),
            CurrencyType = paymentDto.Amount.Currency
        });
        var responseInterst = await _httpClient.PostAsJsonAsync(ApiConstants.DepositBaseUrl, new WithdrawDepositDTO
        {
            AccountId = 1,
            Amount = pay.ToString().Replace(",", "."),
            CurrencyType = paymentDto.Amount.Currency
        });
        if (!responsePay.IsSuccessStatusCode)
            throw new FaildToLoadException("Ошибка оплаты " +responsePay.Content.ReadAsStringAsync().Result);
        if (!responseInterst.IsSuccessStatusCode)
            throw new FaildToLoadException("Ошибка оплаты " +responseInterst.Content.ReadAsStringAsync().Result);
        var accountResponse = await _httpClient.GetAsync(ApiConstants.Account+"/"+paymentDto.AccountId);
        if (!accountResponse.IsSuccessStatusCode)
            throw new FaildToLoadException("Ошибка получения счета");
        var account = JsonConvert.DeserializeObject<AccountDto>(accountResponse.Content.ReadAsStringAsync().Result);
        if (account is { Balance: 0 })
        {
            billPayment.Amount.Amount = 0;
            billPayment.Status = PaymentStatus.Paid;
            _context.BillPayment.Attach(billPayment);
            _context.Entry(billPayment).State = EntityState.Modified;
            _context.Loan.Remove(loan);
            await _context.SaveChangesAsync();
            var closeAccount =  await _httpClient.DeleteAsync(ApiConstants.CloseAccountBaseUrl+"/"+ paymentDto.AccountId);
            if (!closeAccount.IsSuccessStatusCode)
            {
                return new Response
                {
                    Code = "200",
                    Message = "Кредит выплачен, не удалось закрыть счет"
                };
            }
            var responseClose= await _httpClient.PostAsJsonAsync(ApiConstants.DepositBaseUrl, new WithdrawDepositDTO
            {
                AccountId = 1,
                Amount = loan.Amount.ToString("0,0000"),
                CurrencyType = paymentDto.Amount.Currency
            });
            if (!responseClose.IsSuccessStatusCode)
            {
                return new Response
                {
                    Code = "200",
                    Message = "Кредит выплачен, не удалось закрыть счет"
                };
            }
            return new Response
            {
                Code = "200",
                Message = "Кредит выплачен, счет закрыт"
            };
        }
        
        billPayment.Amount.Amount -= paymentDto.Amount.Amount;
        if (billPayment.Amount.Amount <= 0)
        {
            billPayment.Amount.Amount = 0;
            billPayment.Status = PaymentStatus.Paid;

        }
        
        if (account != null) loan.Amount = account.Balance;
        
        _context.BillPayment.Attach(billPayment);
        _context.Entry(billPayment).State = EntityState.Modified;
        _context.Loan.Attach(loan);
        _context.Entry(loan).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return new Response {
                Code = "200",
                Message = "Оплата принята"
            };
    }

    public async Task<List<BillPaymentDTO>> GetBillPayment(int accountId)
    {
        return await _context.BillPayment.Where(x => x.AccountId == accountId).Select(e => new BillPaymentDTO
        {
            Id = e.Id,
            UserId = e.UserId,
            AccountId = e.AccountId,
            LoanId = e.LoanId,
            Amount = new MoneyDto(e.Amount.Amount, e.Amount.Currency),
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            Status = e.Status
        }).ToListAsync();
    }

    public async Task<CreditScoreDto> GetCreditScore(int userId)
    {
        var overduePayments = await _context.BillPayment.Where(e => e.Status == PaymentStatus.OverduePayment && e.UserId == userId).ToListAsync();
        return new CreditScoreDto
        {
            Score = Math.Max(100 - overduePayments.Count, 0)
        };
    }

}