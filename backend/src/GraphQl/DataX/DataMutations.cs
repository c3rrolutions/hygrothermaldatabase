using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Enumerations;
using Database.Services;
using Database.Utilities;
using GraphQL.Client.Abstractions.Utilities;
using HotChocolate;
using HotChocolate.Types;
using static Database.ApiRequests.QueryCurrentUser;

namespace Database.GraphQl.DataX;

[ExtendObjectType(nameof(Mutation))]
public sealed class DataMutations
: DataMutationsBase
{
    // public sealed record CreateDataInput(
    //     [GraphQLType<NonNullType<LocaleType>>] string Locale,
    //     DataKind DataKind,
    //     Guid ComponentId,
    //     string? Name,
    //     string? Description,
    //     string[] Warnings,
    //     DateTime CreatedAt,
    //     Guid CreatorId,
    //     AppliedMethodInput AppliedMethod,
    //     RootGetHttpsResourceInput RootResource
    // );

    // [SuppressMessage("Naming", "CA1707")]
    // public enum CreateDataErrorCode
    // {
    //     UNAUTHENTICATED,
    //     UNAUTHORIZED,
    //     UNKNOWN_DATA,
    //     CREATING_RESPONSE_APPROVAL_FAILED
    // }

    // public sealed record CreateDataError(
    //     CreateDataErrorCode Code,
    //     string Message,
    //     IReadOnlyList<string> Path
    // ) : UserErrorBase<CreateDataErrorCode>(Code, Message, Path);

    // public sealed record CreateDataPayload(
    //     IData? Data,
    //     IReadOnlyCollection<CreateDataError>? Errors
    // );

    // public Task<CreateDataPayload> CreateDataAsync(
    //     CreateDataInput input,
    //     ApplicationDbContext context,
    //     UserService userService,
    //     ResponseApprovalService responseApprovalService,
    //     CancellationToken cancellationToken
    // )
    // {
    //     static CreateDataPayload NewPayload(
    //         IData? data,
    //         IReadOnlyCollection<CreateDataError>? errors
    //     ) => new(data, errors);
    //     static CreateDataError NewError(
    //         CreateDataErrorCode code,
    //         string message,
    //         IReadOnlyList<string> path
    //     ) => new(code, message, path);
    //     return Authorize(
    //         unauthenticatedErrorCode: CreateDataErrorCode.UNAUTHENTICATED,
    //         unauthorizedErrorCode: CreateDataErrorCode.UNAUTHORIZED,
    //         NewPayload,
    //         NewError,
    //         userService,
    //         then: async currentUser =>
    //         {
    //             var data = DataFactory.Create(
    //                 input.DataKind,
    //                 currentUser.Uuid,
    //                 input.Locale,
    //                 input.ComponentId,
    //                 input.Name,
    //                 input.Description,
    //                 input.Warnings,
    //                 input.CreatorId,
    //                 input.CreatedAt,
    //                 new AppliedMethod(
    //                     input.AppliedMethod.MethodId,
    //                     input.AppliedMethod.Arguments
    //                         .Select(a => new NamedMethodArgument(
    //                             a.Name,
    //                             a.Value
    //                         ))
    //                         .ToList(),
    //                     input.AppliedMethod.Sources
    //                         .Select(s => new NamedMethodSource(
    //                             s.Name,
    //                             new CrossDatabaseDataReference(
    //                                 s.Value.DataId,
    //                                 s.Value.DataTimestamp,
    //                                 s.Value.DataKind,
    //                                 s.Value.DatabaseId
    //                             )
    //                         ))
    //                         .ToList()
    //                 )
    //             );
    //             var resource = new GetHttpsResource(
    //                 input.RootResource.Description,
    //                 Sha256FileHasher.ComputeForString(""), // The correct hash value is computed when the file for this resource is being uploaded.
    //                 input.RootResource.DataFormatId,
    //                 null,
    //                 input.RootResource.ArchivedFilesMetaInformation.Select(i =>
    //                     new FileMetaInformation(
    //                         i.Path,
    //                         i.DataFormatId
    //                     )
    //                 ).ToList(),
    //                 input.RootResource.AppliedConversionMethod is null
    //                     ? null
    //                     : new ToTreeVertexAppliedConversionMethod(
    //                         input.RootResource.AppliedConversionMethod.MethodId,
    //                         input.RootResource.AppliedConversionMethod.Arguments.Select(a =>
    //                             new NamedMethodArgument(
    //                                 a.Name,
    //                                 a.Value
    //                             )
    //                         ).ToList(),
    //                         input.RootResource.AppliedConversionMethod.SourceName
    //                     )
    //             );
    //             data.Resources.Add(resource);
    //             switch (input.DataKind)
    //             {
    //                 case DataKind.CALORIMETRIC_DATA:
    //                     context.CalorimetricData.Add(data);
    //                     break;
    //                 case DataKind.GEOMETRIC_DATA:
    //                     context.GeometricData.Add(data);
    //                     break;
    //                 case DataKind.HYGROTHERMAL_DATA:
    //                     context.HygrothermalData.Add(data);
    //                     break;
    //                 case DataKind.OPTICAL_DATA:
    //                     context.OpticalData.Add(data);
    //                     break;
    //                 case DataKind.PHOTOVOLTAIC_DATA:
    //                     context.PhotovoltaicData.Add(data);
    //                     break;
    //                 default:
    //                     throw new NotSupportedException($"Unsupported data kind {input.DataKind}.");
    //             }
    //             await context.SaveChangesAsync(cancellationToken);
    //             try
    //             {
    //                 data.Approval = await responseApprovalService.CreateResponseApproval(data, cancellationToken);
    //                 await context.SaveChangesAsync(cancellationToken);
    //             }
    //             catch (Exception exception)
    //             {
    //                 context.Remove(data);
    //                 await context.SaveChangesAsync(cancellationToken);
    //                 return NewPayload(
    //                     data,
    //                     [NewError(
    //                         CreateDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
    //                         $"Signing failed with message: {exception.Message}",
    //                         []
    //                     )]
    //                 );
    //             }
    //             return NewPayload(data, null);
    //         },
    //         cancellationToken
    //     );
    // }

    public sealed record UpdateDataInput(
        [GraphQLType<NonNullType<LocaleType>>] string Locale,
        Guid DataId,
        DataKind DataKind,
        Guid ComponentId,
        string? Name,
        string? Description,
        string[] Warnings,
        DateTime CreatedAt,
        Guid CreatorId
    ) : IChangeDataInput;

    [SuppressMessage("Naming", "CA1707")]
    public enum UpdateDataErrorCode
    {
        UNAUTHENTICATED,
        UNAUTHORIZED,
        UNKNOWN_DATA,
        CREATING_RESPONSE_APPROVAL_FAILED
    }

    public sealed record UpdateDataError(
        UpdateDataErrorCode Code,
        string Message,
        IReadOnlyList<string> Path
    ) : UserErrorBase<UpdateDataErrorCode>(Code, Message, Path);

    public sealed record UpdateDataPayload(
        IData? data,
        IReadOnlyCollection<UpdateDataError>? Errors
    );

    public Task<UpdateDataPayload> UpdateDataAsync(
        UpdateDataInput input,
        ApplicationDbContext context,
        UserService userService,
        ResponseApprovalService responseApprovalService,
        CancellationToken cancellationToken
    )
    {
        static UpdateDataPayload NewPayload(
            IData? data,
            IReadOnlyCollection<UpdateDataError>? errors
        ) => new(data, errors);
        static UpdateDataError NewError(
            UpdateDataErrorCode code,
            string message,
            IReadOnlyList<string> path
        ) => new(code, message, path);
        return AuthorizeAsync(
            unauthenticatedErrorCode: UpdateDataErrorCode.UNAUTHENTICATED,
            unauthorizedErrorCode: UpdateDataErrorCode.UNAUTHORIZED,
            errors => NewPayload(null, errors),
            NewError,
            userService,
            then: currentUser => FetchDataAsync(
                input,
                unknownDataErrorCode: UpdateDataErrorCode.UNKNOWN_DATA,
                NewPayload,
                NewError,
                context,
                then: async data =>
                {
                    var rememberedValues = new UpdateDataInput(
                        data.Locale,
                        input.DataId,
                        input.DataKind,
                        data.ComponentId,
                        data.Name,
                        data.Description,
                        data.Warnings,
                        data.CreatedAt,
                        data.CreatorId
                    );
                    return await ActAndThenCreateResponseApprovalAsync(
                        UpdateDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                        NewPayload,
                        NewError,
                        context,
                        responseApprovalService,
                        act: async () =>
                        {
                            data.Update(
                                input.Locale,
                                input.ComponentId,
                                input.Name,
                                input.Description,
                                input.Warnings,
                                input.CreatedAt,
                                input.CreatorId
                            );
                            await context.SaveChangesAsync(cancellationToken);
                            return data;
                        },
                        undo: async data =>
                        {
                            data.Update(
                                rememberedValues.Locale,
                                rememberedValues.ComponentId,
                                rememberedValues.Name,
                                rememberedValues.Description,
                                rememberedValues.Warnings,
                                rememberedValues.CreatedAt,
                                rememberedValues.CreatorId
                            );
                            await context.SaveChangesAsync(cancellationToken);
                        },
                        cancellationToken
                    );
                },
                cancellationToken
            ),
            cancellationToken
        );
    }

    public sealed record DeleteDataInput(
        Guid DataId,
        DataKind DataKind
    ) : IChangeDataInput;

    [SuppressMessage("Naming", "CA1707")]
    public enum DeleteDataErrorCode
    {
        UNAUTHENTICATED,
        UNAUTHORIZED,
        UNKNOWN_DATA
    }

    public sealed record DeleteDataError(
        DeleteDataErrorCode Code,
        string Message,
        IReadOnlyList<string> Path
    ) : UserErrorBase<DeleteDataErrorCode>(Code, Message, Path);

    public sealed record DeleteDataPayload(
        IReadOnlyCollection<DeleteDataError>? Errors
    );

    public Task<DeleteDataPayload> DeleteDataAsync(
        DeleteDataInput input,
        ApplicationDbContext context,
        UserService userService,
        CancellationToken cancellationToken
    )
    {
        static DeleteDataPayload NewPayload(
            IData? data,
            IReadOnlyCollection<DeleteDataError>? errors
        ) => new(errors);
        static DeleteDataError NewError(
            DeleteDataErrorCode code,
            string message,
            IReadOnlyList<string> path
        ) => new(code, message, path);
        return AuthorizeAsync(
            unauthenticatedErrorCode: DeleteDataErrorCode.UNAUTHENTICATED,
            unauthorizedErrorCode: DeleteDataErrorCode.UNAUTHORIZED,
            errors => NewPayload(null, errors),
            NewError,
            userService,
            then: currentUser => FetchDataAsync(
                input,
                unknownDataErrorCode: DeleteDataErrorCode.UNKNOWN_DATA,
                NewPayload,
                NewError,
                context,
                then: async data =>
                {
                    switch (data)
                    {
                        case CalorimetricData calorimetricData:
                            context.CalorimetricData.Remove(calorimetricData);
                            break;
                        case GeometricData geometricData:
                            context.GeometricData.Remove(geometricData);
                            break;
                        case HygrothermalData hygrothermalData:
                            context.HygrothermalData.Remove(hygrothermalData);
                            break;
                        case PhotovoltaicData photovoltaicData:
                            context.PhotovoltaicData.Remove(photovoltaicData);
                            break;
                        case OpticalData opticalData:
                            context.OpticalData.Remove(opticalData);
                            break;
                        default:
                            throw new NotSupportedException($"Data kind '{input.DataKind}' is not supported for deletion.");
                    }
                    await context.SaveChangesAsync(cancellationToken);
                    return NewPayload(data, null);
                },
                cancellationToken
            ),
            cancellationToken
        );
    }

    // public async Task<ExtractIntegralValuesFromFilesPayload> ExtractMirroredValuesFromFilesAsync(
    //     ExtractIntegralValuesFromFilesInput input,
    //     ApplicationDbContext context,
    //     UserService userService,
    //     ResponseApprovalService responseApprovalService,
    //     CancellationToken cancellationToken
    // )
    // {
    //     var currentUser = await userService.GetCurrentUser(cancellationToken);
    //     if (currentUser is null)
    //     {
    //         return new ExtractIntegralValuesFromFilesPayload(
    //             new ExtractIntegralValuesFromFilesError(
    //                 ExtractIntegralValuesFromFilesErrorCode.UNAUTHENTICATED,
    //                 $"The user is not authenticated.",
    //                 []
    //             )
    //         );
    //     }
    //     if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
    //     {
    //         return new ExtractIntegralValuesFromFilesPayload(
    //             new ExtractIntegralValuesFromFilesError(
    //                 ExtractIntegralValuesFromFilesErrorCode.UNAUTHORIZED,
    //                 $"The current user is not authorized to set mirrored values from files in this database.",
    //                 []
    //             )
    //         );
    //     }
    // }
}