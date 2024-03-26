using CreditService.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CreditService.DAL;

public class AppDbContext: DbContext
{
    public DbSet<CreditRules> CreditRules { get; set; }
    
    public DbSet<Loan> Loan { get; set; }
    
    public DbSet<LoanApp> LoanApp { get; set; }
    public DbSet<MasterAccount> MasterAccount { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<CreditRules>().OwnsOne(c => c.AmountMax);
            builder.Entity<CreditRules>().OwnsOne(c => c.AmountMin);
            builder.Entity<MasterAccount>().OwnsOne(c => c.Money);
            
        }
}