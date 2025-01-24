using IF.Lastfm.Core.Api;
using LastFM.Analytics.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

var dbProvider = builder.Configuration["DbProvider"];

builder.Services.AddDbContext<DataContext>(
	options => _ = dbProvider switch
	{
		"SQLite" => options.UseSqlite(builder.Configuration["SQLiteConnectionString"], b => b.MigrationsAssembly("LastFM.Analytics.Data.SQLite")),
		_ => throw new Exception($"Unsupported db provider: {dbProvider}")
	}
);

var lastFmClient = new LastfmClient(builder.Configuration["LastFmApiKey"], builder.Configuration["LastFmApiSecret"]);

builder.Services.AddSingleton(lastFmClient);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.UseSwaggerUI((options) => {
		options.SwaggerEndpoint("/openapi/v1.json", "LastFM.Analytics API");
	});
}

app.MapControllers();
app.UseHttpsRedirection();

var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<DataContext>().Database.Migrate();

app.Run();
