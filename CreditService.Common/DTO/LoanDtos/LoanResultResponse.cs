using System.Net;
using CreditService.DAL.Enum;

namespace CreditService.Common.DTO;

public class LoanResultResponse
{ 
        public string LoanStatus { get; set; }
        public HttpStatusCode AccountCode { get; set; }
        public string AccountStatus { get; set;}
}