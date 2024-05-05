using System.Net.Http.Json;
using CreditService.BL.Services;
using CreditService.Common.DTO;
using CreditService.Common.System;
using CreditService.DAL;
using Microsoft.Extensions.Options;

namespace CreditService.BL.Http;

public class MetricHttp
{
    private readonly HttpClient _httpClient;
    public MetricHttp(IOptions<HttpClient> httpClient)
    {
        _httpClient = httpClient.Value;

        
    }
    public async Task SendMetric(MetricDto metric)
    {
        var response = await _httpClient.PostAsJsonAsync(ApiConstants.MetricBaseUrl, metric);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error sending metric");
        }
    }
}