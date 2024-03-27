using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CreditService.BL.Services;

public class ScheduledTaskService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public ScheduledTaskService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var taskService = scope.ServiceProvider.GetRequiredService<PaymentCalculator>();
                await taskService.UpdatePaymentStatusAsync();
                await taskService.CalculateAndSendPaymentsAsync();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while executing the scheduled task: {ex.Message}");
            }

            // Пауза перед следующим запуском
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
}