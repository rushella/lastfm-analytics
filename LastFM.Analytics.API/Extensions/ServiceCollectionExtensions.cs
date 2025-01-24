using IF.Lastfm.Core.Api;
using LastFM.Analytics.Data;
using Microsoft.EntityFrameworkCore;

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
			var lastFmClient = new LastfmClient(configuration["LastFmApiKey"], configuration["LastFmApiSecret"]);

			services.AddSingleton(lastFmClient);
			
			return services;
		}
	}
}