using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.GraphQl.Publications;
using Database.GraphQl.References;
using Database.GraphQl.Standards;
using Database.Json;
using Database.Services;
using GraphQL;
using GraphQL.Client.Abstractions.Utilities;
using GreenDonut.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.Extensions.Logging;

namespace Database.GraphQl.DataApprovals;

public static partial class DataApprovalMutationsLogging
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Unknown signature verification result: {SigantureVerificationResult}")]
    public static partial void UnknownSignatureVerificationResult(this ILogger<DataApprovalMutations> logger, SigantureVerificationResult sigantureVerificationResult);
}

[ExtendObjectType(nameof(Mutation))]
public sealed class DataApprovalMutations
{
    private sealed record Message(
        JsonElement Statement,
        JsonElement Response
    );

    //[Authorize(Policy = Configuration.AuthConfiguration.WriteApiScope)]
    public async Task<AddDataApprovalPayload> AddDataApprovalAsync(
        DataApprovalInput input,
        ApplicationDbContext context,
        UserService userService,
        DataApprovalService dataApprovalService,
        ResponseApprovalService responseApprovalService,
        SigningService signingService,
        ApiRequestService apiRequestService,
        ILogger<DataApprovalMutations> logger,
        AppSettings appSettings,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);
        if (currentUser is null)
        {
            return new AddDataApprovalPayload(
                new AddDataApprovalError(
                    AddDataApprovalErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }
        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new AddDataApprovalPayload(
                new AddDataApprovalError(
                    AddDataApprovalErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to add data approvals to this database.",
                    []
                )
            );
        }
        var errors = new List<AddDataApprovalError>();
        if (!await dataApprovalService.IsGnuPgFingerprintValid(
            input.KeyFingerprint,
            input.ApproverId,
            input.Timestamp,
            cancellationToken
        ))
        {
            errors.Add(
                new AddDataApprovalError(
                    AddDataApprovalErrorCode.INVALID_KEY_FINGERPRINT,
                    $"The key fingerprint does not exist for the institution '{input.ApproverId}' or was not allowed before or was forbidden before the timestamp {input.Timestamp}.",
                    [nameof(input), nameof(input.KeyFingerprint).ToLowerFirst()]
                )
            );
        }
        var sigantureVerificationResult = await signingService.VerifySignature(input.Message, input.Signature, input.KeyFingerprint);
        switch (sigantureVerificationResult)
        {
            case SigantureVerificationResult.FAILED_RECEIVING_KEY:
                errors.Add(
                    new AddDataApprovalError(
                        AddDataApprovalErrorCode.FAILED_RECEIVING_KEY,
                        $"Failed to receive the key '{input.KeyFingerprint}' from the keyserver '{SigningService.KeyServerUrl.AbsoluteUri}'.",
                        [nameof(input), nameof(input.KeyFingerprint).ToLowerFirst()]
                    )
                );
                break;
            case SigantureVerificationResult.BAD_SIGNATURE:
                errors.Add(
                    new AddDataApprovalError(
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
                    new AddDataApprovalError(
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
                new AddDataApprovalError(
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
                new AddDataApprovalError(
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
                new AddDataApprovalError(
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
                    new AddDataApprovalError(
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
                    new AddDataApprovalError(
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
                new AddDataApprovalError(
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
                new AddDataApprovalError(
                    AddDataApprovalErrorCode.WRONG_RESPONSE,
                    $"The message response '{JsonSerializer.Serialize(message.Response)}' is not equal to the response '{JsonSerializer.Serialize(response)}' of the query '{input.Query}' against the GraphQL endpoint '{appSettings.DatabaseGraphQlEndpoint}'.",
                    [nameof(input), nameof(input.Message).ToLowerFirst()]
                )
            );
        }
        if (errors.Count >= 1)
        {
            return new AddDataApprovalPayload(errors);
        }

        var data = await context.GetDataAsync(input.DataId, cancellationToken);
        if (data is null)
        {
            return new AddDataApprovalPayload(
                new AddDataApprovalError(
                    AddDataApprovalErrorCode.UNKNOWN_DATA,
                    $"Unknown data.",
                    [nameof(input), nameof(input.DataId).ToLowerFirst()]
                )
            );
        }

        var approval = new DataApproval(
            input.Timestamp,
            input.Signature.Trim(),
            input.KeyFingerprint,
            input.Query.Trim(),
            input.Variables,
            input.Message,
            input.ApproverId,
            ReferenceType.FromInput(input.Statement)
        );

        data.Approvals.Add(approval);
        await context.SaveChangesAsync(cancellationToken);

        try
        {
            data.Approval = await responseApprovalService.CreateResponseApproval(data, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            data.Approvals.Remove(approval);
            await context.SaveChangesAsync(cancellationToken);

            return new AddDataApprovalPayload(
                approval,
                new AddDataApprovalError(
                    AddDataApprovalErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                    $"Signing failed with message: {exception.Message}",
                    []
                )
            );
        }
        return new AddDataApprovalPayload(approval);
    }

    public async Task<RemoveDataApprovalPayload> RemoveDataApprovalAsync(
        DataApprovalInput input,
        ApplicationDbContext context,
        UserService userService,
        ResponseApprovalService responseApprovalService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);
        if (currentUser is null)
        {
            return new RemoveDataApprovalPayload(
                new RemoveDataApprovalError(
                    RemoveDataApprovalErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }
        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new RemoveDataApprovalPayload(
                new RemoveDataApprovalError(
                    RemoveDataApprovalErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to add data approvals to this database.",
                    []
                )
            );
        }

        var data = await context.GetDataAsync(input.DataId, cancellationToken);
        if (data is null)
        {
            return new RemoveDataApprovalPayload(
                new RemoveDataApprovalError(
                    RemoveDataApprovalErrorCode.UNKNOWN_DATA,
                    $"Unknown data.",
                    [nameof(input), nameof(input.DataId).ToLowerFirst()]
                )
            );
        }

        var approval = data.Approvals
            .Where(a => a.Signature.Trim() == input.Signature.Trim())
            .SingleOrDefault();
        if (approval is null)
        {
            return new RemoveDataApprovalPayload(
                new RemoveDataApprovalError(
                    RemoveDataApprovalErrorCode.UNKNOWN_APPROVAL,
                    $"Unknown data approval.",
                    [nameof(input), nameof(input.Signature).ToLowerFirst()]
                )
            );
        }

        data.Approvals.Remove(approval);
        await context.SaveChangesAsync(cancellationToken);

        try
        {
            data.Approval = await responseApprovalService.CreateResponseApproval(data, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            data.Approvals.Add(approval);
            await context.SaveChangesAsync(cancellationToken);

            return new RemoveDataApprovalPayload(
                approval,
                new RemoveDataApprovalError(
                    RemoveDataApprovalErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                    $"Signing failed with message: {exception.Message}",
                    []
                )
            );
        }
        return new RemoveDataApprovalPayload(approval);
    }
}