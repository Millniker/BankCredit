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

        foreach (var billPayment in from loan in loans let existingPayments = _dbContext.BillPayment
                     .Where(bp => bp.AccountId == loan.AccountId)
                     .ToList() where existingPayments.Count < loan.Term && loan.Amount > 0 select new BillPayment
                 {
                     Id = Guid.NewGuid(),
                     UserId = loan.UserId,
                     AccountId = loan.AccountId,
                     LoanId = loan.Id,
                     Amount = new Money((decimal)loan.InterestRate/100 * loan.Amount, loan.CurrencyType),
                     StartDate = DateTime.UtcNow,
                     EndDate = DateTime.UtcNow.AddMinutes(10),
                     Status = PaymentStatus.AwaitPayment
                 })
        {
            _dbContext.BillPayment.Add(billPayment);
        }
        await _dbContext.SaveChangesAsync();
    }
    public async Task UpdatePaymentStatusAsync()
    {
        var payments = await _dbContext.BillPayment.ToListAsync();
        await CalculateAndSendPaymentsAsync();
	Console.WriteLine(payments.Count);
        if (payments.Count > 0)
        {
            var overduePayments = await _dbContext.BillPayment
                .Where(p => p.Status != PaymentStatus.OverduePayment && p.Status != PaymentStatus.Paid && p.EndDate < DateTime.UtcNow)
                .ToListAsync();
            Console.WriteLine(overduePayments);
            foreach (var payment in overduePayments)
            {
                payment.Status = PaymentStatus.OverduePayment;
                _dbContext.BillPayment.Attach(payment);
                _dbContext.Entry(payment).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

            };
	        await _dbContext.SaveChangesAsync();

        }
    }
}
