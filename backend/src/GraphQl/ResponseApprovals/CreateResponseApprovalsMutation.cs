using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut.Data;
using Database.Authorization;
using Database.Data;
using Database.Services;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.Extensions.Logging;
using Database.Extensions;
using Database.GraphQl.Extensions;
using HotChocolate.Resolvers;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.ResponseApprovals;

[SuppressMessage("Naming", "CA1707")]
public enum CreateResponseApprovalsErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    UNKNOWN_DATABASE,
    CREATING_RESPONSE_APPROVAL_FAILED
}

public sealed record CreateResponseApprovalsError(
    CreateResponseApprovalsErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreateResponseApprovalsErrorCode>(Code, Message, Path);

public static partial class Log
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Creating response approval for data of type {Type} with ID {Id}."
    )]
    public static partial void FailedCreatingResponseApproval(this ILogger<CreateResponseApprovalsMutation> logger, Type type, Guid id, Exception exception);
}

public sealed record CreateResponseApprovalsPayload(
    IReadOnlyCollection<IData>? Data,
    IReadOnlyCollection<CreateResponseApprovalsError>? Errors
) : Payload;

public sealed class CreateResponseApprovalsFilterType
: ResponseApprovalFilterType
{
    protected override void Configure(
        IFilterInputTypeDescriptor<IData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(CreateResponseApprovalsFilterType)[..^10] + GraphQlConstants.FilterInputSuffix);
    }
}

[ExtendObjectType(nameof(Mutation))]
public sealed class CreateResponseApprovalsMutation
: DataMutationBase<IReadOnlyCollection<IData>, CreateResponseApprovalsPayload, CreateResponseApprovalsError, CreateResponseApprovalsErrorCode>
{
    //[Authorize(Policy = Configuration.AuthConfiguration.WriteApiScope)]
    protected override CreateResponseApprovalsPayload NewPayload(
        IReadOnlyCollection<IData>? data,
        IReadOnlyCollection<CreateResponseApprovalsError>? errors
    ) => new(data, errors);

    protected override CreateResponseApprovalsError NewError(
        CreateResponseApprovalsErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    [UseFiltering<CreateResponseApprovalsFilterType>]
    public async Task<CreateResponseApprovalsPayload> CreateResponseApprovalsAsync(
        IResolverContext resolverContext,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        ResponseApprovalService responseApprovalService,
        ILogger<CreateResponseApprovalsMutation> logger,
        CancellationToken cancellationToken
    )
    {
        var queryContext = resolverContext.GetQueryContext<IData>();
        if ((await AuthorizeAsync(
                CreateResponseApprovalsErrorCode.UNAUTHENTICATED,
                CreateResponseApprovalsErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var _, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }
        var dataSets = new ConcurrentBag<IData>();
        var errors = new ConcurrentBag<CreateResponseApprovalsError>();
        await Parallel.ForEachAsync(
            context.GetAllDataAsync(_ => _.Approval == null, queryContext),
            cancellationToken,
            async (data, cancellationToken) =>
            {
                try
                {
                    data.Approval = await responseApprovalService.CreateResponseApproval(data, cancellationToken);
                    dataSets.Add(data);
                }
                catch (Exception exception)
                {
                    logger.FailedCreatingResponseApproval(data.GetType(), data.Id, exception);
                    errors.Add(
                        NewError(
                            CreateResponseApprovalsErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                            $"Failed creating a response approval for the data set {data.Id} named '{data.Name}' with the error message: {exception.Message}",
                            []
                        )
                    );
                }
            }
        );
        await context.SaveChangesAsync(cancellationToken);
        if (!errors.IsEmpty)
        {
            return NewPayload(dataSets, errors);
        }
        return NewPayload(dataSets, null);
    }
}