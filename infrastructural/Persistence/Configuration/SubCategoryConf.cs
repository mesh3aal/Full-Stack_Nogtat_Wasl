
namespace Persistence_Layer.Persistence.Configuration;

public class SubCategoryConf : IEntityTypeConfiguration<SubCategory>
{
    public void Configure(EntityTypeBuilder<SubCategory> builder)
    {
        builder.HasKey(x => x.SubCategoryId);
        builder.ToTable("SubCategory");
        builder.HasIndex(x => x.Name).IsUnique();

        builder.HasMany(x => x.Places)
            .WithOne(x => x.SubCategory)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
