
namespace Persistence_Layer.Persistence.Configuration;

public class ReviewReactionConfig : IEntityTypeConfiguration<ReviewReaction>
{
    public void Configure(EntityTypeBuilder<ReviewReaction> builder)
    {
        builder.HasKey(r => r.Id);
        builder.HasIndex(r => new { r.UserId, r.ReviewId }).IsUnique();
        
        builder.HasOne(x => x.Review)
        .WithMany(x => x.ReviewReactions)
        .HasForeignKey(x => x.ReviewId)
        .OnDelete(DeleteBehavior.Cascade)
        .IsRequired();
        
        // User
        builder.HasOne(x => x.User)
            .WithMany(x => x.ReviewReactions)
        .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        
    }
}
