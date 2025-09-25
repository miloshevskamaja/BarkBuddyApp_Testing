using System.Data.Common;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BarkBuddyApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
  
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Producer> Producers { get; set; }
        public virtual DbSet<DogBreed> DogBreeds { get; set; }
        public virtual DbSet<Toys> Toys { get; set; }
        public virtual DbSet<Buyer> Buyers { get; set; }
       
        public virtual DbSet<OrderViewModel> Orders { get; set; }
        public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public virtual DbSet<Grooming> Groomings { get; set; }
        public virtual DbSet<GroomingDog> GroomingDogs { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
   
        
        // Configure many-to-many relationship
        modelBuilder.Entity<Product>()
                .HasMany(p => p.DogBreeds)
                .WithMany(d => d.Products)
                .Map(m =>
                {
                    m.ToTable("ProductDogBreeds"); // Create a join table
                    m.MapLeftKey("ProductId");
                    m.MapRightKey("DogBreedId");
                });
            base.OnModelCreating(modelBuilder);
        }
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public ApplicationDbContext(DbConnection existingConnection, bool throwIfV1Schema)
     : base(existingConnection, throwIfV1Schema) { }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //public System.Data.Entity.DbSet<BarkBuddyApp.Models.GroomingDog> GroomingDogs { get; set; }

        //public System.Data.Entity.DbSet<BarkBuddyApp.Models.Grooming> Groomings { get; set; }

        // public System.Data.Entity.DbSet<BarkBuddyApp.Models.Toys> Toys { get; set; }
    }
}