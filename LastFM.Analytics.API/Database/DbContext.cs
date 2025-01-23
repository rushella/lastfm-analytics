using LastFM.Analytics.API.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace LastFM.Analytics.API.Database;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
	public DbSet<User> Users { get; set; }
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>((entity) => 
		{
			entity.ToTable(nameof(User));
			entity.HasKey(x => x.Id);
			entity.HasIndex(x => x.Name).IsUnique(true);
		});

		modelBuilder.Entity<SyncTask>((entity) => 
		{
			entity.ToTable(nameof(SyncTask));
			entity.HasKey(x => x.Id);
			entity.HasIndex(x => x.UserId).IsUnique(false);
			entity.HasOne(x => x.User).WithMany(x => x.SyncTasks).HasForeignKey(x => x.UserId);
		});
	}
}