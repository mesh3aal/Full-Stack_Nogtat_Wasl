
namespace Persistence_Layer.Persistence.Configuration;

public class CommericalConfigurations : IEntityTypeConfiguration<CommericalRegisteration>
{
    public void Configure(EntityTypeBuilder<CommericalRegisteration> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.CrNumber)
            .HasMaxLength(15)
            .IsRequired();
        
        builder.Property(c => c.EntityName)
            .IsRequired();
        
        builder.Property(c => c.ImagePath).IsRequired();
        //placeDetails relationship
        builder.HasMany(d => d.Place)
            .WithOne(p => p.CommericalRegisteration)
            .HasForeignKey(d => d.CommericalRegisterationId)
            .IsRequired();
    }
}
