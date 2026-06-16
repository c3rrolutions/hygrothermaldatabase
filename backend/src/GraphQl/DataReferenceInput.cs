using System;
using Database.Data;
using Database.Enumerations;

namespace Database.GraphQl;

public sealed record DataReferenceInput(
    Guid DataId,
    DataKind DataKind
)
    : IIdentifyDataInput
{
    public DataReference ToDomainModel()
    {
        return new DataReference(
            DataId,
            DataKind
        );
    }
};