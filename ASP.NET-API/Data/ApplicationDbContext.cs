using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Entities;

namespace ASP.NET_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AuthorBook>()
                .HasKey(e => new { e.AuthorId, e.BookId });

            modelBuilder.Entity<Bill>()
                .Property(e => e.Amount).HasColumnType("decimal(18,2)");
        }

        public DbSet<APIKey> APIKeys { get; set; }
        public DbSet<APIRequest> APIRequests { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<AuthorBook> AuthorBooks { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CreatedBill> CreatedBills { get; set; }
        public DbSet<RestrictionByDomain> RestrictionsByDomain { get; set; }
        public DbSet<RestrictionByIP> RestrictionsByIP { get; set; }
    }
}
