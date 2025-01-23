var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI((options) => {
        options.SwaggerEndpoint("/openapi/v1.json", "LastFM.Analytics API");
    });
}

app.UseHttpsRedirection();

app.Run();