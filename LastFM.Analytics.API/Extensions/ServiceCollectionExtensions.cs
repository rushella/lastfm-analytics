using IF.Lastfm.Core.Api;
using LastFM.Analytics.API.Utils;
using LastFM.Analytics.Data;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace LastFM.Analytics.API.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddDataContext(this IServiceCollection services, IConfiguration configuration)
		{
			var dbProvider = configuration["DbProvider"];

			services.AddDbContext<DatabaseContext>(
				options => _ = dbProvider switch
				{
					"SQLite" => options.UseSqlite(configuration["SQLiteConnectionString"]),
					_ => throw new Exception($"Unsupported db provider: {dbProvider}")
				}
			);

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
				});
			});

			return services;
		}

		public static IServiceCollection AddLastFMClient(this IServiceCollection services, IConfiguration configuration)
		{
			var httpClient = new HttpClient(new LastFmRequestRateLimiter(new OutgoingRequestRateLimiter(5, TimeSpan.FromSeconds(1))));
			var lastFmClient = new LastfmClient(configuration["LastFmApiKey"], configuration["LastFmApiSecret"], httpClient);

			services.AddSingleton(lastFmClient);

			return services;
		}
	}
}

