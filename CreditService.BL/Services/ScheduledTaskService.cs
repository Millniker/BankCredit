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
                Console.WriteLine("Пошла возьня");
                using var scope = _serviceProvider.CreateScope();
                var taskService = scope.ServiceProvider.GetRequiredService<PaymentCalculator>(); 
                var circusService = scope.ServiceProvider.GetRequiredService<RetryService>(); 
                await taskService.UpdatePaymentStatusAsync();
                await circusService.ChangStatus();
                               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while executing the scheduled task: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
