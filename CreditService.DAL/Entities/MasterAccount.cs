using System.ComponentModel.DataAnnotations;

namespace CreditService.DAL.Entities;

public class MasterAccount
{
    [Key]
    public Guid Id { get; set; }
    public List<Money> Money { get; set; }
}