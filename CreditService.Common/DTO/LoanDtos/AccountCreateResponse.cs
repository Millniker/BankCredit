using System.Net;

namespace CreditService.Common.DTO;

public class AccountCreateResponse
{
    public HttpStatusCode Code { get; set;}
    
    public string Message { get; set; }
    public int  AccountId { get; set; }
}