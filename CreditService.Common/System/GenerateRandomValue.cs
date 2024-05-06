namespace CreditService.Common.System;

public class ThrowException
{
    public bool GenerateRandomValue()
    {
        DateTime currentTime = DateTime.Now;

        bool isOddMinute = currentTime.Minute % 2 != 0;

        int randomNumber = new Random().Next(100);

        if (isOddMinute)
        {
            return randomNumber < 50;
        }
        else
        {
            return randomNumber < 90;
        }
    }
}
