using System;

namespace Database.Data;

public interface ICreateData<TData> where TData : IData
{
    static abstract TData Create(
        Guid userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTime createdAt,
        AppliedMethod appliedMethod
    );
}