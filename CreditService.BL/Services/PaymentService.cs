using System.Net.Http.Json;
using CreditService.Common.DTO;
using CreditService.Common.Exceptions;
using CreditService.Common.Interfaces;
using CreditService.Common.System;
using CreditService.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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

    public async Task<Response> PaymentProcessing(PaymentDto paymentDto)
    {
        var loan = await _context.Loan.FindAsync(paymentDto.LoanId);
        if (loan == null)
        {
            throw new ItemNotFoundException($"не найден кредит с id={paymentDto.LoanId}");
        }
        
        //var response = await _httpClient.PostAsJsonAsync(ApiConstants.DepositBaseUrl, paymentDto);
        loan.Amount -= paymentDto.Amount;
        
        _context.Loan.Attach(loan);
        _context.Entry(loan).State = EntityState.Modified;

        if (loan.Amount <= 0)
        {
                //var apiUrlCloseAccount = $"{ApiConstants.CloseAccountBaseUrl}/{paymentDto.LoanId}";
                //var responseCloseAccount = await _httpClient.DeleteAsync(apiUrlCloseAccount);
                _context.Loan.Remove(loan);
                await _context.SaveChangesAsync();

                return new Response
                {
                    Code = "200",
                    Message = "Кредит выплачен"
                };
        }
        
        return new Response
        {
            Code = "200",
            Message = "Оплата принята"
        };
    }
    

}