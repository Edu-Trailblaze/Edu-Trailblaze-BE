namespace Contracts.Domain.Interfaces
{
    public interface IEntityBase<T>
    {
        T Id { get; set; }
    }
}
