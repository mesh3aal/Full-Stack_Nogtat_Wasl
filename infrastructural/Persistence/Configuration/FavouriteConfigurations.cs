
namespace Persistence_Layer.Persistence.Configuration;

public class FavouriteConfigurations : IEntityTypeConfiguration<Favourite>
{
    public void Configure(EntityTypeBuilder<Favourite> builder)
    {
        builder.HasKey(c => new { IdentityUserId = c.UserId, c.PlaceId});

        builder.HasOne(c => c.User)
            .WithMany(c => c.Favourites)
            .HasForeignKey(c => c.UserId)
            .IsRequired();
        
        builder.HasOne(c => c.Place)
            .WithMany(c => c.FavouritedBy)
            .HasForeignKey(c => c.PlaceId)
            .IsRequired();
    }
}
