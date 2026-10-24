
namespace Persistence_Layer.Persistence.Configuration;

public class PlaceConfiguration : IEntityTypeConfiguration<Place>
{
    public void Configure(EntityTypeBuilder<Place> builder)
    {
        builder.HasKey(k => k.PlaceId);

        builder
            .HasOne(p => p.Request)
            .WithOne(r => r.Place)
            .HasForeignKey<Place>(p => p.RequestId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Rates)
            .WithOne(r => r.Place)
            .HasForeignKey(r => r.PlaceId)
            .IsRequired();

        builder.HasMany(p => p.Phones)
            .WithOne(p => p.Place)
            .HasForeignKey(p => p.PlaceId);
    }
}
