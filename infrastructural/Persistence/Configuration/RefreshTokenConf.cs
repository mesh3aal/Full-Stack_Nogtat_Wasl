
namespace Persistence_Layer.Persistence.Configuration;

public class RefreshTokenConf : IEntityTypeConfiguration<RefreshTokenEntity>
{
    public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Token).HasMaxLength(250);
        builder.HasIndex(x => x.Token).IsUnique();
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(a => a.UserId);
    }
}
