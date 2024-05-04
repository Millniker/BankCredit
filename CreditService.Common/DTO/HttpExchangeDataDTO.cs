using System.ComponentModel.DataAnnotations;

namespace CreditService.Common.DTO;

public class HttpExchangeDataDTO
{
    public Guid Id { get; set; }

    [Required]
    public DateTime RequestDate { get; set; }

    [Required]
    public string RequestPath { get; set; }

    [Required]
    public string RequestMethod { get; set; }

    public string RequestBody { get; set; }

    public string RequestHeaders { get; set; }

    [Required]
    public int ResponseCode { get; set; }

    public string ResponseBody { get; set; }

    public string ResponseHeaders { get; set; }
}