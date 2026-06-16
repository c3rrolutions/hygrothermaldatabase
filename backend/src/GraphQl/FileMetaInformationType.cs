using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Data;
using Database.Extensions;
using HotChocolate;
using HotChocolate.Types;

namespace Database.GraphQl;

public sealed class FileMetaInformationType
    : ObjectType<FileMetaInformation>
{
    protected override void Configure(
        IObjectTypeDescriptor<FileMetaInformation> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor
            .Field(nameof(FileMetaInformation.DataFormatId)[..^2].FirstCharToLower())
            .Type<ObjectType<DataFormatDataLoader.DataFormat>>()
            .Cost(3)
            .ResolveWith<Resolvers>(_ => Resolvers.GetDataFormatAsync(default!, default!));
    }

    private sealed class Resolvers
    {
        public static Task<DataFormatDataLoader.DataFormat?> GetDataFormatAsync(
            [Parent] FileMetaInformation parent,
            IDataFormatByIdDataLoader byId
        )
        {
            return byId.LoadAsync(parent.DataFormatId);
        }
    }
}