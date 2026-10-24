namespace Persistence_Layer.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Location> Locations { get; set; }
    public DbSet<Directorate> Directorates { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<SubCategory>  SubCategories { get; set; }
    public DbSet<CommericalRegisteration> Registerations { get; set; }
    public DbSet<Favourite>  Favourites { get; set; }
    public DbSet<PhoneNumber> PhoneNumbers { get; set; }
    public DbSet<Place> Places { get; set; }
    public DbSet<PlaceImage>  PlaceImages { get; set; }
    public DbSet<Review>  Reviews { get; set; }
    public DbSet<Request>  Requests { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<ReviewReaction> ReviewReactions { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // this will disable the mvc external logins, and this is an api one so i don't care about it.
        builder.Ignore<IdentityUserLogin<string>>();
        
        // removing UserClaim table will result disabling claims functionalities such as [authorize(polices = "")]
        builder.Ignore<IdentityUserClaim<string>>();
        
        // remove if You don’t use role-based claims 
        builder.Ignore<IdentityRoleClaim<string>>();
        
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}