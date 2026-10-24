
namespace Persistence_Layer.Persistence.Configuration;

public class AppUserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Email).IsUnique();
        
        // Role Relation overriden
        builder.HasOne(i => i.Role)
            .WithMany()
            .HasForeignKey(i => i.RoleId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
        // Request Relationship
        builder.HasMany(m => m.Requests)
            .WithOne(m => m.User)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        // Places Relationship
        builder.HasMany(m => m.Places)
            .WithOne(m => m.User)
            .HasForeignKey(fk => fk.UserId)
            .IsRequired();
        // Favourites Relationships
        builder.HasMany(m => m.Favourites)
            .WithOne(m => m.User)
            .HasForeignKey(fk => fk.UserId)
            .IsRequired();
        // Rates Relationship
        builder.HasMany(m => m.Rates)
            .WithOne(m => m.User)
            .IsRequired();
        // Registeration Relationship
        builder.HasMany(m => m.Registerations)
            .WithOne(m => m.User)
            .IsRequired();
        // ReviewReaction Relation
        builder.HasMany(r => r.ReviewReactions)
            .WithOne()
            .IsRequired();
    }
}
