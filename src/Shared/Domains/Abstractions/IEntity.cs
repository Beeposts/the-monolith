namespace Shared.Domains.Abstractions;

public interface IEntity<TId>
{
    TId Id { get; set; }
}

public interface IEntity : IEntity<int>
{
}