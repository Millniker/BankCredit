using CreditService.BL.Services;
using CreditService.Common.Interfaces;
using CreditService.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CreditService.BL;

    public static class Configurator
    {
        public static void ConfigureAppDb(this WebApplicationBuilder builder)
        {
            var connection = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connection));
        } 
        public static void ConfigureAppServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICreditService, CreditRulesService>();
            builder.Services.AddScoped<ILoanService, LoanService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            
        }
        
        public static void Migrate(IServiceProvider serviceProvider)
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        
                       
                            dbContext.Database.Migrate();
                        
                    }
                }
    }
