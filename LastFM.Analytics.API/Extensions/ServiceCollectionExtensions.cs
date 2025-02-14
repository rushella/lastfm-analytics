using IF.Lastfm.Core.Api;
using LastFM.Analytics.API.SyncTasks;
using LastFM.Analytics.API.Utils;
using LastFM.Analytics.Data;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.AspNetCore;

namespace LastFM.Analytics.API.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddDataContext(this IServiceCollection services, IConfiguration configuration)
		{
			var dbProvider = configuration["DbProvider"];
			
			switch (dbProvider)
			{
				case "SQLite":
					services.AddDbContext<DatabaseContext, SqLiteDatabaseContext>((dbOptions) =>
					{
						var sqliteConnectionString = configuration["SQLiteConnectionString"];
						dbOptions.UseSqlite(sqliteConnectionString, sqliteOptions => sqliteOptions.MigrationsAssembly("LastFM.Analytics.Data"));
					});
					break;
				default:
					throw new Exception($"Unsupported db provider: {dbProvider}");
			}

			return services;
		}
		
		public static IServiceCollection AddPersistentQuartz(this IServiceCollection services, IConfiguration configuration)
		{
			var dbProvider = configuration["DbProvider"];

			services.AddQuartz(quartz =>
			{
				quartz.UsePersistentStore(config =>
				{
					switch (dbProvider)
					{
						case "SQLite":
							config.UseSQLite(configuration["SQLiteConnectionString"]);
							break;
						default:
							throw new Exception($"Unsupported db provider: {dbProvider}");
					}
					config.UseSystemTextJsonSerializer();
				});

				quartz.AddJob<FullSyncJob>(options => options.WithIdentity(nameof(FullSyncJob)).StoreDurably().Build());
			});

			services.AddQuartzServer(quartz =>
			{
				quartz.WaitForJobsToComplete = true;
			});
			
			return services;
		}

		public static IServiceCollection AddLastFMClient(this IServiceCollection services, IConfiguration configuration)
		{
			var httpClient = new HttpClient(new HttpRequestRateLimiter(new RateLimiter(5, TimeSpan.FromSeconds(1))));
			var lastFmClient = new LastfmClient(configuration["LastFmApiKey"], configuration["LastFmApiSecret"], httpClient);

			services.AddSingleton(lastFmClient);

			return services;
		}
	}
}

