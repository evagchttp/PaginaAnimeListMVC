using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PaginaAnimeListMVC.Models;

namespace PaginaAnimeListMVC.Data;

public class ApplicationDbContext : IdentityDbContext
{   

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder){
        base.OnModelCreating(builder);
        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.UserName)
            .IsUnique();
        builder.Entity<ShowLike>()
            .HasKey(sl => new { sl.ApplicationUserId, sl.ShowId });
        builder.Entity<ShowLike>()
            .HasOne(sl => sl.ApplicationUser)
            .WithMany(u => u.ShowLikes)
            .HasForeignKey(sl => sl.ApplicationUserId);
        builder.Entity<ShowWatchlist>()
            .HasKey(sw => new { sw.ApplicationUserId, sw.ShowId });
        builder.Entity<ShowWatchlist>()
            .HasOne(sw => sw.ApplicationUser)
            .WithMany(u => u.ShowWatchlists)
            .HasForeignKey(sw => sw.ApplicationUserId);
    }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<ShowLike> ShowLikes { get; set; }
    public DbSet<ShowWatchlist> ShowWatchlists { get; set; }
}
