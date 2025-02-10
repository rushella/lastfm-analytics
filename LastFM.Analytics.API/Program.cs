using LastFM.Analytics.API.Extensions;
using LastFM.Analytics.API.Services;
using LastFM.Analytics.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddPersistentQuartz(builder.Configuration);
builder.Services.AddLastFMClient(builder.Configuration);
builder.Services.AddDataContext(builder.Configuration);

builder.Services.AddTransient<LastFmSyncService>();

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
scope.ServiceProvider.GetRequiredService<DatabaseContext>().Database.Migrate();

app.Run();