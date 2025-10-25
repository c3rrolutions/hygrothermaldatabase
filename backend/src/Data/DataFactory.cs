using System;
using Database.Enumerations;

namespace Database.Data;

public sealed class DataFactory
{
    public static T Create<T>(
        Guid userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTime createdAt,
        AppliedMethod appliedMethod
    )
    where T : IData, ICreateData<T>
    {
        return T.Create(
            userId,
            locale,
            componentId,
            name,
            description,
            warnings,
            creatorId,
            createdAt,
            appliedMethod
        );
    }

    public static IData Create(
        DataKind dataKind,
        Guid userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTime createdAt,
        AppliedMethod appliedMethod
    )
    {
        return dataKind switch
        {
            DataKind.CALORIMETRIC_DATA => Create<CalorimetricData>(
                userId,
                locale,
                componentId,
                name,
                description,
                warnings,
                creatorId,
                createdAt,
                appliedMethod
            ),
            DataKind.GEOMETRIC_DATA => Create<GeometricData>(
                userId,
                locale,
                componentId,
                name,
                description,
                warnings,
                creatorId,
                createdAt,
                appliedMethod
            ),
            DataKind.HYGROTHERMAL_DATA => Create<HygrothermalData>(
                userId,
                locale,
                componentId,
                name,
                description,
                warnings,
                creatorId,
                createdAt,
                appliedMethod
            ),
            DataKind.OPTICAL_DATA => Create<OpticalData>(
                userId,
                locale,
                componentId,
                name,
                description,
                warnings,
                creatorId,
                createdAt,
                appliedMethod
            ),
            DataKind.PHOTOVOLTAIC_DATA => Create<PhotovoltaicData>(
                userId,
                locale,
                componentId,
                name,
                description,
                warnings,
                creatorId,
                createdAt,
                appliedMethod
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(dataKind), $"Unsupported data kind {dataKind}.")
        };
    }
}