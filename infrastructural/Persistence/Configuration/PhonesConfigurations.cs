
namespace Persistence_Layer.Persistence.Configuration;

public class PhonesConfigurations : IEntityTypeConfiguration<PhoneNumber>
{
    public void Configure(EntityTypeBuilder<PhoneNumber> builder)
    {
        builder.HasKey(p => p.Number);
        
        builder.Property(p => p.Number)
            .HasMaxLength(10)
            .ValueGeneratedNever();
        
        builder.HasOne(p => p.Place)
            .WithMany(p => p.Phones)
            .HasForeignKey(p => p.PlaceId);


    }
}
