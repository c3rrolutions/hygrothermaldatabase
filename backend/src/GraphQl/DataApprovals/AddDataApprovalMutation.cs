using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Authorization;
using Database.Data;
using Database.Enumerations;
using Database.Extensions;
using Database.GraphQl.Publications;
using Database.GraphQl.References;
using Database.GraphQl.Standards;
using Database.Json;
using Database.Services;
using GraphQL;
using GraphQL.Client.Abstractions.Utilities;
using HotChocolate.Types;
using HotChocolate.Authorization;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace Database.GraphQl.DataApprovals;

public sealed record AddDataApprovalInput
(
    Guid DataId,
    DataKind DataKind,
    OffsetDateTime Timestamp,
    string Signature,
    string KeyFingerprint,
    string Query,
    JsonElement Variables,
    string Message,
    Guid ApproverId,
    ReferenceInput Statement
) : IIdentifyDataInput;

[SuppressMessage("Naming", "CA1707")]
public enum AddDataApprovalErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    CREATING_RESPONSE_APPROVAL_FAILED,
    AMBIGUOUS_STATEMENT,
    MISSING_STATEMENT,
    INVALID_KEY_FINGERPRINT,
    FAILED_RECEIVING_KEY,
    BAD_SIGNATURE,
    FAILED_VERIFYING_SIGNATURE,
    FAILED_VERIFYING_RESPONSE,
    ILLEGAL_MESSAGE,
    WRONG_RESPONSE,
    WRONG_STATEMENT
}

public sealed record AddDataApprovalError(
    AddDataApprovalErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<AddDataApprovalErrorCode>(Code, Message, Path);

public sealed record AddDataApprovalPayload(
    DataApproval? DataApproval,
    IReadOnlyCollection<AddDataApprovalError>? Errors
) : Payload;

public static partial class Log
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Unknown signature verification result: {SigantureVerificationResult}")]
    public static partial void UnknownSignatureVerificationResult(this ILogger<AddDataApprovalMutation> logger, SigantureVerificationResult sigantureVerificationResult);
}

