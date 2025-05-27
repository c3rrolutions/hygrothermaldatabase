using System;
using HotChocolate.Data;
using HotChocolate.Data.Filters;
using HotChocolate.Data.Sorting;
using HotChocolate.Language;
using HotChocolate.Types;
using System.Net.Http;
using System.Text.RegularExpressions;
using Database.Data;
using Database.GraphQl;
using Database.GraphQl.CalorimetricDataX;
using Database.GraphQl.Common;
using Database.GraphQl.Databases;
using Database.GraphQl.DataX;
using Database.GraphQl.Filters;
using Database.GraphQl.GetHttpsResources;
using Database.GraphQl.HygrothermalDataX;
using Database.GraphQl.OpticalDataX;
using Database.GraphQl.PhotovoltaicDataX;
using Database.GraphQl.Publications;
using Database.GraphQl.References;
using Database.GraphQl.Standards;
using Database.GraphQl.GeometricDataX;
using Database.GraphQl.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IServiceCollection = Microsoft.Extensions.DependencyInjection.IServiceCollection;
using Database.GraphQl.Approvals;
using Database.GraphQl.AccessRights;
using Database.GraphQl.MethodAsService;

namespace Database.Configuration;

public static class GraphQlConfiguration
{
    public static void ConfigureServices(
        IServiceCollection services,
        IWebHostEnvironment environment,
        AppSettings appSettings
    )
    {
        // Stitching Services https://chillicream.com/docs/hotchocolate/v13/distributed-schema/schema-stitching
        var httpMetabaseClientBuilder = services.AddHttpClient(
            WellKnownSchemaNames.Metabase,
            _ => _.BaseAddress = new Uri($"{appSettings.MetabaseHost}/graphql")
        );
        var httpDatabaseClientBuilder = services.AddHttpClient(
            WellKnownSchemaNames.Database,
            _ => _.BaseAddress = new Uri($"{appSettings.Host}/graphql")
        );
        if (!environment.IsProduction())
            httpMetabaseClientBuilder.ConfigurePrimaryHttpMessageHandler(_ =>
                new HttpClientHandler
                {
                    // ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                }
            );
        httpDatabaseClientBuilder.ConfigurePrimaryHttpMessageHandler(_ =>
            new HttpClientHandler
            {
                // ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }
        );
        // Automatic-Persisted-Queries Services
        services
            .AddMemoryCache()
            .AddSha256DocumentHashProvider(HashFormat.Hex);
        // GraphQL Server
        services
            .AddGraphQLServer()
            .BindRuntimeType<uint, NonNegativeIntType>()
            // Services https://chillicream.com/docs/hotchocolate/v13/integrations/entity-framework#registerdbcontext
            .RegisterDbContextFactory<ApplicationDbContext>()
            .AddMutationConventions(new MutationConventionOptions { ApplyToAllMutations = false })
            // Extensions
            .AddProjections()
            .AddFiltering<CustomFilterConvention>()
            .AddSorting<CustomSortConvention>()
            .AddAuthorization()
            .AddGlobalObjectIdentification()
            .AddQueryFieldToMutationPayloads()
            .ModifyOptions(options =>
                {
                    // https://github.com/ChilliCream/hotchocolate/blob/main/src/HotChocolate/Core/src/Types/Configuration/Contracts/ISchemaOptions.cs
                    options.StrictValidation = true;
                    options.UseXmlDocumentation = false;
                    options.SortFieldsByName = true;
                    options.RemoveUnreachableTypes = false;
                    options.DefaultBindingBehavior = BindingBehavior.Implicit;
                    /* options.FieldMiddleware = ... */
                }
            )
            .ModifyRequestOptions(options =>
                {
                    // https://github.com/ChilliCream/hotchocolate/blob/main/src/HotChocolate/Core/src/Execution/Options/RequestExecutorOptions.cs
                    /* options.ExecutionTimeout = ...; */
                    options.IncludeExceptionDetails = !environment.IsProduction(); // Default is `Debugger.IsAttached`.
                    /* options.QueryCacheSize = ...; */
                    /* options.UseComplexityMultipliers = ...; */
                }
            )
            // Configure `https://github.com/ChilliCream/hotchocolate/blob/main/src/HotChocolate/Core/src/Validation/Options/ValidationOptions.cs`. But how?
            // Subscriptions
            /* .AddInMemorySubscriptions() */
            // Persisted queries
            /* .AddFileSystemOperationDocumentStorage("./persisted_operations") */
            /* .UsePersistedOperationPipeline(); */
            // HotChocolate uses the default authentication scheme,
            // which we set to `null` in `AuthConfiguration` to force
            // users to be explicit about what scheme to use when
            // making it easier to grasp the various authentication
            // flows.
            .AddHttpRequestInterceptor(async (httpContext, requestExecutor, requestBuilder, cancellationToken) =>
            {
                try
                {
                    await HttpContextAuthentication.Authenticate(httpContext);
                }
                catch (Exception e)
                {
                    // TODO Log to a `ILogger<GraphQlConfiguration>` instead.
                    Console.WriteLine(e);
                }
            })
            .AddDiagnosticEventListener(_ =>
                new LoggingDiagnosticEventListener(
                    _.GetApplicationService<ILogger<LoggingDiagnosticEventListener>>()
                )
            )
            // Scalar Types
            .AddType(new UuidType("Uuid",
                defaultFormat: 'D')) // https://chillicream.com/docs/hotchocolate/defining-a-schema/scalars#uuid-type
            .AddType(new UrlType("Url"))
            .AddType(new JsonType("Any",
                BindingBehavior
                    .Implicit)) // https://chillicream.com/blog/2023/02/08/new-in-hot-chocolate-13#json-scalar
            .AddType(new LocaleType())
            // Query Types
            .AddQueryType(d => d.Name(nameof(Query)))
            .AddType<CalorimetricDataQueries>()
            .AddType<DataConnection>()
            .AddType<DatabaseQueries>()
            .AddType<GetHttpsResourceQueries>()
            .AddType<HygrothermalDataQueries>()
            .AddType<OpticalDataQueries>()
            .AddType<PhotovoltaicDataQueries>()
            .AddType<UserQueries>()
            .AddType<VerificationCodeQueries>()
            .AddType<GeometricDataQueries>()
            .AddType<MethodAsServiceQuaries>()
            .AddType<DataQueries>()
            // Mutation Types
            .AddMutationType(d => d.Name(nameof(Mutation)))
            .AddType<CalorimetricDataMutations>()
            .AddType<DatabaseMutations>()
            .AddType<GetHttpsResourceMutations>()
            .AddType<HygrothermalDataMutations>()
            .AddType<OpticalDataMutations>()
            .AddType<PhotovoltaicDataMutations>()
            .AddType<GeometricDataMutations>()
            .AddType<ApprovalMutations>()
            .AddType<DataAccessRightsMutations>()
            /* .AddSubscriptionType(d => d.Name(nameof(GraphQl.Subscription))) */
            /*     .AddType<ComponentSubscriptions>() */
            // Object Types
            .AddType<OpenEndedDateTimeRangeType>()
            .AddType<CalorimetricDataType>()
            .AddType<DataApprovalType>()
            .AddType<DataType>()
            .AddType<GetHttpsResourceType>()
            .AddType<HygrothermalDataType>()
            .AddType<NamedMethodArgumentType>()
            .AddType<OpticalDataType>()
            .AddType<PhotovoltaicDataType>()
            .AddType<PublicationType>()
            .AddType<ReferenceType>()
            .AddType<StandardType>()
            .AddType<GeometricDataType>()
            .AddType<UserType>()
            .AddType<UploadType>()
            // Data Loaders
            /* .AddDataLoader<GraphQl.Components.ComponentByIdDataLoader>() */
            // Paging
            .AddDbContextCursorPagingProvider()
            .ModifyPagingOptions(_ =>
                {
                    _.MaxPageSize = 100;
                    _.DefaultPageSize = 100;
                    _.IncludeTotalCount = true;
                    _.IncludeNodesField = false;
                    // TODO I actually want to infer connection names from fields (which is the default in HotChocolate). However, the current `database.graphql` schema that I hand-wrote still infers connection names from types.
                    _.InferConnectionNameFromField = false;
                }
            )
            // Stitching
            // .AddRemoteSchema(WellKnownSchemaNames.Metabase, ignoreRootTypes: true)
            // .AddTypeExtensionsFromString(@"
            //     extend type OpticalData {
            //         component: Component @delegate(schema: 'Metabase', path: 'component(uuid: $fields:ComponentId)')
            //     }
            // ")
            // Automatic Peristed Queries
            .UseAutomaticPersistedOperationPipeline()
            .AddInMemoryOperationDocumentStorage(); // Needed by the automatic persisted operation pipeline
    }

    public static class WellKnownSchemaNames
    {
        public const string Metabase = "metabase";
        public const string Database = "database";
    }
}

// See https://chillicream.com/docs/hotchocolate/fetching-data/filtering/#filter-conventions
public partial class CustomFilterConvention : FilterConvention
{
    protected override void Configure(IFilterConventionDescriptor descriptor)
    {
        descriptor.AddDefaults();
        // Use argument name `where`
        descriptor.ArgumentName("where");
        // Allow conjunction and disjunction
        descriptor.AllowAnd();
        descriptor.AllowOr();
        // Bind custom types
        descriptor.BindRuntimeType<GetHttpsResource, GetHttpsResourceFilterType>();
        descriptor.BindRuntimeType<NamedMethodArgument, NamedMethodArgumentFilterType>();
        descriptor.BindRuntimeType<CalorimetricData, CalorimetricDataFilterType>();
        descriptor.BindRuntimeType<IData, DataFilterType>();
        descriptor.BindRuntimeType<HygrothermalData, HygrothermalDataFilterType>();
        descriptor.BindRuntimeType<OpticalData, OpticalDataFilterType>();
        descriptor.BindRuntimeType<GeometricData, GeometricDataFilterType>();
        descriptor.BindRuntimeType<PhotovoltaicData, PhotovoltaicDataFilterType>();
    }

    // TODO Overriding and changing type names in this way is _super_ error-prone. However, using `descriptor.Configure<...FilterInputType<T>>(x => x.Name(name))` does not work. Why?
    // For the base implementation see https://github.com/ChilliCream/hotchocolate/blob/f0dff93a14cb7ddecc7b3a0530a687a5bc4bad71/src/HotChocolate/Data/src/Data/Filters/Convention/FilterConvention.cs#L129
    public override string GetTypeName(Type runtimeType)
    {
        var nameString = base.GetTypeName(runtimeType);
        return
            IDataRegex().Replace(
                DoubleRegex().Replace(
                    FloatRegex().Replace(
                        ListStringRegex().Replace(
                            ListFloatRegex().Replace(
                                GuidRegex().Replace(
                                    ComparableRegex().Replace(
                                        FilterInputRegex().Replace(
                                            OperationFilterInputRegex().Replace(
                                                nameString,
                                                "PropositionInput"
                                            ),
                                            "PropositionInput"
                                        ),
                                        ""
                                    ),
                                    "Uuid"
                                ),
                                "Floats"
                            ),
                            "Strings"
                        ),
                        "Float"
                    ),
                    "Float"
                ),
                "Data"
            );
    }

    [GeneratedRegex(@"IData")]
    private static partial Regex IDataRegex();

    [GeneratedRegex(@"Double")]
    private static partial Regex DoubleRegex();

    [GeneratedRegex(@"Float")]
    private static partial Regex FloatRegex();

    [GeneratedRegex(@"ListString")]
    private static partial Regex ListStringRegex();

    [GeneratedRegex(@"ListFloat")]
    private static partial Regex ListFloatRegex();

    [GeneratedRegex(@"Guid")]
    private static partial Regex GuidRegex();

    [GeneratedRegex(@"^Comparable")]
    private static partial Regex ComparableRegex();

    [GeneratedRegex(@"FilterInput$")]
    private static partial Regex FilterInputRegex();

    [GeneratedRegex(@"OperationFilterInput")]
    private static partial Regex OperationFilterInputRegex();
}

public static class FilterConventionDescriptorExtensions
{
    // Inspired by FilterConventionDescriptorExtensions#AddDefaults
    // https://github.com/ChilliCream/hotchocolate/blob/ee5813646fdfea81035c681989793514f33b5d94/src/HotChocolate/Data/src/Data/Filters/Convention/Extensions/FilterConventionDescriptorExtensions.cs#L16
    public static IFilterConventionDescriptor AddDefaults(
        this IFilterConventionDescriptor descriptor)
    {
        return descriptor.AddDefaultOperations().BindDefaultTypes().UseQueryableProvider();
    }

    // Inspired by FilterConventionDescriptorExtensions#AddDefaultOperations
    // https://github.com/ChilliCream/hotchocolate/blob/ee5813646fdfea81035c681989793514f33b5d94/src/HotChocolate/Data/src/Data/Filters/Convention/Extensions/FilterConventionDescriptorExtensions.cs#L28
    public static IFilterConventionDescriptor AddDefaultOperations(
        this IFilterConventionDescriptor descriptor)
    {
        descriptor.Operation(DefaultFilterOperations.Equals).Name("equalTo");
        descriptor.Operation(DefaultFilterOperations.NotEquals).Name("notEqualTo");
        descriptor.Operation(DefaultFilterOperations.GreaterThan).Name("greaterThan");
        descriptor.Operation(DefaultFilterOperations.NotGreaterThan).Name("notGreaterThan");
        descriptor.Operation(DefaultFilterOperations.GreaterThanOrEquals).Name("greaterThanOrEqualTo");
        descriptor.Operation(DefaultFilterOperations.NotGreaterThanOrEquals).Name("notGreaterThanOrEqualTo");
        descriptor.Operation(DefaultFilterOperations.LowerThan).Name("lessThan");
        descriptor.Operation(DefaultFilterOperations.NotLowerThan).Name("notLessThan");
        descriptor.Operation(DefaultFilterOperations.LowerThanOrEquals).Name("lessThanOrEqualTo");
        descriptor.Operation(DefaultFilterOperations.NotLowerThanOrEquals).Name("notLessThanOrEqualTo");
        descriptor.Operation(DefaultFilterOperations.Contains).Name("contains");
        descriptor.Operation(DefaultFilterOperations.NotContains).Name("doesNotContain");
        descriptor.Operation(DefaultFilterOperations.In).Name("in");
        descriptor.Operation(DefaultFilterOperations.NotIn).Name("notIn");
        descriptor.Operation(DefaultFilterOperations.StartsWith).Name("startsWith");
        descriptor.Operation(DefaultFilterOperations.NotStartsWith).Name("doesNotStartWith");
        descriptor.Operation(DefaultFilterOperations.EndsWith).Name("endsWith");
        descriptor.Operation(DefaultFilterOperations.NotEndsWith).Name("doesNotEndWith");
        descriptor.Operation(DefaultFilterOperations.All).Name("all");
        descriptor.Operation(DefaultFilterOperations.None).Name("none");
        descriptor.Operation(DefaultFilterOperations.Some).Name("some");
        descriptor.Operation(DefaultFilterOperations.Any).Name("any");
        descriptor.Operation(DefaultFilterOperations.And).Name("and");
        descriptor.Operation(DefaultFilterOperations.Or).Name("or");
        descriptor.Operation(DefaultFilterOperations.Data).Name("data");
        descriptor.Operation(AdditionalFilterOperations.Not).Name("not");
        // TODO `inClosedInterval`
        return descriptor;
    }

    // Inspired by FilterConventionDescriptorExtensions#BindDefaultTypes
    // https://github.com/ChilliCream/hotchocolate/blob/ee5813646fdfea81035c681989793514f33b5d94/src/HotChocolate/Data/src/Data/Filters/Convention/Extensions/FilterConventionDescriptorExtensions.cs#L73
    public static IFilterConventionDescriptor BindDefaultTypes(
        this IFilterConventionDescriptor descriptor)
    {
        descriptor
            .BindRuntimeType<string, StringOperationFilterInputType>()
            .BindRuntimeType<bool, BooleanOperationFilterInputType>()
            .BindRuntimeType<bool?, BooleanOperationFilterInputType>()
            .BindComparableType<byte>("BytePropositionInput")
            .BindComparableType<short>("ShortPropositionInput")
            .BindComparableType<int>("IntPropositionInput")
            .BindComparableType<long>("LongPropositionInput")
            .BindComparableType<float>("FloatXPropositionInput")
            .BindComparableType<double>("FloatPropositionInput")
            .BindComparableType<decimal>("DecimalPropositionInput")
            .BindComparableType<sbyte>("SignedBytePropositionInput")
            .BindComparableType<ushort>("UnsignedShortPropositionInput")
            .BindComparableType<uint>("UnsignedIntPropositionInput")
            .BindComparableType<ulong>("UnsigendLongPropositionInput")
            .BindComparableType<Guid>("UuidPropositionInput")
            .BindComparableType<DateTime>("DateTimePropositionInput")
            .BindComparableType<DateTimeOffset>("DateTimeOffsetPropositionInput")
            .BindComparableType<TimeSpan>("TimeSpanPropositionInput");
        // TODO Why does this not work?
        // descriptor
        //     .Configure<StringOperationFilterInputType>(x => x.Name("StringPropositionInput"))
        //     .Configure<BooleanOperationFilterInputType>(x => x.Name("BooleanPropositionInput"));
        return descriptor;
    }

    // Inspired by FilterConventionDescriptorExtensions#FilterConventionDescriptorExtensions
    // https://github.com/ChilliCream/hotchocolate/blob/ee5813646fdfea81035c681989793514f33b5d94/src/HotChocolate/Data/src/Data/Filters/Convention/Extensions/FilterConventionDescriptorExtensions.cs#L102
    private static IFilterConventionDescriptor BindComparableType<T>(
        this IFilterConventionDescriptor descriptor,
        string? name = null)
        where T : struct
    {
        descriptor
            .BindRuntimeType<T, ComparableOperationFilterInputType<T>>()
            .BindRuntimeType<T?, ComparableOperationFilterInputType<T?>>();
        // TODO Why does this not work?
        // if (name is not null)
        // {
        //     descriptor
        //         .Configure<ComparableOperationFilterInputType<T>>(x => x.Name(name))
        //         .Configure<ComparableOperationFilterInputType<T?>>(x => x.Name($"Maybe{name}"));
        // }
        return descriptor;
    }
}

// See https://chillicream.com/docs/hotchocolate/fetching-data/sorting/#sort-conventions
public partial class CustomSortConvention : SortConvention
{
    protected override void Configure(ISortConventionDescriptor descriptor)
    {
        descriptor.AddDefaults();
        // Bind custom types
        descriptor.BindRuntimeType<GetHttpsResource, GetHttpsResourceSortType>();
        descriptor.BindRuntimeType<NamedMethodArgument, NamedMethodArgumentSortType>();
        descriptor.BindRuntimeType<CalorimetricData, CalorimetricDataSortType>();
        descriptor.BindRuntimeType<IData, DataSortType>();
        descriptor.BindRuntimeType<HygrothermalData, HygrothermalDataSortType>();
        descriptor.BindRuntimeType<OpticalData, OpticalDataSortType>();
        descriptor.BindRuntimeType<GeometricData, GeometricDataSortType>();
        descriptor.BindRuntimeType<PhotovoltaicData, PhotovoltaicDataSortType>();
    }
}