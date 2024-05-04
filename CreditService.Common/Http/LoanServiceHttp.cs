using System.Net;
using System.Net.Http.Json;
using CreditService.Common.DTO;
using CreditService.Common.DTO.LoanAppDtos;
using CreditService.Common.Exceptions;
using CreditService.Common.System;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CreditService.Common.Http;

public class LoanServiceHttp
{
    private readonly HttpClient _httpClient;

    public LoanServiceHttp(IOptions<HttpClient> httpClient)
    {
        _httpClient = httpClient.Value;
    }
    public async Task<CreditScoreDto> getCreditScore(int userId)
    {
        var responseCreditScore = await _httpClient.GetAsync(ApiConstants.CreditScore + userId);
        if (responseCreditScore.StatusCode != (HttpStatusCode)200)
            throw new ErrorReceivingDecisionException("Ошибка получения ответа по заявке на кредит");
        var responseCreditScoreObject =
            JsonConvert.DeserializeObject<CreditScoreDto>(responseCreditScore.Content.ReadAsStringAsync().Result);
        return responseCreditScoreObject;
    }

    public async Task<LoanResponse> SendOnCheckLoan(ToDesicionDto toDesicionDto)
    {
        var response = await _httpClient.PostAsJsonAsync(ApiConstants.SendOnCheckLoanBaseUrl, toDesicionDto);
        if (response.StatusCode != (HttpStatusCode)200)
            throw new ErrorReceivingDecisionException("Ошибка получения ответа по заявке на кредит");
        var responseObject = JsonConvert.DeserializeObject<LoanResponse>(response.Content.ReadAsStringAsync().Result);
        return responseObject;
    }
}