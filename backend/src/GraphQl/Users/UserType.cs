using Database.Data;
using Database.GraphQl.Entities;
using HotChocolate.Types;

namespace Database.GraphQl.Users;

public sealed class UserType
    : EntityType<User, IUserByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<User> descriptor
    )
    {
        base.Configure(descriptor);
    }
}