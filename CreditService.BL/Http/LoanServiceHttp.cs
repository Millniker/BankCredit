using System.Net;
using System.Net.Http.Json;
using CreditService.BL.Services;
using CreditService.Common.DTO;
using CreditService.Common.DTO.LoanAppDtos;
using CreditService.Common.Exceptions;
using CreditService.Common.System;
using CreditService.DAL;
using CreditService.DAL.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CreditService.Common.Http;

public class LoanServiceHttp
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _context;
    private readonly RetryService _retryService;

    public LoanServiceHttp(IOptions<HttpClient> httpClient,AppDbContext context, RetryService retryService)
    {
        _httpClient = httpClient.Value;
        _context = context;
        _retryService = retryService;
    }
    public async Task<CreditScoreDto> getCreditScore(int userId, string requestId, string deviceId)
    {
        var circuc =await _context.CircuitBreaker.FirstOrDefaultAsync();
        while (circuc.CircuitBreakerStatus != CircuitBreakerStatus.Open)
        {
            try
            {
                if (circuc.CircuitBreakerStatus == CircuitBreakerStatus.HalfOpen && new Random().Next(0, 1) == 0)
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, ApiConstants.CreditScore + userId);
                    requestMessage.Headers.Add("RequestId", requestId);
                    requestMessage.Headers.Add("DeviceId", deviceId);
                    var responseCreditScore = await _httpClient.SendAsync(requestMessage);
                    await _retryService.AddRequest();
                    if (responseCreditScore.StatusCode != (HttpStatusCode)200)
                        throw new ErrorReceivingDecisionException("Ошибка получения ответа по заявке на кредит");
                    var responseCreditScoreObject =
                        JsonConvert.DeserializeObject<CreditScoreDto>(responseCreditScore.Content.ReadAsStringAsync()
                            .Result);
                    return responseCreditScoreObject;
                }

                if (circuc.CircuitBreakerStatus == CircuitBreakerStatus.Closed)
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, ApiConstants.CreditScore + userId);
                    requestMessage.Headers.Add("RequestId", requestId);
                    requestMessage.Headers.Add("DeviceId", deviceId);
                    var responseCreditScore = await _httpClient.SendAsync(requestMessage);
                    await _retryService.AddRequest();
                    if (responseCreditScore.StatusCode != (HttpStatusCode)200)
                        throw new ErrorReceivingDecisionException("Ошибка получения ответа по заявке на кредит");
                    var responseCreditScoreObject =
                        JsonConvert.DeserializeObject<CreditScoreDto>(responseCreditScore.Content.ReadAsStringAsync()
                            .Result);
                    return responseCreditScoreObject;
                }
            }
            catch (Exception ex)
            {
                await _retryService.AddException();
            }
            finally
            {
                await _retryService.ChangStatus();
            }
        }
        throw new FaildToLoadException("Не удалось открыть счет ");
    }           

    public async Task<LoanResponse> SendOnCheckLoan(ToDesicionDto toDesicionDto)
    {var circuc =await _context.CircuitBreaker.FirstOrDefaultAsync();
        while (circuc.CircuitBreakerStatus != CircuitBreakerStatus.Open)
        {

            try
            {
                if (circuc.CircuitBreakerStatus == CircuitBreakerStatus.HalfOpen && new Random().Next(0, 1) == 0)
                {
                    var response =
                        await _httpClient.PostAsJsonAsync(ApiConstants.SendOnCheckLoanBaseUrl, toDesicionDto);
                    if (response.StatusCode != (HttpStatusCode)200)
                        throw new ErrorReceivingDecisionException("Ошибка получения ответа по заявке на кредит");
                    await _retryService.AddRequest();
                    var responseObject =
                        JsonConvert.DeserializeObject<LoanResponse>(response.Content.ReadAsStringAsync().Result);
                    return responseObject;
                }

                if (circuc.CircuitBreakerStatus == CircuitBreakerStatus.Closed)
                {
                    var response =
                        await _httpClient.PostAsJsonAsync(ApiConstants.SendOnCheckLoanBaseUrl, toDesicionDto);
                    if (response.StatusCode != (HttpStatusCode)200)
                        throw new ErrorReceivingDecisionException("Ошибка получения ответа по заявке на кредит");
                    await _retryService.AddRequest();
                    var responseObject =
                        JsonConvert.DeserializeObject<LoanResponse>(response.Content.ReadAsStringAsync().Result);
                    return responseObject;
                }
            }
            catch (Exception ex)
            {
                await _retryService.AddException();
            }
            finally
            {
                await _retryService.ChangStatus();
            }
        }
        throw new FaildToLoadException("Не удалось открыть счет");
    }
}