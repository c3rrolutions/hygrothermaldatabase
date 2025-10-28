using System;

namespace Database.Data;

public interface IEntity
{
    Guid Id { get; }

    uint Version { get; } // https://www.npgsql.org/efcore/modeling/concurrency.html
}