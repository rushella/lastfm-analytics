using LastFM.Analytics.API.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSqlite<DataContext>(builder.Configuration["SqliteConnectionString"]);

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