using CreditService.DAL;
using CreditService.DAL.Entities;
using CreditService.DAL.Enum;
using Microsoft.EntityFrameworkCore;

namespace CreditService.BL.Services;

public class PaymentCalculator
{
    private readonly AppDbContext _dbContext;

    public PaymentCalculator(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CalculateAndSendPaymentsAsync()
    {
        var loans = await _dbContext.Loan.ToListAsync();
        Console.WriteLine("Пошла возьня кредитов");

        foreach (var loan in loans)
        {
            var existingPayments = _dbContext.BillPayment
                .Where(bp => bp.AccountId == loan.AccountId)
                .ToList();

            if (existingPayments.Count < loan.Term)
            {
                int remainingPayments = loan.Term - existingPayments.Count;

                for (int i = 0; i < remainingPayments; i++)
                {
                    var billPayment = new BillPayment
                    {
                        Id = Guid.NewGuid(),
                        UserId = loan.UserId,
                        AccountId = loan.AccountId,
                        LoanId = loan.Id,
                        Amount = new Money((decimal)loan.InterestRate * loan.Amount, loan.CurrencyType),
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow + TimeSpan.FromDays(30),
                        Status = PaymentStatus.AwaitPayment
                    };
                    _dbContext.BillPayment.Add(billPayment);
                }
            }
        }
        await _dbContext.SaveChangesAsync();
    }
    public async Task UpdatePaymentStatusAsync()
    {
        var overduePayments = await _dbContext.BillPayment
            .Where(p => p.Status != PaymentStatus.OverduePayment && p.EndDate < DateTime.Now)
            .ToListAsync();
        foreach (var payment in overduePayments)
        {
            payment.Status = PaymentStatus.OverduePayment;
            _dbContext.BillPayment.Attach(payment);
            _dbContext.Entry(payment).State = EntityState.Modified;
        }

        await _dbContext.SaveChangesAsync();
    }
}