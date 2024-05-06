using CreditService.BL.Http;
using CreditService.BL.Services;
using CreditService.Common.Http;
using CreditService.Common.Interfaces;
using CreditService.Common.System;
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
            builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connection).EnableSensitiveDataLogging());
        } 
        public static void ConfigureAppServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICreditService, CreditRulesService>();
            builder.Services.AddScoped<ILoanService, LoanService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddHostedService<ScheduledTaskService>();
            builder.Services.AddScoped<PaymentCalculator>();
            builder.Services.AddScoped<AccountHttp>();
            builder.Services.AddScoped<LoanServiceHttp>();
            builder.Services.AddScoped<LoggerService>();
            builder.Services.AddScoped<RetryService>();
            builder.Services.AddScoped<ThrowException>();

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