[ExtendObjectType(nameof(Mutation))]
public sealed class AddDataApprovalMutation
: DataMutationBase<DataApproval, AddDataApprovalPayload, AddDataApprovalError, AddDataApprovalErrorCode>
{
    private sealed record Message(
        JsonElement Statement,
        JsonElement Response
    );

    //[Authorize(Policy = Configuration.AuthConfiguration.WriteApiScope)]
    protected override AddDataApprovalPayload NewPayload(
        DataApproval? data,
        IReadOnlyCollection<AddDataApprovalError>? errors
    ) => new(data, errors);

    protected override AddDataApprovalError NewError(
        AddDataApprovalErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public async Task<AddDataApprovalPayload> AddDataApprovalAsync(
        AddDataApprovalInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        IsGnuPgFingerprintValid isGnuPgFingerprintValid,
        ResponseApprovalService responseApprovalService,
        SigningService signingService,
        ApiRequestService apiRequestService,
        ILogger<AddDataApprovalMutation> logger,
        AppSettings appSettings,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                AddDataApprovalErrorCode.UNAUTHENTICATED,
                AddDataApprovalErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var _, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }
        var errors = new List<AddDataApprovalError>();
        if (!await isGnuPgFingerprintValid.Do(
            input.KeyFingerprint,
            input.ApproverId,
            input.Timestamp,
            cancellationToken
        ))
        {
            errors.Add(
                NewError(
                    AddDataApprovalErrorCode.INVALID_KEY_FINGERPRINT,
                    $"The key fingerprint does not exist for the institution '{input.ApproverId}' or was not allowed before or was forbidden before the timestamp {input.Timestamp}.",
                    [nameof(input), nameof(input.KeyFingerprint).ToLowerFirst()]
                )
            );
        }
        var sigantureVerificationResult = await signingService.VerifySignatureAsync(input.Message, input.Signature, input.KeyFingerprint, cancellationToken);
        switch (sigantureVerificationResult)
        {
            case SigantureVerificationResult.FAILED_RECEIVING_KEY:
                errors.Add(
                    NewError(
                        AddDataApprovalErrorCode.FAILED_RECEIVING_KEY,
                        $"Failed to receive the key '{input.KeyFingerprint}' from the keyserver '{SigningService.KeyServerUrl.AbsoluteUri}'.",
                        [nameof(input), nameof(input.KeyFingerprint).ToLowerFirst()]
                    )
                );
                break;
            case SigantureVerificationResult.BAD_SIGNATURE:
                errors.Add(
                    NewError(
                        AddDataApprovalErrorCode.BAD_SIGNATURE,
                        $"The signature '{input.Signature}' for the message '{input.Message}' with the fingerprint '{input.KeyFingerprint}' is bad. Maybe the whitespace in the message differs from that in the original message file. For example, is there a newline at the end of the message and/or are the control characters for newlines identical? Note that in the WYSIWYG GraphQL interface, using triple double quotes for multiline strings changes the newline character at the very end of the string, which may not be how it was originally. Instead of using a multiline string, you may use a oneline string with escaped characters. To turn your message into such a JSON-escaped string, you may use the command-line tool `jq` as follows: `jq --raw-input --slurp '.' < ./message.json`",
                        [nameof(input), nameof(input.Signature).ToLowerFirst()]
                    )
                );
                break;
            case SigantureVerificationResult.GOOD_SIGNATURE:
                break;
            default:
                errors.Add(
                    NewError(
                        AddDataApprovalErrorCode.FAILED_VERIFYING_SIGNATURE,
                        $"Failed to verify the signature '{input.Signature}' for the message '{input.Message}' with the fingerprint '{input.KeyFingerprint}' for the reason '{sigantureVerificationResult}.",
                        [nameof(input), nameof(input.KeyFingerprint).ToLowerFirst()]
                    )
                );
                logger.UnknownSignatureVerificationResult(sigantureVerificationResult);
                break;
        }
        ;
        if (input.Statement.Standard is null
            && input.Statement.Publication is null)
        {
            errors.Add(
                NewError(
                    AddDataApprovalErrorCode.MISSING_STATEMENT,
                    "Both standard and publication are null.",
                    [nameof(input), nameof(input.Statement).ToLowerFirst()]
                )
            );
        }
        if (input.Statement.Standard is not null
            && input.Statement.Publication is not null)
        {
            errors.Add(
                NewError(
                    AddDataApprovalErrorCode.AMBIGUOUS_STATEMENT,
                    "Both standard and publication are non-null.",
                    [nameof(input), nameof(input.Statement).ToLowerFirst()]
                )
            );
        }
        Message? message = null;
        try
        {
            using var document = JsonDocument.Parse(input.Message);
            var root = document.RootElement;
            message = new Message(
                root.GetProperty(nameof(message.Statement).ToLowerFirst()).Clone(),
                root.GetProperty(nameof(message.Response).ToLowerFirst()).Clone()
            );
        }
        catch (Exception exception) when (exception is JsonException || exception is KeyNotFoundException)
        {
            errors.Add(
                NewError(
                    AddDataApprovalErrorCode.ILLEGAL_MESSAGE,
                    $"The message is not valid JSON or does not have the required structure '{{\"statement\": ..., \"response\": ...}}: {exception}",
                    [nameof(input), nameof(input.Message).ToLowerFirst()]
                )
            );
        }
        if (message is not null
            && input.Statement.Standard is null
            && input.Statement.Publication is not null
        )
        {
            var messagePublication = message.Statement.Deserialize<PublicationInput>(JsonSerializerSettings.GraphQl);
            if (!JsonElement.DeepEquals(
                JsonSerializer.SerializeToElement(input.Statement.Publication),
                JsonSerializer.SerializeToElement(messagePublication)
            ))
            {
                errors.Add(
                    NewError(
                        AddDataApprovalErrorCode.WRONG_STATEMENT,
                        $"The message statement '{JsonSerializer.Serialize(messagePublication)}' is not equal to the input publication statement '{JsonSerializer.Serialize(input.Statement.Publication)}'.",
                        [nameof(input), nameof(input.Statement).ToLowerFirst(), nameof(input.Statement.Publication).ToLowerFirst()]
                    )
                );
            }
        }
        if (message is not null
            && input.Statement.Standard is not null
            && input.Statement.Publication is null
        )
        {
            var messageStandard = message.Statement.Deserialize<StandardInput>(JsonSerializerSettings.GraphQl);
            if (!JsonElement.DeepEquals(
                JsonSerializer.SerializeToElement(input.Statement.Standard),
                JsonSerializer.SerializeToElement(messageStandard)
            ))
            {
                errors.Add(
                    NewError(
                        AddDataApprovalErrorCode.WRONG_STATEMENT,
                        $"The message statement '{JsonSerializer.Serialize(messageStandard)}' is not equal to the input standard statement '{JsonSerializer.Serialize(input.Statement.Standard)}'.",
                        [nameof(input), nameof(input.Statement).ToLowerFirst(), nameof(input.Statement.Standard).ToLowerFirst()]
                    )
                );
            }
        }
        JsonElement? response = null;
        try
        {
            // TODO Query the database GraphQL endpoint directly in C# instead of over HTTP.
            response = await apiRequestService.QueryGraphQlAsJson(
                appSettings.DatabaseGraphQlEndpoint,
                new GraphQLRequest(
                    input.Query,
                    input.Variables
                ),
                cancellationToken
            );
        }
        catch (Exception exception) when (exception is HttpRequestException || exception is JsonException)
        {
            errors.Add(
                NewError(
                    AddDataApprovalErrorCode.FAILED_VERIFYING_RESPONSE,
                    $"Failed verifying the response due to the exception: {exception}",
                    [nameof(input), nameof(input.Query).ToLowerFirst()]
                )
            );
        }
        if (message is not null
            && response is not null
            && !JsonElement.DeepEquals((JsonElement)response, message.Response)
        )
        {
            errors.Add(
                NewError(
                    AddDataApprovalErrorCode.WRONG_RESPONSE,
                    $"The message response '{JsonSerializer.Serialize(message.Response)}' is not equal to the response '{JsonSerializer.Serialize(response)}' of the query '{input.Query}' against the GraphQL endpoint '{appSettings.DatabaseGraphQlEndpoint}'.",
                    [nameof(input), nameof(input.Message).ToLowerFirst()]
                )
            );
        }
        if (errors.Count > 0)
        {
            return NewPayload(null, errors);
        }

        if ((await FetchDataAsync(
                input,
                AddDataApprovalErrorCode.UNKNOWN_DATA,
                context,
                cancellationToken
            )
            ).Failed(out var data, out var fetchDataErrorPayload)
        )
        {
            return fetchDataErrorPayload;
        }

        var approval = new DataApproval(
            input.Timestamp,
            input.Signature.Trim(),
            input.KeyFingerprint,
            input.Query.Trim(),
            input.Variables,
            input.Message,
            input.ApproverId,
            input.Statement.ToDomainModel()
        );

        data.Approvals.Add(approval);
        await context.SaveChangesAsync(cancellationToken);

        if ((await CreateResponseApprovalAsync(
                data,
                AddDataApprovalErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                responseApprovalService,
                context,
                cancellationToken
            )
            ).Failed(out var createResponseApprovalErrorPayload)
        )
        {
            data.Approvals.Remove(approval);
            await context.SaveChangesAsync(cancellationToken);
            return createResponseApprovalErrorPayload;
        }

        return NewPayload(approval, null);
    }
}