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
    public DbSet<Show> Shows { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
}
