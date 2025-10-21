using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.Authorization;
using Database.Enumerations;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Database.Services;
using GraphQL.Client.Abstractions.Utilities;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Database.Utilities;

namespace Database.GraphQl.GetHttpsResources;

public static partial class GetHttpsResourceMutationsLogging
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Recomputing the hash value of the GET HTTPS resource with ID {Id} failed."
    )]
    public static partial void FailedRecomputingHashValue(this ILogger<GetHttpsResourceMutations> logger, Guid id, Exception exception);
}

[ExtendObjectType(nameof(Mutation))]
public sealed class GetHttpsResourceMutations
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    public async Task<CreateGetHttpsResourcePayload> CreateGetHttpsResourceAsync(
        CreateGetHttpsResourceInput input,
        ApplicationDbContext context,
        UserService userService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);
        if (currentUser is null)
        {
            return new CreateGetHttpsResourcePayload(
                new CreateGetHttpsResourceError(
                    CreateGetHttpsResourceErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }
        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new CreateGetHttpsResourcePayload(
                new CreateGetHttpsResourceError(
                    CreateGetHttpsResourceErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to create GET HTTPS resource in this database.",
                    []
                )
            );
        }

        IData? data = input.DataKind switch
        {
            DataKind.CALORIMETRIC_DATA => await context.CalorimetricData.AsQueryable().SingleOrDefaultAsync(d => d.Id == input.DataId, cancellationToken),
            DataKind.GEOMETRIC_DATA => await context.GeometricData.AsQueryable().SingleOrDefaultAsync(d => d.Id == input.DataId, cancellationToken),
            DataKind.HYGROTHERMAL_DATA => await context.HygrothermalData.AsQueryable().SingleOrDefaultAsync(d => d.Id == input.DataId, cancellationToken),
            DataKind.OPTICAL_DATA => await context.OpticalData.AsQueryable().SingleOrDefaultAsync(d => d.Id == input.DataId, cancellationToken),
            DataKind.PHOTOVOLTAIC_DATA => await context.PhotovoltaicData.AsQueryable().SingleOrDefaultAsync(d => d.Id == input.DataId, cancellationToken),
            _ => throw new ArgumentException($"The data kind {input.DataKind} is not supported.")
        };
        if (data is null)
        {
            return new CreateGetHttpsResourcePayload(
                new CreateGetHttpsResourceError(
                    CreateGetHttpsResourceErrorCode.UNKNOWN_DATA,
                    $"There is no data of kind {input.DataKind} with identifier {input.DataId}.",
                    [nameof(input), nameof(input.DataId).ToLowerFirst()]
                )
            );
        }

        var getHttpsResource = new GetHttpsResource(
            input.Description,
            Sha256FileHasher.ComputeForString(""), // The correct hash value is computed when the file for this resource is being uploaded.
            input.DataFormatId,
            input.DataKind == DataKind.CALORIMETRIC_DATA ? input.DataId : null,
            input.DataKind == DataKind.GEOMETRIC_DATA ? input.DataId : null,
            input.DataKind == DataKind.HYGROTHERMAL_DATA ? input.DataId : null,
            input.DataKind == DataKind.OPTICAL_DATA ? input.DataId : null,
            input.DataKind == DataKind.PHOTOVOLTAIC_DATA ? input.DataId : null,
            input.ParentId,
            input.ArchivedFilesMetaInformation.Select(i =>
                new FileMetaInformation(
                    i.Path,
                    i.DataFormatId
                )
            ).ToList(),
            input.AppliedConversionMethod is null
                ? null
                : new ToTreeVertexAppliedConversionMethod(
                    input.AppliedConversionMethod.MethodId,
                    input.AppliedConversionMethod.Arguments.Select(a =>
                        new NamedMethodArgument(
                            a.Name,
                            a.Value
                        )
                    ).ToList(),
                    input.AppliedConversionMethod.SourceName
                )
        );
        context.GetHttpsResources.Add(getHttpsResource);
        await context.SaveChangesAsync(cancellationToken);
        return new CreateGetHttpsResourcePayload(getHttpsResource);
    }

    public async Task<RecomputeGetHttpsResourceHashValuesPayload> RecomputeGetHttpsResourceHashValuesAsync(
        ApplicationDbContext context,
        UserService userService,
        ILogger<GetHttpsResourceMutations> logger,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);
        if (currentUser is null)
        {
            return new RecomputeGetHttpsResourceHashValuesPayload(
                new RecomputeGetHttpsResourceHashValuesError(
                    RecomputeGetHttpsResourceHashValuesErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }
        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new RecomputeGetHttpsResourceHashValuesPayload(
                new RecomputeGetHttpsResourceHashValuesError(
                    RecomputeGetHttpsResourceHashValuesErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to recompute GET HTTPS resource hash values in this database.",
                    []
                )
            );
        }
        var resources = await context.GetHttpsResources.ToListAsync(cancellationToken);
        var errors = new ConcurrentBag<RecomputeGetHttpsResourceHashValuesError>();
        await Parallel.ForEachAsync(
            resources,
            cancellationToken,
            async (resource, cancellationToken) =>
            {
                try
                {
                    await resource.RecomputeHashValue(cancellationToken);
                }
                catch (Exception exception)
                {
                    logger.FailedRecomputingHashValue(resource.Id, exception);
                    errors.Add(
                        new RecomputeGetHttpsResourceHashValuesError(
                            RecomputeGetHttpsResourceHashValuesErrorCode.FAILED,
                            $"Recomputing the hash value for the GET HTTPS resource with ID {resource.Id} failed with message: {exception.Message}",
                            []
                        )
                    );
                }

            }
        );
        await context.SaveChangesAsync(cancellationToken);
        if (!errors.IsEmpty)
        {
            return new RecomputeGetHttpsResourceHashValuesPayload(resources, errors);
        }
        return new RecomputeGetHttpsResourceHashValuesPayload(resources);
    }
}