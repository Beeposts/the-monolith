using Shared.Domains.Abstractions;

namespace Shared.Domains;

public abstract record Entity<TId> : IEntity<TId>
{
    public required TId Id { get; set; }
}

public abstract record Entity : Entity<int>, IEntity
{
}