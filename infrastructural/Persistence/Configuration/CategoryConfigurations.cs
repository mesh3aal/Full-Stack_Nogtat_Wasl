
namespace Persistence_Layer.Persistence.Configuration;

public class CategoryConfigurations : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(c => c.CategoryName)
            .HasMaxLength(150)
            .IsRequired();
        
        builder.HasIndex(c => c.CategoryName)
            .IsUnique();
        
        builder.HasKey(c => c.CategoryId);

        // Relationship w subCategories.
        builder.HasMany(c => c.SubCategories)
            .WithOne(c => c.Category)
            .HasForeignKey(f => f.CategoryId)
            .IsRequired();
    }
}
