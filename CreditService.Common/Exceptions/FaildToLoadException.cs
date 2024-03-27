namespace CreditService.Common.Exceptions;

public class FaildToLoadException:Exception
{
    public FaildToLoadException() { }

    public FaildToLoadException(string message) : base(message) { }

    public FaildToLoadException(string message, Exception innerException) : base(message, innerException) { }

}