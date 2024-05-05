namespace CreditService.DAL.Entities;

public interface IIdentity<TKey>
{
    TKey Id { get; set; }
}