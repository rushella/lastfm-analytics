using IF.Lastfm.Core.Api;
using LastFM.Analytics.API.Utils;
using LastFM.Analytics.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LastFM.Analytics.API.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddDataContext(this IServiceCollection services, IConfiguration configuration)
		{
			var dbProvider = configuration["DbProvider"];

			services.AddDbContext<DataContext>(
				options => _ = dbProvider switch
				{
					"SQLite" => options.UseSqlite(configuration["SQLiteConnectionString"], b => b.MigrationsAssembly("LastFM.Analytics.Data.SQLite")),
					_ => throw new Exception($"Unsupported db provider: {dbProvider}")
				}
			);

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

