using System.Net.Http.Json;
using CreditService.BL.Http;
using CreditService.Common.DTO;
using CreditService.Common.DTO.Payment;
using CreditService.Common.Exceptions;
using CreditService.Common.Http;
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
    private readonly AccountHttp _accountHttpClient;
    public PaymentService(AppDbContext context,  AccountHttp accountHttp)
    {
        _context = context;
        _accountHttpClient = accountHttp;
    }

    public async Task<Response> PaymentProcessing(SendPaymentDto paymentDto, string requestId,  string deviceId)
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
        await _accountHttpClient.WithdrowMoney(new WithdrawDepositDTO
        {
            AccountId = paymentDto.AccountId,
            Amount = interest.ToString().Replace(",", "."),
            CurrencyType = paymentDto.Amount.Currency

        });
        await _accountHttpClient.DepositMoney(new WithdrawDepositDTO
        {
            AccountId = 1,
            Amount = pay.ToString().Replace(",", "."),
            CurrencyType = paymentDto.Amount.Currency

        });


        var account = await _accountHttpClient.GetAccount(paymentDto.AccountId, requestId,  deviceId);
        if (account is { Balance: 0 })
        {
            _context.BillPayment.Remove(billPayment);
            _context.Loan.Remove(loan);
            await _context.SaveChangesAsync();
           
            if (! await _accountHttpClient.DeleteAccount(paymentDto.AccountId))
            {
                return new Response
                {
                    Code = "200",
                    Message = "Кредит выплачен, не удалось закрыть счет"
                };
            }

            await _accountHttpClient.DepositMoney(new WithdrawDepositDTO
            {
                AccountId = 1,
                Amount = loan.Amount.ToString("0,0000"),
                CurrencyType = paymentDto.Amount.Currency
            });
            
            return new Response
            {
                Code = "200",
                Message = "Кредит выплачен, счет закрыт"
            };
        }
        
        billPayment.Amount.Amount -= paymentDto.Amount.Amount;
        if (billPayment.Amount.Amount <= 0)
        {
            _context.BillPayment.Remove(billPayment);
        }
        
        if (account != null) loan.Amount = account.Balance;
        
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
