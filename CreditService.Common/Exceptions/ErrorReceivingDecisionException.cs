namespace CreditService.Common.Exceptions;

public class ErrorReceivingDecisionException:Exception
{
    public ErrorReceivingDecisionException() { }

    public ErrorReceivingDecisionException(string message) : base(message) { }

    public ErrorReceivingDecisionException(string message, Exception innerException) : base(message, innerException) { }
}