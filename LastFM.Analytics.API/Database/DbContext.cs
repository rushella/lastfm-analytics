using LastFM.Analytics.API.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace LastFM.Analytics.API.Database;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
	public DbSet<User> Users { get; set; }
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable(nameof(User));
    }
}