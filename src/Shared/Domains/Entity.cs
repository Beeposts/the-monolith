using Shared.Domains.Abstractions;

namespace Shared.Domains;

public abstract record Entity<TId> : IEntity<TId>
{
    public TId Id { get; set; }
}

public abstract record Entity : Entity<int>, IEntity
{
}