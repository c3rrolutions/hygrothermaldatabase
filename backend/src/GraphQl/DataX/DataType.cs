using Database.ApiRequests;
using Database.Data;
using Database.Extensions;
using Database.GraphQl.Scalars;
using HotChocolate.Types;

namespace Database.GraphQl.DataX;

public sealed class DataType(AppSettings appSettings)
 : InterfaceType<IData>
{
    public const string DatabaseIdFieldName = "databaseId";
    public const string TimestampFieldName = "timestamp";
    public const string ResourceTreeFieldName = "resourceTree";

    protected override void Configure(
        IInterfaceTypeDescriptor<IData> descriptor
    )
    {
        // `1..` is a range as introduced in https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8#indices-and-ranges
        descriptor.Name(nameof(IData)[1..]);
        descriptor
            .Field(_ => _.PublishingState)
            .Ignore();
        descriptor
            .Field(GraphQlConstants.IdFieldName)
            .Type<NonNullType<IdType>>();
        descriptor
            .Field(GraphQlConstants.UuidFieldName)
            .Type<NonNullType<UuidType>>();
        descriptor
            .Field(_ => _.UpdatedAt)
            .Name(TimestampFieldName);
        descriptor
            .Field(_ => _.CreatedAt);
        descriptor
            .Field(x => x.Locale)
            .Type<NonNullType<LocaleType>>();
        descriptor
            .Field(DatabaseIdFieldName)
            .Type<NonNullType<UuidType>>()
            .Resolve(_ => appSettings.DatabaseId);
        descriptor
            .Field(ResourceTreeFieldName)
            .Type<NonNullType<ObjectType<GetHttpsResourceTree>>>();
        descriptor
            .Field(x => x.Approval)
            .Type<NonNullType<ObjectType<ResponseApproval>>>();
        descriptor
            .Field(DataType.DatabaseIdFieldName[..^2].FirstCharToLower())
            .Type<ObjectType<DatabaseDataLoader.Database>>();
        descriptor
            .Field(nameof(IData.ComponentId)[..^2].FirstCharToLower())
            .Type<ObjectType<ComponentDataLoader.Component>>();
        descriptor
            .Field(nameof(IData.CreatorId)[..^2].FirstCharToLower())
            .Type<ObjectType<InstitutionDataLoader.Institution>>();
    }
}