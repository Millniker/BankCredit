namespace CreditService.Common.DTO;

public class MetricDto
{
    public string ActionName { get; set; } 
    public string Code { get; set; }
    public string Message { get; set; }
    public string DeviceId { get; set; }
    public DateTimeOffset StartTime{get; set; }
    public DateTimeOffset EndTime { get; set; }
}