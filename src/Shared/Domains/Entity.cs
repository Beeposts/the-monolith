using Shared.Domains.Abstractions;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Shared.Domains;

public abstract record Entity<TId> : IEntity<TId>
{
    public TId Id { get; set; }
}

public abstract record Entity : Entity<int>, IEntity
{
}