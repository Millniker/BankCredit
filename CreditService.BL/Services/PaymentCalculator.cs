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

        foreach (var billPayment in from loan in loans let paymentAmount = (decimal)loan.InterestRate * loan.Amount select new BillPayment
                 {
                     Id = new Guid(),
                     UserId = loan.UserId,
                     AccountId = loan.AccountId,
                     LoanId = loan.Id,
                     Amount = new Money(paymentAmount, loan.CurrencyType),
                     StartDate = DateTime.Now,
                     EndDate = DateTime.Now + TimeSpan.FromDays(30),
                     Status = PaymentStatus.AwaitPayment
                 })
        {
            _dbContext.BillPayment.Add(billPayment);
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
            _dbContext.Entry(payment).State = EntityState.Modified;
            _dbContext.BillPayment.Attach(payment);
        }

        await _dbContext.SaveChangesAsync();
    }
}