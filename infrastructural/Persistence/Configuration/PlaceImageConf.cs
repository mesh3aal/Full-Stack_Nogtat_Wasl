
namespace Persistence_Layer.Persistence.Configuration;

public class PlaceImageConf : IEntityTypeConfiguration<PlaceImage>
{
    public void Configure(EntityTypeBuilder<PlaceImage> builder)
    {
        builder.HasKey(p => p.PlaceImageId);

        builder.Property(p => p.ImageUrl)
            .IsRequired();
    }
}
