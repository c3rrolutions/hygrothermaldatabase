using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Database.Enumerations;
using Database.Extensions;
using Database.GraphQl.Extensions;
using GreenDonut.Data;
using Laraue.EfCoreTriggers.Common.Extensions;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchemaNameOptionsExtension = Database.Data.Extensions.SchemaNameOptionsExtension;

namespace Database.Data;

// Inspired by
// [Authentication and authorization for SPAs](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-3.0)
// [Customize Identity Model](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-3.0)
public sealed class ApplicationDbContext
    : DbContext, IDataProtectionKeyContext
{
    private const string DefaultSchemaName = "database";
    private readonly string _schemaName;

    internal const string CalorimetricObserverTypeName = "calorimetric_observer";
    internal const string CoatedSideTypeName = "coated_side";
    internal const string DataKindTypeName = "data_kind";
    internal const string IlluminantTypeName = "illuminant";
    internal const string OpticalComponentSubtypeTypeName = "optical_component_subtype";
    internal const string OpticalComponentTypeTypeName = "optical_component_type";
    internal const string PublishingStateTypeName = "publishing_state";
    internal const string StandardizerTypeName = "standardizer";

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options
    )
        : base(options)
    {
        // The schema-name option is set in `Metabase.Startup` by an invocation of
        // `UseSchemaName` on a `DbContextOptionsBuilder` instance.
        var schemaNameOptions = options.FindExtension<SchemaNameOptionsExtension>();
        _schemaName = schemaNameOptions is null ? DefaultSchemaName : schemaNameOptions.SchemaName;
    }

    // https://docs.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types#dbcontext-and-dbset
    public DbSet<GetHttpsResource> GetHttpsResources { get; private set; } = default!;
    public DbSet<User> Users { get; private set; } = default!;
    public DbSet<DataProtectionKey> DataProtectionKeys { get; private set; } = default!;
    public DbSet<InstitutionAccessRights> InstitutionAccessRights { get; private set; } = default!;


    public DbSet<CalorimetricData> CalorimetricData { get; private set; } = default!;
    public DbSet<GeometricData> GeometricData { get; private set; } = default!;
    public DbSet<HygrothermalData> HygrothermalData { get; private set; } = default!;
    public DbSet<PhotovoltaicData> PhotovoltaicData { get; private set; } = default!;
    public DbSet<OpticalData> OpticalData { get; private set; } = default!;

    public IQueryable<IData> Data(DataKind dataKind)
    {
        return dataKind switch
        {
            DataKind.CALORIMETRIC_DATA => CalorimetricData,
            DataKind.GEOMETRIC_DATA => GeometricData,
            DataKind.HYGROTHERMAL_DATA => HygrothermalData,
            DataKind.OPTICAL_DATA => OpticalData,
            DataKind.PHOTOVOLTAIC_DATA => PhotovoltaicData,
            _ => throw new ArgumentOutOfRangeException(nameof(dataKind), $"Unsupported data kind {dataKind}."),
        };
    }

    public Task<IData?> GetDataAsync(Guid id, DataKind dataKind, CancellationToken cancellationToken)
    {
        return Data(dataKind).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public IAsyncEnumerable<IData> GetAllDataAsync(
        Expression<Func<IData, bool>> where,
        QueryContext<IData> queryContext
    )
    {
        return (
            CalorimetricData.AsQueryable<IData>()
            .With(queryContext, sort => sort.StabilizeOrder())
            .Where(where)
            .ToAsyncEnumerable()
        )
        .Union(
            GeometricData.AsQueryable<IData>()
            .With(queryContext, sort => sort.StabilizeOrder())
            .Where(where)
            .ToAsyncEnumerable()
        )
        .Union(
            HygrothermalData.AsQueryable<IData>()
            .With(queryContext, sort => sort.StabilizeOrder())
            .Where(where)
            .ToAsyncEnumerable()
        )
        .Union(
            OpticalData.AsQueryable<IData>()
            .With(queryContext, sort => sort.StabilizeOrder())
            .Where(where)
            .ToAsyncEnumerable()
        )
        .Union(
            PhotovoltaicData.AsQueryable<IData>()
            .With(queryContext, sort => sort.StabilizeOrder())
            .Where(where)
            .ToAsyncEnumerable()
        );
    }

    private static void CreateEnumerations(ModelBuilder builder, string schemaName)
    {
        // https://www.npgsql.org/efcore/mapping/enum.html?tabs=with-datasource#mapping-your-enum
        // Create enumerations in the same schema used by
        // `NpgsqlDataSourceBuilder.MapEnum` in `Startup`. The format of how to
        // specify the type name here and in `Startup` differ slightly. This
        // was complained about in
        // https://github.com/npgsql/efcore.pg/issues/2963#issuecomment-1818866360
        builder.HasPostgresEnum<DataKind>(schemaName, DataKindTypeName);
        builder.HasPostgresEnum<Standardizer>(schemaName, StandardizerTypeName);
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
                x => x.Approval,
                approval =>
                {
                    approval
                        .Property(a => a.Variables)
                        .HasDefaultValueSql("'{}'");
                }
            );
        builder
            .OwnsMany(
                x => x.Approvals,
                approvals =>
                {
                    approvals
                        .Property(a => a.Variables)
                        .HasDefaultValueSql("'{}'");
                }
            );
        return builder;
    }

    private static
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
            );
        builder
            .HasIndex(_ => new { _.GeometricDataId })
            .HasFilter(
                $"""
                {nameof(GetHttpsResource.GeometricDataId).Enquote()} IS NOT NULL AND {nameof(GetHttpsResource.ParentId).Enquote()} IS NULL
                """
            );
        builder
            .HasIndex(_ => new { _.HygrothermalDataId })
            .HasFilter(
                $"""
                {nameof(GetHttpsResource.HygrothermalDataId).Enquote()} IS NOT NULL AND {nameof(GetHttpsResource.ParentId).Enquote()} IS NULL
                """
            );
        builder
            .HasIndex(_ => new { _.OpticalDataId })
            .HasFilter(
                $"""
                {nameof(GetHttpsResource.OpticalDataId).Enquote()} IS NOT NULL AND {nameof(GetHttpsResource.ParentId).Enquote()} IS NULL
                """
            );
        builder
            .HasIndex(_ => new { _.PhotovoltaicDataId })
            .HasFilter(
                $"""
                {nameof(GetHttpsResource.PhotovoltaicDataId).Enquote()} IS NOT NULL AND {nameof(GetHttpsResource.ParentId).Enquote()} IS NULL
                """
            );
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
                               FROM "{GetHttpsResource.TableName}"
                               WHERE COALESCE({string.Join(", ", GetHttpsResource.DataIdFieldNames.Select(_ => _.Enquote()))}) = COALESCE({string.Join(", ", GetHttpsResource.DataIdFieldNames.Select(_ => $"NEW.{_.Enquote()}"))})
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
                    $"{nameof(CielabColor.LStar)}.Enquote() >= 0.0 AND {nameof(CielabColor.LStar).Enquote()} <= 100.0"
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

    private static void ConfigureIdentityEntities(
        ModelBuilder builder
    )
    {
        // https://stackoverflow.com/questions/19902756/asp-net-identity-dbcontext-confusion/35722688#35722688
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(_schemaName);
        modelBuilder.HasPostgresExtension("pgcrypto"); // https://www.npgsql.org/efcore/modeling/generated-properties.html#guiduuid-generation
        CreateEnumerations(modelBuilder, _schemaName);
        ConfigureIdentityEntities(modelBuilder);
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
            ).ToTable("calorimetric_data")
        );
        ConfigureGeometricData(
            ConfigureData(
                ConfigureEntity(modelBuilder.Entity<GeometricData>())
            ).ToTable("geometric_data")
        );
        ConfigureHygrothermalData(
            ConfigureData(
                ConfigureEntity(modelBuilder.Entity<HygrothermalData>())
            ).ToTable("hygrothermal_data")
        );
        ConfigureOpticalData(
            ConfigureData(
                ConfigureEntity(modelBuilder.Entity<OpticalData>())
            )
        ).ToTable("optical_data");
        ConfigurePhotovoltaicData(
            ConfigureData(
                ConfigureEntity(modelBuilder.Entity<PhotovoltaicData>())
            ).ToTable("photovoltaic_data")
        );
        ConfigureEntity(
                modelBuilder.Entity<User>()
            )
            .ToTable("user");
        ConfigureEntity(
                modelBuilder.Entity<InstitutionAccessRights>()
            )
            .ToTable("institution_access_rights");
    }
}