using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using CreditService.Common.DTO;
using CreditService.Common.DTO.Payment;
using CreditService.Common.Exceptions;
using CreditService.Common.System;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CreditService.Common.Http;

public class AccountHttp
{
    private readonly HttpClient _httpClient;

    public AccountHttp(IOptions<HttpClient> httpClient)
    {
        _httpClient = httpClient.Value;
    }
    public async Task<AccountCreateResponse> openAccount(int userId, CreateAccountDto createAccountDTO)
    {
        int retryCount = 0;
        int errorCount = 0;
        bool circuitOpen = false;

        while (true)
        {
            try
            {
                if (circuitOpen)
                {
                    
                    await Task.Delay(5000); 
                    circuitOpen = false;
                }

                var response = await _httpClient.PostAsJsonAsync(ApiConstants.OpenAccountBaseUrl+"/"+userId, createAccountDTO);
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new FaildToLoadException("Не удалось открыть счет "+ response.Content.ReadAsStringAsync().Result);
        
                var account = JsonConvert.DeserializeObject<AccountDto>(response.Content.ReadAsStringAsync().Result);
                retryCount = 0;
                errorCount = 0;
                return new AccountCreateResponse
                {
                    Code = response.StatusCode,
                    Message = response.ReasonPhrase,
                    AccountId = account.Id
                };
            }
            catch (Exception ex)
            {
                errorCount++;
        
                if (errorCount > 3)
                {
                    // Если количество ошибок больше 3, активируем Circuit Breaker
                    circuitOpen = true;
                }

                if (errorCount > 5)
                {
                    // Если количество ошибок больше 5, выбрасываем исключение
                    throw new Exception("Превышено максимальное количество попыток");
                }

                retryCount++;
        
                if (retryCount > 3)
                {
                    // Если количество попыток больше 3, делаем паузу перед следующей попыткой
                    await Task.Delay(500); // Пауза в 0.5 секунды (можно изменить)
                }
            }
        }

    }

    public async Task<bool> DeleteAccount(int accountId)
    {
        var closeAccount =  await _httpClient.DeleteAsync(ApiConstants.CloseAccountBaseUrl+"/"+ accountId);
        return closeAccount.IsSuccessStatusCode;
    }
    public async Task<object> DepositMoney(WithdrawDepositDTO requestBody)
    {
        var transaction = new TransactionDto
        {
            Id = GenerateTransactionId(), // Здесь нужно сгенерировать уникальный идентификатор для транзакции
            Amount = requestBody.Amount.ToString(),
            FromAccountId = null,
            ToAccountId = requestBody.AccountId,
            TransactionType = "DEPOSIT", // Предполагая, что это метод для пополнения счета
            TransactionDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"),
            CurrencyType = requestBody.CurrencyType.ToString()
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        var json = JsonSerializer.Serialize(transaction, options);

        await SendMessageToRabbitMq(json);

        return new { Message = "Transaction created and sent to RabbitMQ." };
    }
    private static Task SendMessageToRabbitMq(string message)
    {
        var factory = new ConnectionFactory { HostName = "92.63.100.27", Port = 5672, UserName = "user", Password = "password" };

        try
        {
            using var connection =  factory.CreateConnection();
            using var channel =  connection.CreateModel();
             channel.QueueDeclare(queue: "transactionQueue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var body = Encoding.UTF8.GetBytes(message);
            var basicProperties = channel.CreateBasicProperties();
             channel.BasicPublish(exchange: "",
                routingKey: "transactionQueue",
                basicProperties: null,
                body: body);
        } catch (Exception e)
        {
            Console.WriteLine("Error while sending notification to RabbitMQ: {Message}", e.Message);
        }

        return Task.CompletedTask;
    }
    
    private int GenerateTransactionId()
    {
        // Здесь реализуй генерацию уникального идентификатора для транзакции
        // Например, можешь использовать Guid.NewGuid().ToString() для простой генерации
        return new Random().Next(1000000, 9999999); // Пример простой генерации
    }
    public async Task<object> WithdrowMoney(WithdrawDepositDTO requestBody)
    {
        var transaction = new TransactionDto
        {
            Id = GenerateTransactionId(), // Здесь нужно сгенерировать уникальный идентификатор для транзакции
            Amount = requestBody.Amount.ToString(),
            FromAccountId = null,
            ToAccountId = requestBody.AccountId,
            TransactionType = "WITHDROW", // Предполагая, что это метод для пополнения счета
            TransactionDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"),
            CurrencyType = requestBody.CurrencyType.ToString()
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        var json = JsonSerializer.Serialize(transaction, options);

        await SendMessageToRabbitMq(json);

        return new { Message = "Transaction created and sent to RabbitMQ." };
    }

    public async Task<AccountDto> GetAccount(int accountId)
    {
        var accountResponse = await _httpClient.GetAsync(ApiConstants.Account+"/"+accountId);
        if (!accountResponse.IsSuccessStatusCode)
            throw new FaildToLoadException("Ошибка получения счета");
        var account = JsonConvert.DeserializeObject<AccountDto>(accountResponse.Content.ReadAsStringAsync().Result);
        return account;
    }
}