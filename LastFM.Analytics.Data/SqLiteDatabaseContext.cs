using AppAny.Quartz.EntityFrameworkCore.Migrations;
using AppAny.Quartz.EntityFrameworkCore.Migrations.SQLite;
using LastFM.Analytics.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LastFM.Analytics.Data;

public sealed class SqLiteDatabaseContext(DbContextOptions<DatabaseContext> options)
    : DatabaseContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>((entity) => 
        {
            entity.ToTable(nameof(User));
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Name).IsUnique(true);
        });

        modelBuilder.AddQuartz(builder => builder.UseSqlite());
    }
}