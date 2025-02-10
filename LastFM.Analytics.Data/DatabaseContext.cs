using LastFM.Analytics.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LastFM.Analytics.Data;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) 
	: DbContext(options)
{
	public DbSet<User> Users { get; set; }
}