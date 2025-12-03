using NodaTime;
using HotChocolate.Types;
using NpgsqlTypes;

namespace Database.GraphQl.Common;

public sealed class OpenEndedDateTimeRangeType
    : ObjectType<NpgsqlRange<OffsetDateTime>>
{
    protected override void Configure(
        IObjectTypeDescriptor<NpgsqlRange<OffsetDateTime>> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();

        var suffixedName = nameof(OpenEndedDateTimeRangeType);
        descriptor.Name(suffixedName[..^"Type".Length]);

        descriptor
            .Field("from")
            .Type<DateTimeType>()
            .Resolve(context =>
                {
                    var range = context.Parent<NpgsqlRange<OffsetDateTime>>();
                    return range.LowerBoundInfinite
                        ? null
                        : range.LowerBound;
                }
            );

        descriptor
            .Field("until")
            .Type<DateTimeType>()
            .Resolve(context =>
                {
                    var range = context.Parent<NpgsqlRange<OffsetDateTime>>();
                    return range.UpperBoundInfinite
                        ? null
                        : range.UpperBound;
                }
            );
    }
}