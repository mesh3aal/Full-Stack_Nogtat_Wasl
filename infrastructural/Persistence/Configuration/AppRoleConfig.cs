
namespace Persistence_Layer.Persistence.Configuration;

public class AppRoleConfig : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
