using HotChocolate.Types;
using Database.Data;

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
            .Field(GraphQlConstants.UuidFieldName)
            .Type<NonNullType<UuidType>>();
        descriptor
            .Field(DatabaseIdFieldName)
            .Type<NonNullType<UuidType>>()
            .Resolve(_ => appSettings.DatabaseId);
        descriptor
            .Field(TimestampFieldName)
            .Type<NonNullType<DateTimeType>>();
        descriptor
            .Field(ResourceTreeFieldName)
            .Type<NonNullType<ObjectType<GetHttpsResourceTree>>>();
        descriptor
            .Field(x => x.Locale)
            .Type<NonNullType<LocaleType>>();
        descriptor
            .Field(x => x.Approval)
            .Type<NonNullType<ObjectType<ResponseApproval>>>();
        descriptor
            .Field(_ => _.PublishingState)
            .Ignore();
    }
}