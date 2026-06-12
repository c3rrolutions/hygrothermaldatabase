using Database.Data.AccessPolicies;
using Database.GraphQl.Entities;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.AccessPolicies;

public class DataAccessPolicyFilterType
    : AuditableEntityFilterType<DataAccessPolicy>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<DataAccessPolicy> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Name(nameof(DataAccessPolicyFilterType)[..^"FilterType".Length] + GraphQlConstants.FilterInputSuffix);
        descriptor.Field(_ => _.Combinator);
        descriptor.Field(_ => _.UserAccessPolicies);
        descriptor.Field(_ => _.InstitutionAccessPolicies);
        descriptor.Field(_ => _.OpenIdConnectApplicationAccessPolicies);
        // descriptor.Field(_ => _.Data);
        descriptor.Field(_ => _.CalorimetricData);
        descriptor.Field(_ => _.GeometricData);
        descriptor.Field(_ => _.HygrothermalData);
        descriptor.Field(_ => _.LifeCycleData);
        descriptor.Field(_ => _.OpticalData);
        descriptor.Field(_ => _.PhotovoltaicData);
    }
}