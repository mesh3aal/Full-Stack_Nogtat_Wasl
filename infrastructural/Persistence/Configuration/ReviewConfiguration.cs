
namespace Persistence_Layer.Persistence.Configuration;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(k => k.ReviewId);

        builder.Property(k => k.RateValue).IsRequired();
        
        builder.Property(k => k.Note).IsRequired(false);

        // place relation
        builder.HasOne(p => p.Place)
            .WithMany(p => p.Rates);
        
        // review Reaction relation 
        builder.HasMany(r => r.ReviewReactions)
            .WithOne()
            .IsRequired();
    }
}
