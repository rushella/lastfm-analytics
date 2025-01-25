using LastFM.Analytics.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LastFM.Analytics.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
	public DbSet<User> Users { get; set; }
	public DbSet<SyncTask> SyncTasks { get; set; }
	
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
			entity.HasIndex(x => x.UserName).IsUnique(false);
		});
	}
}