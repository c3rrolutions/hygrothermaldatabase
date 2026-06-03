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
        EntityTypeBuilder<TEntity>
        ConfigureEntity<TEntity>(
            EntityTypeBuilder<TEntity> builder
        )
        where TEntity : Entity
    {
        // https://www.npgsql.org/efcore/modeling/generated-properties.html#guiduuid-generation
        builder
            .HasKey(e => e.Id);
        builder
            .Property(e => e.Id)
            .HasDefaultValueSql("gen_random_uuid()");
        // https://www.npgsql.org/efcore/modeling/concurrency.html#the-postgresql-xmin-system-column
        builder
            .Property(e => e.Version)
            .IsRowVersion();
        return builder;
    }

    private static
        EntityTypeBuilder<TData>
        ConfigureData<TData>(
            EntityTypeBuilder<TData> builder
        )
        where TData : class, IData
    {
        builder
            .ComplexProperty(
                data => data.AccessPolicy,
                _ => _.ToJson()
            );
        // .OwnsOne(
        //     dataAccessPolicy =>
        //     {
        //         dataAccessPolicy
        //             // The issues
        //             // https://github.com/dotnet/efcore/issues/33170#issuecomment-1966366300
        //             // https://github.com/dotnet/efcore/issues/31238
        //             // track the missing support of complex properties in owned types,
        //             // which would be used as below:
        //             // .ComplexProperty(_ => _.GrantedUserAndQuantity, d => d.ToJson())
        //             .Property(_ => _.GrantedUserAndQuantity)
        //             .HasConversion(
        //                 dictionary => JsonSerializer.Serialize(dictionary),
        //                 stringValue => JsonSerializer.Deserialize<Dictionary<Guid, uint?>>(stringValue) ?? new()
        //             );
        //     }
        // );
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
                               WHERE {nameof(GetHttpsResource.Id).Enquote()} = NEW.{nameof(GetHttpsResource.ParentId).Enquote()} AND COALESCE({string.Join(", ", GetHttpsResource.DataIdFieldNames.Select(_ => _.Enquote()))}) = COALESCE({string.Join(", ", GetHttpsResource.DataIdFieldNames.Select(_ => $"NEW.{_.Enquote()}"))})
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
                        IF COALESCE({string.Join(", ", GetHttpsResource.DataIdFieldNames.Select(_ => $"OLD.{_.Enquote()}"))}) <> COALESCE({string.Join(", ", GetHttpsResource.DataIdFieldNames.Select(_ => $"NEW.{_.Enquote()}"))})
                        THEN
                            RAISE EXCEPTION 'You cannot change the data ID of a resource.';
                        END IF;
                        """
                    )
                )
            );
        return builder;
    }

    private static
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
        return builder;
    }

    private static
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
        return builder;
    }

    private static
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
        return builder;
    }

    private static
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
        return builder;
    }

    private static
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
        return builder;
    }

    private static
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
        return builder;
    }

    private static
        EntityTypeBuilder<TAccessPolicy>
        ConfigureAccessPolicy<TAccessPolicy>(
            EntityTypeBuilder<TAccessPolicy> builder
        )
        where TAccessPolicy : AccessPolicyBase
    {
        builder.ComplexProperty(_ => _.UpperAccessLimitPerTimeDuration, _ => _.ToJson());
        builder.ComplexProperty(_ => _.AccessCountSinceStartTime, _ => _.ToJson());
        return builder;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(_schemaName);
        modelBuilder.HasPostgresExtension("pgcrypto"); // https://www.npgsql.org/efcore/modeling/generated-properties.html#guiduuid-generation
        ConfigureEntity(
            ConfigureGetHttpsResource(modelBuilder.Entity<GetHttpsResource>())
        )
        .ToTable(
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
                    $"NUM_NONNULLS({string.Join(", ", GetHttpsResource.DataIdFieldNames.Select(_ => _.Enquote()))}) = 1"
                );
                foreach (var triggerName in GetHttpsResource.TriggerNames)
                {
                    _.HasTrigger(triggerName);
                }
            }
        );
        ConfigureCalorimetricData(
            ConfigureData(
                ConfigureEntity(modelBuilder.Entity<CalorimetricData>())
            )
        ).ToTable("calorimetric_data");
        ConfigureGeometricData(
            ConfigureData(
                ConfigureEntity(modelBuilder.Entity<GeometricData>())
            )
        ).ToTable("geometric_data");
        ConfigureHygrothermalData(
            ConfigureData(
                ConfigureEntity(modelBuilder.Entity<HygrothermalData>())
            )
        ).ToTable("hygrothermal_data");
        ConfigureLifeCycleData(
            ConfigureData(
                ConfigureEntity(modelBuilder.Entity<LifeCycleData>())
            )
        ).ToTable("lifeCycle_data");
        ConfigureOpticalData(
            ConfigureData(
                ConfigureEntity(modelBuilder.Entity<OpticalData>())
            )
        ).ToTable("optical_data");
        ConfigurePhotovoltaicData(
            ConfigureData(
                ConfigureEntity(modelBuilder.Entity<PhotovoltaicData>())
            )
        ).ToTable("photovoltaic_data");
        ConfigureAccessPolicy(
            modelBuilder.Entity<UserAccessPolicy>()
        ).ToTable("user_access_policy");
        ConfigureAccessPolicy(
            modelBuilder.Entity<InstitutionAccessPolicy>()
        ).ToTable("institution_access_policy");
        ConfigureAccessPolicy(
            modelBuilder.Entity<OpenIdConnectApplicationAccessPolicy>()
        ).ToTable("open_id_connect_application_access_policy");
        ConfigureEntity(
            modelBuilder.Entity<User>()
        )
        .ToTable("user");
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IEntity).IsAssignableFrom(entityType.ClrType))
            {
                var entity = modelBuilder.Entity(entityType.ClrType);
                // https://www.npgsql.org/efcore/modeling/generated-properties.html#guiduuid-generation
                entity
                    .Property(nameof(IEntity.Id))
                    .HasDefaultValueSql("gen_random_uuid()");
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