
namespace Persistence_Layer.Persistence.Configuration;

public class RequestConfiguration : IEntityTypeConfiguration<Request>
{
    public void Configure(EntityTypeBuilder<Request> builder)
    {
        builder.HasKey(r => r.RequestId);

        builder.HasOne(r => r.Place)
            .WithOne(p => p.Request)
            .HasForeignKey<Place>(p => p.RequestId)
            .IsRequired();

        // Configure the one-to-one relationship with PlaceDetails
        builder.HasOne(r => r.Place)
            .WithOne(r => r.Request)
            .HasForeignKey<Place>(r => r.PlaceId)
            .IsRequired();
    }
}
