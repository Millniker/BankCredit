using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditService.DAL.Entities;

public class IdempotencyId
{    [Key]
    public Guid Id { get; set; }

    [Required]
    public DateTime CreateAt { get; set; }

    public DateTime? LockedAt { get; set; }

    [Required]
    public string IdempotencyKey { get; set; }

    public Guid HttpExchangeDataId { get; set; }

}