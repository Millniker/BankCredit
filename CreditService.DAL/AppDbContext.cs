using CreditService.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CreditService.DAL;

public class AppDbContext: DbContext
{
    public DbSet<CreditRules> CreditRules { get; set; }
    
    public DbSet<Loan> Loan { get; set; }
    
    public DbSet<LoanApp> LoanApp { get; set; }
    public DbSet<BillPayment> BillPayment { get; set; }
    public DbSet<HttpExchangeData> HttpExchangeData { get; set; }
    public DbSet<IdempotencyId> IdempotencyId { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<CreditRules>().OwnsOne(c => c.AmountMax);
            builder.Entity<CreditRules>().OwnsOne(c => c.AmountMin);
            builder.Entity<BillPayment>().OwnsOne(c => c.Amount);
            
        }
}