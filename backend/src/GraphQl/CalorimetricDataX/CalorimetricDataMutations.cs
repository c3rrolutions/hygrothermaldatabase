using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Services;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;

namespace Database.GraphQl.CalorimetricDataX;

[ExtendObjectType(nameof(Mutation))]
public sealed class CalorimetricDataMutations
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    public async Task<CreateCalorimetricDataPayload> CreateCalorimetricDataAsync(
        CreateCalorimetricDataInput input,
        ApplicationDbContext context,
        IUserService userService,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(
            httpContextAccessor,
            cancellationToken).ConfigureAwait(false);
        if (currentUser == null)
        {
            return new CreateCalorimetricDataPayload(
                new CreateCalorimetricDataError(
                    CreateCalorimetricDataErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }

        if (!CalorimetricDataAuthorization.IsAuthorizedToCreateCalorimetricDataForInstitution(
            currentUser,
            input.CreatorId,
            cancellationToken
            )
        )
            return new CreateCalorimetricDataPayload(
                new CreateCalorimetricDataError(
                    CreateCalorimetricDataErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to create calorimetric data for the institution.",
                    []
                )
            );
        var calorimetricData = new CalorimetricData(
            input.Locale,
            input.ComponentId,
            input.Name,
            input.Description,
            input.Warnings,
            input.CreatorId,
            input.CreatedAt,
            new AppliedMethod(
                input.AppliedMethod.MethodId,
                input.AppliedMethod.Arguments
                    .Select(a => new NamedMethodArgument(
                        a.Name,
                        // TODO Turn `a.Value` into `JsonDocument`. It comes
                        // as nested `IReadOnlyDictionary/-List` as said on
                        // https://chillicream.com/docs/hotchocolate/v11/defining-a-schema/scalars/#any-type
                        // Take inspiration from
                        // https://josef.codes/custom-dictionary-string-object-jsonconverter-for-system-text-json/
                        // and
                        // https://github.com/joseftw/JOS.SystemTextJsonDictionaryStringObjectJsonConverter/blob/develop/src/JOS.SystemTextJsonDictionaryObjectModelBinder/DictionaryStringObjectJsonConverter.cs
                        // This is also needed in `GetHttpsResourceMutations`.
                        JsonDocument.Parse(@"""TODO""")
                    ))
                    .ToList(),
                input.AppliedMethod.Sources
                    .Select(s => new NamedMethodSource(
                        s.Name,
                        new CrossDatabaseDataReference(
                            s.Value.DataId,
                            s.Value.DataTimestamp,
                            s.Value.DataKind,
                            s.Value.DatabaseId
                        )
                    ))
                    .ToList()
            ),
            input.GValues,
            input.UValues
        );
        var resource = new GetHttpsResource(
            input.RootResource.Description,
            input.RootResource.HashValue,
            input.RootResource.DataFormatId,
            null,
            input.RootResource.ArchivedFilesMetaInformation.Select(i =>
                new FileMetaInformation(
                    i.Path,
                    i.DataFormatId
                )
            ).ToList(),
            input.RootResource.AppliedConversionMethod is null
                ? null
                : new ToTreeVertexAppliedConversionMethod(
                    input.RootResource.AppliedConversionMethod.MethodId,
                    input.RootResource.AppliedConversionMethod.Arguments.Select(a =>
                        new NamedMethodArgument(
                            a.Name,
                            // TODO Turn `a.Value` into `JsonDocument`. It comes
                            // as nested `IReadOnlyDictionary/-List` as said on
                            // https://chillicream.com/docs/hotchocolate/v11/defining-a-schema/scalars/#any-type
                            // Take inspiration from
                            // https://josef.codes/custom-dictionary-string-object-jsonconverter-for-system-text-json/
                            // and
                            // https://github.com/joseftw/JOS.SystemTextJsonDictionaryStringObjectJsonConverter/blob/develop/src/JOS.SystemTextJsonDictionaryObjectModelBinder/DictionaryStringObjectJsonConverter.cs
                            // This is also needed in `GetHttpsResourceMutations`.
                            JsonDocument.Parse(@"""TODO""")
                        )
                    ).ToList(),
                    input.RootResource.AppliedConversionMethod.SourceName
                )
        );
        calorimetricData.Resources.Add(resource);
        context.CalorimetricData.Add(calorimetricData);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new CreateCalorimetricDataPayload(calorimetricData);
    }
}