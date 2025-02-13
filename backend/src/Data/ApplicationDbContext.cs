using Database.Enumerations;
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

    internal const string CoatedSideTypeName = "coated_side";
    internal const string DataKindTypeName = "data_kind";
    internal const string DataSubtypeTypeName = "data_subtype";
    internal const string DataTypeTypeName = "data_type";
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
    public DbSet<CalorimetricData> CalorimetricData { get; private set; } = default!;
    public DbSet<HygrothermalData> HygrothermalData { get; private set; } = default!;
    public DbSet<OpticalData> OpticalData { get; private set; } = default!;
    public DbSet<PhotovoltaicData> PhotovoltaicData { get; private set; } = default!;
    public DbSet<User> Users { get; private set; } = default!;
    public DbSet<DataProtectionKey> DataProtectionKeys { get; private set; } = default!;
    public DbSet<GeometricData> GeometricData { get; private set; } = default!;

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
            .Property(e => e.Id)
            .HasDefaultValueSql("gen_random_uuid()");
        // https://www.npgsql.org/efcore/modeling/concurrency.html#the-postgresql-xmin-system-column
        builder
            .Property(e => e.Version)
            .IsRowVersion();
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
        modelBuilder.HasPostgresExtension(
            "pgcrypto"); // https://www.npgsql.org/efcore/modeling/generated-properties.html#guiduuid-generation
        CreateEnumerations(modelBuilder, _schemaName);
        ConfigureIdentityEntities(modelBuilder);
        ConfigureEntity(
                modelBuilder.Entity<GetHttpsResource>()
            )
            .ToTable("get_https_resource");
        ConfigureEntity(
                modelBuilder.Entity<CalorimetricData>()
            )
            .OwnsOne(
                data => data.AppliedMethod,
                method =>
                {
                    method.OwnsMany(m => m.Arguments);
                    method.OwnsMany(m => m.Sources);
                }
            )
            .ToTable("calorimetric_data");
        ConfigureEntity(
                modelBuilder.Entity<HygrothermalData>()
            )
            .ToTable("hygrothermal_data");
        ConfigureEntity(
                modelBuilder.Entity<OpticalData>()
            )
            .ToTable("optical_data");
        ConfigureEntity(
                modelBuilder.Entity<PhotovoltaicData>()
            )
            .ToTable("photovoltaic_data");
        ConfigureEntity(
                modelBuilder.Entity<User>()
            )
            .ToTable("user");
        ConfigureEntity(
                modelBuilder.Entity<GeometricData>()
            )
            .ToTable("geometric_data");
    }
}