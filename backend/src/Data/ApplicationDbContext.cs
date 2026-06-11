using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Database.Enumerations;
using Database.Extensions;
using GreenDonut.Data;
using Laraue.EfCoreTriggers.Common.Extensions;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using Database.GraphQl;
using Database.Data.Extensions;
using Database.Data.AccessPolicies;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;

namespace Database.Data;

// Inspired by
// [Authentication and authorization for SPAs](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-3.0)
// [Customize Identity Model](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-3.0)
public sealed class ApplicationDbContext
    : DbContext, IDataProtectionKeyContext
{
    private const string DefaultSchemaName = "database";
    private readonly string _schemaName;
    private readonly IClock _clock;

    internal const string CalorimetricObserverTypeName = "calorimetric_observer";
    internal const string CoatedSideTypeName = "coated_side";
    internal const string DataKindTypeName = "data_kind";
    internal const string IlluminantTypeName = "illuminant";
    internal const string LogicalCombinatorTypeName = "logical_combinator";
    internal const string OpticalComponentSubtypeTypeName = "optical_component_subtype";
    internal const string OpticalComponentTypeTypeName = "optical_component_type";
    internal const string PublishingStateTypeName = "publishing_state";
    internal const string StandardizerTypeName = "standardizer";

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IClock clock
    )
        : base(options)
    {
        // The schema-name option is set in `Metabase.Startup` by an invocation
        // of `UseSchemaName` on a `DbContextOptionsBuilder` instance.
        var schemaNameOptions = options.FindExtension<SchemaNameOptionsExtension>();
        _schemaName = schemaNameOptions is null ? DefaultSchemaName : schemaNameOptions.SchemaName;
        _clock = clock;
    }

    // https://docs.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types#dbcontext-and-dbset
    public DbSet<GetHttpsResource> GetHttpsResources { get; private set; } = default!;
    public DbSet<User> Users { get; private set; } = default!;
    public DbSet<DataProtectionKey> DataProtectionKeys { get; private set; } = default!;
    public DbSet<DataAccessPolicy> DataAccessPolicies { get; private set; } = default!;
    public DbSet<UserAccessPolicy> UserAccessPolicies { get; private set; } = default!;
    public DbSet<InstitutionAccessPolicy> InstitutionAccessPolicies { get; private set; } = default!;
    public DbSet<OpenIdConnectApplicationAccessPolicy> OpenIdConnectApplicationAccessPolicies { get; private set; } = default!;

    public IQueryable<GetHttpsResource> GetHttpsResourcesWithData =>
        GetHttpsResources.AsQueryable()
        .Include(_ => _.CalorimetricData)
        .Include(_ => _.GeometricData)
        .Include(_ => _.HygrothermalData)
        .Include(_ => _.LifeCycleData)
        .Include(_ => _.OpticalData)
        .Include(_ => _.PhotovoltaicData);

    public DbSet<CalorimetricData> CalorimetricData { get; private set; } = default!;
    public DbSet<GeometricData> GeometricData { get; private set; } = default!;
    public DbSet<HygrothermalData> HygrothermalData { get; private set; } = default!;
    public DbSet<LifeCycleData> LifeCycleData { get; private set; } = default!;
    public DbSet<OpticalData> OpticalData { get; private set; } = default!;
    public DbSet<PhotovoltaicData> PhotovoltaicData { get; private set; } = default!;

    // Inspired by https://github.com/openiddict/openiddict-core/issues/1376#issuecomment-1151275376
    // It is needed to fix the following error that occurred when trying to redeem OpenId Connect tokens in production:
    // `Cannot write DateTime with Kind=Unspecified to PostgreSQL type 'timestamp with time zone', only UTC is supported (...)`
    // See also https://github.com/openiddict/openiddict-core/issues/1376
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeUtcValueConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<DateTimeUtcValueConverter>();
        configurationBuilder.Properties<DateTimeOffset>().HaveConversion<DateTimeOffsetUtcValueConverter>();
        configurationBuilder.Properties<DateTimeOffset?>().HaveConversion<DateTimeOffsetUtcValueConverter>();
        configurationBuilder.Properties<OffsetDateTime>().HaveConversion<OffsetDateTimeUtcValueConverter>();
        configurationBuilder.Properties<OffsetDateTime?>().HaveConversion<OffsetDateTimeUtcValueConverter>();
        base.ConfigureConventions(configurationBuilder);
    }

    private sealed class DateTimeUtcValueConverter : ValueConverter<DateTime, DateTime>
    {
        public DateTimeUtcValueConverter()
            : base(
                v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            )
        {
        }
    }

    private sealed class DateTimeOffsetUtcValueConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
    {
        public DateTimeOffsetUtcValueConverter()
            : base(
            v => v.ToUniversalTime(),
            v => v
        )
        {
        }
    }

    private sealed class OffsetDateTimeUtcValueConverter : ValueConverter<OffsetDateTime, OffsetDateTime>
    {
        public OffsetDateTimeUtcValueConverter()
            : base(
            v => v.WithOffset(Offset.Zero),
            v => v
        )
        {
        }
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker
            .Entries<IAuditable>()
            .Where(_ =>
                _.State == EntityState.Added
                || _.State == EntityState.Modified
            // || _.State == EntityState.Deleted
            );
        var now = _clock.GetUtcNow().ToDateTimeOffset();
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity.CreatedAt == default)
                    {
                        entry.Entity.CreatedAt = now;
                    }
                    entry.Entity.UpdatedAt = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    break;
                    // NOTE that soft deletes do not cascade
                    // case EntityState.Deleted:
                    //     // soft delete
                    //     entry.State = EntityState.Modified;
                    //     entry.Entity.DeletedAt = now;
                    //     entry.Entity.UpdatedAt = now;
                    //     break;
            }
        }
    }

    public IQueryable<IData> Data(DataKind dataKind)
    {
        return dataKind switch
        {
            DataKind.CALORIMETRIC_DATA => CalorimetricData,
            DataKind.GEOMETRIC_DATA => GeometricData,
            DataKind.HYGROTHERMAL_DATA => HygrothermalData,
            DataKind.LIFE_CYCLE_DATA => LifeCycleData,
            DataKind.OPTICAL_DATA => OpticalData,
            DataKind.PHOTOVOLTAIC_DATA => PhotovoltaicData,
            _ => throw new ArgumentOutOfRangeException(nameof(dataKind), $"Unsupported data kind {dataKind}."),
        };
    }

    public Task<IData?> GetDataAsync(Guid id, DataKind dataKind, CancellationToken cancellationToken)
    {
        return Data(dataKind).SingleOrDefaultAsync(_ => _.Id == id, cancellationToken);
    }

    public IAsyncEnumerable<IData> GetAllDataAsync(
        Expression<Func<IData, bool>>? where = null,
        QueryContext<IData>? queryContext = null
    )
    {
        where ??= _ => true;
        return (
            CalorimetricData.AsQueryable<IData>()
            .Where(where)
            .With(queryContext, Sorting.DefaultEntityOrder)
            .ToAsyncEnumerable()
        )
        .Union(
            GeometricData.AsQueryable<IData>()
            .Where(where)
            .With(queryContext, Sorting.DefaultEntityOrder)
            .ToAsyncEnumerable()
        )
        .Union(
            HygrothermalData.AsQueryable<IData>()
            .Where(where)
            .With(queryContext, Sorting.DefaultEntityOrder)
            .ToAsyncEnumerable()
        )
        .Union(
            LifeCycleData.AsQueryable<IData>()
            .Where(where)
            .With(queryContext, Sorting.DefaultEntityOrder)
            .ToAsyncEnumerable()
        )
        .Union(
            OpticalData.AsQueryable<IData>()
            .Where(where)
            .With(queryContext, Sorting.DefaultEntityOrder)
            .ToAsyncEnumerable()
        )
        .Union(
            PhotovoltaicData.AsQueryable<IData>()
            .Where(where)
            .With(queryContext, Sorting.DefaultEntityOrder)
            .ToAsyncEnumerable()
        );
    }

    private static
        EntityTypeBuilder<TData>
        ConfigureData<TData>(
            EntityTypeBuilder<TData> builder
        )
        where TData : class, IData
    {
        builder
            .OwnsOne(
                data => data.AppliedMethod,
                method =>
                {
                    method.OwnsMany(m => m.Arguments);
                    method.OwnsMany(m => m.Sources);
                }
            );
        builder
            .OwnsOne(
                _ => _.Approval,
                approval =>
                {
                    approval
                        .Property(a => a.Variables)
                        .HasDefaultValueSql("'{}'");
                }
            );
        builder
            .OwnsMany(
                _ => _.Approvals,
                approvals =>
                {
                    approvals
                        .Property(a => a.Variables)
                        .HasDefaultValueSql("'{}'");
                }
            );
        return builder;
    }

    private
        EntityTypeBuilder<GetHttpsResource>
        ConfigureGetHttpsResource(
            EntityTypeBuilder<GetHttpsResource> builder
        )
    {
        // add partial indexes, see https://www.postgresql.org/docs/18/indexes-partial.html
        builder
            .HasIndex(_ => new { _.CalorimetricDataId })
            .HasFilter(
                $"""
                {nameof(GetHttpsResource.CalorimetricDataId).Enquote()} IS NOT NULL AND {nameof(GetHttpsResource.ParentId).Enquote()} IS NULL
                """
            )
            .IsUnique(true);
        builder
            .HasIndex(_ => new { _.GeometricDataId })
            .HasFilter(
                $"""
                {nameof(GetHttpsResource.GeometricDataId).Enquote()} IS NOT NULL AND {nameof(GetHttpsResource.ParentId).Enquote()} IS NULL
                """
            )
            .IsUnique(true);
        builder
            .HasIndex(_ => new { _.HygrothermalDataId })
            .HasFilter(
                $"""
                {nameof(GetHttpsResource.HygrothermalDataId).Enquote()} IS NOT NULL AND {nameof(GetHttpsResource.ParentId).Enquote()} IS NULL
                """
            )
            .IsUnique(true);
        builder
            .HasIndex(_ => new { _.LifeCycleDataId })
            .HasFilter(
                $"""
                {nameof(GetHttpsResource.LifeCycleDataId).Enquote()} IS NOT NULL AND {nameof(GetHttpsResource.ParentId).Enquote()} IS NULL
                """
            )
            .IsUnique(true);
        builder
            .HasIndex(_ => new { _.OpticalDataId })
            .HasFilter(
                $"""
                {nameof(GetHttpsResource.OpticalDataId).Enquote()} IS NOT NULL AND {nameof(GetHttpsResource.ParentId).Enquote()} IS NULL
                """
            )
            .IsUnique(true);
        builder
            .HasIndex(_ => new { _.PhotovoltaicDataId })
            .HasFilter(
                $"""
                {nameof(GetHttpsResource.PhotovoltaicDataId).Enquote()} IS NOT NULL AND {nameof(GetHttpsResource.ParentId).Enquote()} IS NULL
                """
            )
            .IsUnique(true);
        builder
            .BeforeInsert(trigger => trigger
                .SetTriggerName(GetHttpsResource.DataIdsMustMatchTriggerName)
                .Action(action => action
                    // .Condition(_ => _.New.ParentId != null)
                    .ExecuteRawSql(
                        $"""
                        IF NEW.{nameof(GetHttpsResource.ParentId).Enquote()} IS NOT NULL
                           AND (
                               SELECT COUNT({nameof(GetHttpsResource.Id).Enquote()})
                               FROM {_schemaName}.{GetHttpsResource.TableName.Enquote()}
                               WHERE {nameof(GetHttpsResource.Id).Enquote()} = NEW.{nameof(GetHttpsResource.ParentId).Enquote()} AND COALESCE({string.Join(", ", GetHttpsResource.DataIdFieldAndDataTableNames.Select(_ => _.Field.Enquote()))}) = COALESCE({string.Join(", ", GetHttpsResource.DataIdFieldAndDataTableNames.Select(_ => $"NEW.{_.Field.Enquote()}"))})
                           )
                           <> 1
                        THEN
                            RAISE EXCEPTION 'The new resource must have the same data ID as its parent.';
                        END IF;
                        """
                    )
                )
            );
        builder
            .BeforeUpdate(trigger => trigger
                .SetTriggerName(GetHttpsResource.DataIdCannotChangeTriggerName)
                .Action(action => action
                    .ExecuteRawSql(
                        $"""
                        IF COALESCE({string.Join(", ", GetHttpsResource.DataIdFieldAndDataTableNames.Select(_ => $"OLD.{_.Field.Enquote()}"))}) <> COALESCE({string.Join(", ", GetHttpsResource.DataIdFieldAndDataTableNames.Select(_ => $"NEW.{_.Field.Enquote()}"))})
                        THEN
                            RAISE EXCEPTION 'You cannot change the data ID of a resource.';
                        END IF;
                        """
                    )
                )
            );
        builder
            .BeforeDelete(trigger => trigger
                .SetTriggerName(GetHttpsResource.RootCanOnlyBeDeletedAlongsideItsDataTriggerName)
                .Action(action => action
                    .ExecuteRawSql(
                        $"""
                        IF OLD.{nameof(GetHttpsResource.ParentId).Enquote()} IS NULL AND (
                            {string.Join(" OR ", GetHttpsResource.DataIdFieldAndDataTableNames.Select(_ => $"""
                            (OLD.{_.Field.Enquote()} IS NOT NULL AND EXISTS (
                                SELECT 1 FROM {_schemaName}.{_.Table.Enquote()} 
                                WHERE {nameof(IData.Id).Enquote()} = OLD.{_.Field.Enquote()}
                            ))
                            """
                            ))}
                        ) THEN
                            RAISE EXCEPTION 'You cannot delete a root resource without also deleting the corresponding data in the same transaction.';
                        END IF;
                        """
                    )
                )
            );
        return builder;
    }

    private
        EntityTypeBuilder<TData>
        AddTriggerThatAssertsExistenceOfRootResource<TData>(
            EntityTypeBuilder<TData> builder,
            string triggerName,
            string dataIdColumn
        )
        where TData : class, IData
    {
        // In the generated migration turn `CREATE TRIGGER` into `CREATE
        // CONSTRAINT TRIGGER` and add `DEFERRABLE INITIALLY DEFERRED` before
        // `FOR EACH ...` .
        builder
            .AfterInsert(trigger => trigger
                .SetTriggerName(triggerName)
                .Action(action => action
                    .ExecuteRawSql(
                        $"""
                        IF NOT EXISTS (
                            SELECT 1 FROM {_schemaName}.{GetHttpsResource.TableName.Enquote()}
                            WHERE {nameof(GetHttpsResource.ParentId).Enquote()} IS NULL
                            AND {dataIdColumn.Enquote()} = NEW.{nameof(IData.Id)}
                        )
                        THEN
                            RAISE EXCEPTION 'You cannot insert data without also inserting the corresponding root resource in the same transaction.';
                        END IF;
                        """
                    )
                )
            );
        return builder;
    }

    private static
        EntityTypeBuilder<TData>
        AddTriggerThatCreatesDataAccessPolicyIfNecessary<TData>(
            EntityTypeBuilder<TData> builder,
            string triggerName,
            string dataIdColumn
        )
        where TData : class, IData
    {
        // In the generated migration turn `CREATE TRIGGER` into `CREATE
        // CONSTRAINT TRIGGER` and add `DEFERRABLE INITIALLY DEFERRED` before
        // `FOR EACH ...` .
        builder
            .AfterInsert(trigger => trigger
                .SetTriggerName(triggerName)
                .Action(action => action
                    .ExecuteRawSql(
                        $"""
                        INSERT INTO {DataAccessPolicy.TableName.Enquote()}
                        ({dataIdColumn.Enquote()}, {nameof(DataAccessPolicy.Combinator).Enquote()})
                        VALUES (NEW.{nameof(IEntity.Id).Enquote()}, '{LogicalCombinator.ALL.ToString().ToLowerInvariant()}')
                        ON CONFLICT ({dataIdColumn.Enquote()}) DO NOTHING;
                        RETURN NEW;
                        """
                    )
                // an alternative is
                // .InsertIfNotExists(
                //     insertedData => new DataAccessPolicy
                //     {
                //         CalorimetricDataId = insertedData.Id,
                //         Combinator = LogicalCombinator.ALL
                //     },
                //     data => new { data.CalorimetricDataId }
                // )
                )
            );
        return builder;
    }

    private
        EntityTypeBuilder<CalorimetricData>
        ConfigureCalorimetricData(
            EntityTypeBuilder<CalorimetricData> builder
        )
    {
        builder
            .HasMany(_ => _.Resources)
            .WithOne(_ => _.CalorimetricData)
            .HasForeignKey(_ => _.CalorimetricDataId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(_ => _.AccessPolicy)
            .WithOne(_ => _.CalorimetricData)
            .HasForeignKey<DataAccessPolicy>(_ => _.CalorimetricDataId)
            .OnDelete(DeleteBehavior.Cascade);
        AddTriggerThatAssertsExistenceOfRootResource(
            builder,
            global::Database.Data.CalorimetricData.AssertExistenceOfRootResourceTriggerName,
            nameof(GetHttpsResource.CalorimetricDataId)
        );
        AddTriggerThatCreatesDataAccessPolicyIfNecessary(
            builder,
            global::Database.Data.CalorimetricData.CreateDataAccessPolicyIfNecessaryTriggerName,
            nameof(DataAccessPolicy.CalorimetricDataId)
        );
        return builder;
    }

    private
        EntityTypeBuilder<GeometricData>
        ConfigureGeometricData(
            EntityTypeBuilder<GeometricData> builder
        )
    {
        builder
            .HasMany(_ => _.Resources)
            .WithOne(_ => _.GeometricData)
            .HasForeignKey(_ => _.GeometricDataId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(_ => _.AccessPolicy)
            .WithOne(_ => _.GeometricData)
            .HasForeignKey<DataAccessPolicy>(_ => _.GeometricDataId)
            .OnDelete(DeleteBehavior.Cascade);
        AddTriggerThatAssertsExistenceOfRootResource(
            builder,
            global::Database.Data.GeometricData.AssertExistenceOfRootResourceTriggerName,
            nameof(GetHttpsResource.GeometricDataId)
        );
        AddTriggerThatCreatesDataAccessPolicyIfNecessary(
            builder,
            global::Database.Data.GeometricData.CreateDataAccessPolicyIfNecessaryTriggerName,
            nameof(DataAccessPolicy.GeometricDataId)
        );
        return builder;
    }

    private
        EntityTypeBuilder<HygrothermalData>
        ConfigureHygrothermalData(
            EntityTypeBuilder<HygrothermalData> builder
        )
    {
        builder
            .HasMany(_ => _.Resources)
            .WithOne(_ => _.HygrothermalData)
            .HasForeignKey(_ => _.HygrothermalDataId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(_ => _.AccessPolicy)
            .WithOne(_ => _.HygrothermalData)
            .HasForeignKey<DataAccessPolicy>(_ => _.HygrothermalDataId)
            .OnDelete(DeleteBehavior.Cascade);
        AddTriggerThatAssertsExistenceOfRootResource(
            builder,
            global::Database.Data.HygrothermalData.AssertExistenceOfRootResourceTriggerName,
            nameof(GetHttpsResource.HygrothermalDataId)
        );
        AddTriggerThatCreatesDataAccessPolicyIfNecessary(
            builder,
            global::Database.Data.HygrothermalData.CreateDataAccessPolicyIfNecessaryTriggerName,
            nameof(DataAccessPolicy.HygrothermalDataId)
        );
        return builder;
    }

    private
        EntityTypeBuilder<LifeCycleData>
        ConfigureLifeCycleData(
            EntityTypeBuilder<LifeCycleData> builder
        )
    {
        builder
            .HasMany(_ => _.Resources)
            .WithOne(_ => _.LifeCycleData)
            .HasForeignKey(_ => _.LifeCycleDataId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(_ => _.AccessPolicy)
            .WithOne(_ => _.LifeCycleData)
            .HasForeignKey<DataAccessPolicy>(_ => _.LifeCycleDataId)
            .OnDelete(DeleteBehavior.Cascade);
        AddTriggerThatAssertsExistenceOfRootResource(
            builder,
            global::Database.Data.LifeCycleData.AssertExistenceOfRootResourceTriggerName,
            nameof(GetHttpsResource.LifeCycleDataId)
        );
        AddTriggerThatCreatesDataAccessPolicyIfNecessary(
            builder,
            global::Database.Data.LifeCycleData.CreateDataAccessPolicyIfNecessaryTriggerName,
            nameof(DataAccessPolicy.LifeCycleDataId)
        );
        return builder;
    }

    private
        EntityTypeBuilder<OpticalData>
        ConfigureOpticalData(
            EntityTypeBuilder<OpticalData> builder
        )
    {
        builder
            .HasMany(_ => _.Resources)
            .WithOne(_ => _.OpticalData)
            .HasForeignKey(_ => _.OpticalDataId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(_ => _.AccessPolicy)
            .WithOne(_ => _.OpticalData)
            .HasForeignKey<DataAccessPolicy>(_ => _.OpticalDataId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.OwnsMany(
            data => data.CielabColors
        ).ToTable(
            color =>
            {
                color.HasCheckConstraint(
                    $"CK_{nameof(OpticalData)}_{nameof(CielabColor)}s_{nameof(CielabColor.LStar)}",
                    $"{nameof(CielabColor.LStar).Enquote()} >= 0.0 AND {nameof(CielabColor.LStar).Enquote()} <= 100.0"
                );
            }
        );
        AddTriggerThatAssertsExistenceOfRootResource(
            builder,
            global::Database.Data.OpticalData.AssertExistenceOfRootResourceTriggerName,
            nameof(GetHttpsResource.OpticalDataId)
        );
        AddTriggerThatCreatesDataAccessPolicyIfNecessary(
            builder,
            global::Database.Data.OpticalData.CreateDataAccessPolicyIfNecessaryTriggerName,
            nameof(DataAccessPolicy.OpticalDataId)
        );
        return builder;
    }

    private
        EntityTypeBuilder<PhotovoltaicData>
        ConfigurePhotovoltaicData(
            EntityTypeBuilder<PhotovoltaicData> builder
        )
    {
        builder
            .HasMany(_ => _.Resources)
            .WithOne(_ => _.PhotovoltaicData)
            .HasForeignKey(_ => _.PhotovoltaicDataId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(_ => _.AccessPolicy)
            .WithOne(_ => _.PhotovoltaicData)
            .HasForeignKey<DataAccessPolicy>(_ => _.PhotovoltaicDataId)
            .OnDelete(DeleteBehavior.Cascade);
        AddTriggerThatAssertsExistenceOfRootResource(
            builder,
            global::Database.Data.PhotovoltaicData.AssertExistenceOfRootResourceTriggerName,
            nameof(GetHttpsResource.PhotovoltaicDataId)
        );
        AddTriggerThatCreatesDataAccessPolicyIfNecessary(
            builder,
            global::Database.Data.PhotovoltaicData.CreateDataAccessPolicyIfNecessaryTriggerName,
            nameof(DataAccessPolicy.PhotovoltaicDataId)
        );
        return builder;
    }

    private
        EntityTypeBuilder<DataAccessPolicy>
        ConfigureDataAccessPolicy(
            EntityTypeBuilder<DataAccessPolicy> builder
        )
    {
        // each data entity has at most one access policy and there is at most one global access policy (no data ID)
        builder
            .HasIndex(_ => new
            {
                _.CalorimetricDataId,
                _.GeometricDataId,
                _.HygrothermalDataId,
                _.LifeCycleDataId,
                _.OpticalDataId,
                _.PhotovoltaicDataId
            })
            .IsUnique(true)
            .AreNullsDistinct(false);
        // configure one-to-many associations
        builder
            .HasMany(_ => _.UserAccessPolicies)
            .WithOne(_ => _.DataAccessPolicy)
            .HasForeignKey(_ => _.DataAccessPolicyId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasMany(_ => _.InstitutionAccessPolicies)
            .WithOne(_ => _.DataAccessPolicy)
            .HasForeignKey(_ => _.DataAccessPolicyId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasMany(_ => _.OpenIdConnectApplicationAccessPolicies)
            .WithOne(_ => _.DataAccessPolicy)
            .HasForeignKey(_ => _.DataAccessPolicyId)
            .OnDelete(DeleteBehavior.Cascade);
        // add partial indexes, see https://www.postgresql.org/docs/18/indexes-partial.html
        builder
            .HasIndex(_ => new { _.CalorimetricDataId })
            .HasFilter(
                $"""
                {nameof(DataAccessPolicy.CalorimetricDataId).Enquote()} IS NOT NULL
                """
            )
            .IsUnique(true);
        builder
            .HasIndex(_ => new { _.GeometricDataId })
            .HasFilter(
                $"""
                {nameof(DataAccessPolicy.GeometricDataId).Enquote()} IS NOT NULL
                """
            )
            .IsUnique(true);
        builder
            .HasIndex(_ => new { _.HygrothermalDataId })
            .HasFilter(
                $"""
                {nameof(DataAccessPolicy.HygrothermalDataId).Enquote()} IS NOT NULL
                """
            )
            .IsUnique(true);
        builder
            .HasIndex(_ => new { _.LifeCycleDataId })
            .HasFilter(
                $"""
                {nameof(DataAccessPolicy.LifeCycleDataId).Enquote()} IS NOT NULL
                """
            )
            .IsUnique(true);
        builder
            .HasIndex(_ => new { _.OpticalDataId })
            .HasFilter(
                $"""
                {nameof(DataAccessPolicy.OpticalDataId).Enquote()} IS NOT NULL
                """
            )
            .IsUnique(true);
        builder
            .HasIndex(_ => new { _.PhotovoltaicDataId })
            .HasFilter(
                $"""
                {nameof(DataAccessPolicy.PhotovoltaicDataId).Enquote()} IS NOT NULL
                """
            )
            .IsUnique(true);
        builder
            .BeforeUpdate(trigger => trigger
                .SetTriggerName(DataAccessPolicy.DataIdCannotChangeTriggerName)
                .Action(action => action
                    .ExecuteRawSql(
                        $"""
                        IF COALESCE({string.Join(", ", DataAccessPolicy.DataIdFieldAndDataTableNames.Select(_ => $"OLD.{_.Field.Enquote()}"))}) <> COALESCE({string.Join(", ", DataAccessPolicy.DataIdFieldAndDataTableNames.Select(_ => $"NEW.{_.Field.Enquote()}"))})
                        THEN
                            RAISE EXCEPTION 'You cannot change the data ID of a data access policy.';
                        END IF;
                        """
                    )
                )
            );
        builder
            .BeforeDelete(trigger => trigger
                .SetTriggerName(DataAccessPolicy.GlobalPolicyCannotBeDeletedTriggerName)
                .Action(action => action
                    .ExecuteRawSql(
                        $"""
                        IF {string.Join(" AND ", DataAccessPolicy.DataIdFieldAndDataTableNames.Select(_ => $"OLD.{_.Field.Enquote()} IS NULL"))}
                        THEN
                            RAISE EXCEPTION 'You cannot delete the global data access policy.';
                        END IF;
                        """
                    )
                )
            );
        builder
            .BeforeDelete(trigger => trigger
                .SetTriggerName(DataAccessPolicy.CanOnlyBeDeletedAlongsideCorrespondingDataTriggerName)
                .Action(action => action
                    .ExecuteRawSql(
                        $"""
                        IF (
                            {string.Join(" OR ", DataAccessPolicy.DataIdFieldAndDataTableNames.Select(_ => $"""
                            (OLD.{_.Field.Enquote()} IS NOT NULL AND EXISTS (
                                SELECT 1 FROM {_schemaName}.{_.Table.Enquote()}
                                WHERE {nameof(IData.Id).Enquote()} = OLD.{_.Field.Enquote()}
                            ))
                            """
                            ))}
                        ) THEN
                            RAISE EXCEPTION 'You cannot delete a data access policy without also deleting the corresponding data in the same transaction.';
                        END IF;
                        """
                    )
                )
            );
        return builder;
    }

    private static
        EntityTypeBuilder<UserAccessPolicy>
        ConfigureUserAccessPolicy(
            EntityTypeBuilder<UserAccessPolicy> builder
        )
    {
        builder
            .HasIndex(_ => new { _.DataAccessPolicyId, _.UserId })
            .IsUnique(true);
        return builder;
    }

    private static
        EntityTypeBuilder<InstitutionAccessPolicy>
        ConfigureInstitutionAccessPolicy(
            EntityTypeBuilder<InstitutionAccessPolicy> builder
        )
    {
        builder
            .HasIndex(_ => new { _.DataAccessPolicyId, _.InstitutionId })
            .IsUnique(true);
        return builder;
    }

    private static
        EntityTypeBuilder<OpenIdConnectApplicationAccessPolicy>
        ConfigureOpenIdConnectApplicationAccessPolicy(
            EntityTypeBuilder<OpenIdConnectApplicationAccessPolicy> builder
        )
    {
        builder
            .HasIndex(_ => new { _.DataAccessPolicyId, _.ClientId })
            .IsUnique(true);
        return builder;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(_schemaName);
        modelBuilder.HasPostgresExtension("pgcrypto"); // https://www.npgsql.org/efcore/modeling/generated-properties.html#guiduuid-generation
        ConfigureGetHttpsResource(
            modelBuilder.Entity<GetHttpsResource>()
        ).ToTable(
            GetHttpsResource.TableName,
            _ =>
            {
                _.HasCheckConstraint(
                    $"CK_{nameof(GetHttpsResource)}_Root_Or_Child",
                    $"""
                    ({nameof(GetHttpsResource.ParentId).Enquote()} IS NULL AND "{nameof(GetHttpsResource.AppliedConversionMethod)}_{nameof(ToTreeVertexAppliedConversionMethod.MethodId)}" IS NULL)
                    OR ({nameof(GetHttpsResource.ParentId).Enquote()} IS NOT NULL AND "{nameof(GetHttpsResource.AppliedConversionMethod)}_{nameof(ToTreeVertexAppliedConversionMethod.MethodId)}" IS NOT NULL)
                    """
                );
                _.HasCheckConstraint(
                    $"CK_{nameof(GetHttpsResource)}_Exactly_One_Data_Set",
                    $"NUM_NONNULLS({string.Join(", ", GetHttpsResource.DataIdFieldAndDataTableNames.Select(_ => _.Field.Enquote()))}) = 1"
                );
                foreach (var triggerName in GetHttpsResource.TriggerNames)
                {
                    _.HasTrigger(triggerName);
                }
            }
        );
        ConfigureCalorimetricData(
            ConfigureData(
                modelBuilder.Entity<CalorimetricData>()
            )
        ).ToTable(
            global::Database.Data.CalorimetricData.TableName,
            _ =>
            {
                foreach (var triggerName in global::Database.Data.CalorimetricData.TriggerNames)
                {
                    _.HasTrigger(triggerName);
                }
            }
        );
        ConfigureGeometricData(
            ConfigureData(
                modelBuilder.Entity<GeometricData>()
            )
        ).ToTable(
            global::Database.Data.GeometricData.TableName,
            _ =>
            {
                foreach (var triggerName in global::Database.Data.GeometricData.TriggerNames)
                {
                    _.HasTrigger(triggerName);
                }
            }
        );
        ConfigureHygrothermalData(
            ConfigureData(
                modelBuilder.Entity<HygrothermalData>()
            )
        ).ToTable(
            global::Database.Data.HygrothermalData.TableName,
            _ =>
            {
                foreach (var triggerName in global::Database.Data.HygrothermalData.TriggerNames)
                {
                    _.HasTrigger(triggerName);
                }
            }
        );
        ConfigureLifeCycleData(
            ConfigureData(
                modelBuilder.Entity<LifeCycleData>()
            )
        ).ToTable(
            global::Database.Data.LifeCycleData.TableName,
            _ =>
            {
                foreach (var triggerName in global::Database.Data.LifeCycleData.TriggerNames)
                {
                    _.HasTrigger(triggerName);
                }
            }
        );
        ConfigureOpticalData(
            ConfigureData(
                modelBuilder.Entity<OpticalData>()
            )
        ).ToTable(
            global::Database.Data.OpticalData.TableName,
            _ =>
            {
                foreach (var triggerName in global::Database.Data.OpticalData.TriggerNames)
                {
                    _.HasTrigger(triggerName);
                }
            }
        );
        ConfigurePhotovoltaicData(
            ConfigureData(
                modelBuilder.Entity<PhotovoltaicData>()
            )
        ).ToTable(
            global::Database.Data.PhotovoltaicData.TableName,
            _ =>
            {
                foreach (var triggerName in global::Database.Data.PhotovoltaicData.TriggerNames)
                {
                    _.HasTrigger(triggerName);
                }
            }
        );
        ConfigureDataAccessPolicy(
            modelBuilder.Entity<DataAccessPolicy>()
        ).ToTable(
            DataAccessPolicy.TableName,
            _ =>
            {
                _.HasCheckConstraint(
                    $"CK_{nameof(DataAccessPolicy)}_At_Most_One_Data_Set",
                    $"NUM_NONNULLS({string.Join(", ", DataAccessPolicy.DataIdFieldAndDataTableNames.Select(_ => _.Field.Enquote()))}) <= 1"
                );
                foreach (var triggerName in DataAccessPolicy.TriggerNames)
                {
                    _.HasTrigger(triggerName);
                }
            }
        );
        ConfigureUserAccessPolicy(
            modelBuilder.Entity<UserAccessPolicy>()
        ).ToTable("user_access_policy");
        ConfigureInstitutionAccessPolicy(
            modelBuilder.Entity<InstitutionAccessPolicy>()
        ).ToTable("institution_access_policy");
        ConfigureOpenIdConnectApplicationAccessPolicy(
            modelBuilder.Entity<OpenIdConnectApplicationAccessPolicy>()
        ).ToTable("open_id_connect_application_access_policy");
        modelBuilder.Entity<User>()
            .ToTable("user");
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IEntity).IsAssignableFrom(entityType.ClrType))
            {
                var entity = modelBuilder.Entity(entityType.ClrType);
                entity.HasKey(nameof(IEntity.Id));
                // https://www.npgsql.org/efcore/modeling/generated-properties.html#guiduuid-generation
                entity
                    .Property(nameof(IEntity.Id))
                    .HasDefaultValueSql("uuidv7()")
                    .HasValueGenerator<NpgsqlSequentialGuidValueGenerator>();
                // https://www.npgsql.org/efcore/modeling/concurrency.html#the-postgresql-xmin-system-column
                entity
                    .Property(nameof(IEntity.Version))
                    .IsRowVersion();
            }
            if (typeof(IAssociation).IsAssignableFrom(entityType.ClrType))
            {
                var association = modelBuilder.Entity(entityType.ClrType);
                // https://www.npgsql.org/efcore/modeling/concurrency.html#the-postgresql-xmin-system-column
                association
                    .Property(nameof(IAssociation.Version))
                    .IsRowVersion();
            }
            if (typeof(IAuditable).IsAssignableFrom(entityType.ClrType))
            {
                var auditable = modelBuilder.Entity(entityType.ClrType);
                auditable
                    .Property(nameof(IAuditable.CreatedAt))
                    .HasDefaultValueSql("now()");
                auditable
                    .Property(nameof(IAuditable.UpdatedAt))
                    .HasDefaultValueSql("now()");
                // exclude soft-deleted entities with the effect that
                // `context.<Auditables>.ToList()` only returns rows where
                // `DeletedAt` is null and
                // `context.<Auditables>.IgnoreQueryFilters().ToList()` returns
                // all rows
                // entity
                //     .HasQueryFilter((IAuditable _) => _.DeletedAt == null);
            }
            if (typeof(IEntity).IsAssignableFrom(entityType.ClrType)
                && typeof(INamed).IsAssignableFrom(entityType.ClrType))
            {
                var entity = modelBuilder.Entity(entityType.ClrType);
                // https://www.npgsql.org/efcore/modeling/generated-properties.html#guiduuid-generation
                entity
                    .HasIndex(nameof(INamed.Name), nameof(IEntity.Id))
                    .IsUnique();
            }
            if (typeof(IEntity).IsAssignableFrom(entityType.ClrType)
                && typeof(IAuditable).IsAssignableFrom(entityType.ClrType))
            {
                var entity = modelBuilder.Entity(entityType.ClrType);
                // https://www.npgsql.org/efcore/modeling/generated-properties.html#guiduuid-generation
                entity
                    .HasIndex(nameof(IAuditable.CreatedAt), nameof(IEntity.Id))
                    .IsUnique();
            }
        }
    }
}