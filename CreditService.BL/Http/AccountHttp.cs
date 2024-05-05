using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using CreditService.BL.Services;
using CreditService.Common.DTO;
using CreditService.Common.DTO.Payment;
using CreditService.Common.Exceptions;
using CreditService.Common.System;
using CreditService.DAL;
using CreditService.DAL.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CreditService.BL.Http;

public class AccountHttp
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _context;
    private readonly RetryService _retryService;

    public AccountHttp(IOptions<HttpClient> httpClient,AppDbContext context, RetryService retryService)
    {
        _httpClient = httpClient.Value;
        _context = context;
        _retryService = retryService;
    }
    public async Task<AccountCreateResponse> OpenAccount(int userId, CreateAccountDto createAccountDTO, string requestId, string deviceId)
    {
        var circuc =await _context.CircuitBreaker.FirstOrDefaultAsync();
        while (circuc.CircuitBreakerStatus != CircuitBreakerStatus.Open)
        {
            try
            {
                if (circuc.CircuitBreakerStatus == CircuitBreakerStatus.HalfOpen && new Random().Next(0, 2) == 0)
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, ApiConstants.OpenAccountBaseUrl + "/" + userId);
                    requestMessage.Headers.Add("RequestId", requestId);
                    requestMessage.Headers.Add("DeviceId", deviceId);
                    requestMessage.Content = new StringContent(JsonConvert.SerializeObject(createAccountDTO), Encoding.UTF8, "application/json");
                    var response = await _httpClient.SendAsync(requestMessage);
                    await _retryService.AddRequest();
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new FaildToLoadException("Не удалось открыть счет " +
                                                       response.Content.ReadAsStringAsync().Result);

                    var account =
                        JsonConvert.DeserializeObject<AccountDto>(response.Content.ReadAsStringAsync().Result);
                    return new AccountCreateResponse
                    {
                        Code = response.StatusCode,
                        Message = response.ReasonPhrase,
                        AccountId = account.Id
                    };
                }
                if (circuc.CircuitBreakerStatus == CircuitBreakerStatus.Closed)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true
                    };
                    var json = JsonSerializer.Serialize(createAccountDTO, options);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    content.Headers.Add("RequestId", requestId);
                    content.Headers.Add("DeviceId", deviceId);
                    var url = ApiConstants.OpenAccountBaseUrl+userId;
                    var response = await  _httpClient.PostAsync(url, content);
                    await _retryService.AddRequest();
                    if (!response.IsSuccessStatusCode)
                        throw new FaildToLoadException("Не удалось открыть счет " +
                                                       response.Content.ReadAsStringAsync().Result);

                    var account =
                        JsonConvert.DeserializeObject<AccountDto>(response.Content.ReadAsStringAsync().Result);
                    return new AccountCreateResponse
                    {
                        Code = response.StatusCode,
                        Message = response.ReasonPhrase,
                        AccountId = account.Id
                    };
                }
            }
            catch (Exception ex)
            {
                await _retryService.AddException();
            }
            finally
            {
                await _retryService.ChangStatus();            }
        }
        throw new FaildToLoadException("Не удалось открыть счет");
        
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
        var factory = new ConnectionFactory { HostName = "94.154.11.188", Port = 5672, UserName = "user", Password = "password" };

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
            TransactionType = "WITHDRAWAL", // Предполагая, что это метод для пополнения счета
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

    public async Task<AccountDto> GetAccount(int accountId, string requestId, string deviceId)
    {
        var circuc = await _context.CircuitBreaker.FirstOrDefaultAsync();
        while (circuc.CircuitBreakerStatus != CircuitBreakerStatus.Open)
        {
            try
            {
                if (circuc.CircuitBreakerStatus == CircuitBreakerStatus.HalfOpen && new Random().Next(0, 1) == 0)
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, ApiConstants.Account + "/" + accountId);
                    requestMessage.Headers.Add("RequestId", requestId);
                    requestMessage.Headers.Add("DeviceId", deviceId);
                    var responseCreditScore = await _httpClient.SendAsync(requestMessage);
                    if (!responseCreditScore.IsSuccessStatusCode)
                        throw new FaildToLoadException("Ошибка получения счета");
                    var account =
                        JsonConvert.DeserializeObject<AccountDto>(responseCreditScore.Content.ReadAsStringAsync().Result);
                    await _retryService.AddRequest();
                    return account;
                }

                if (circuc.CircuitBreakerStatus == CircuitBreakerStatus.Closed)
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, ApiConstants.Account + "/" + accountId);
                    requestMessage.Headers.Add("RequestId", requestId);
                    requestMessage.Headers.Add("DeviceId", deviceId);
                    var responseCreditScore = await _httpClient.SendAsync(requestMessage);
                    if (!responseCreditScore.IsSuccessStatusCode)
                        throw new FaildToLoadException("Ошибка получения счета");
                    var account =
                        JsonConvert.DeserializeObject<AccountDto>(responseCreditScore.Content.ReadAsStringAsync().Result);
                    await _retryService.AddRequest();
                    return account;
                }
            }
            catch (Exception exception)
            {
                await _retryService.AddException();
            }
            finally
            {
                await _retryService.ChangStatus();
            }
        }
        throw new FaildToLoadException("Не удалось оплатить счет");

    }
}