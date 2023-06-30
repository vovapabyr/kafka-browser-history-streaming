using BrowserHisotrySyteaming.Streaming;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<BrowserHistoryStreamingService>();
builder.Services.AddSingleton<BrowserHistoryStreamBuilderService>();
var app = builder.Build();

var applicationLifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
var streamBuilderService = app.Services.GetRequiredService<BrowserHistoryStreamBuilderService>();
applicationLifetime.ApplicationStopping.Register((object? toDispose) => (toDispose as BrowserHistoryStreamBuilderService).Dispose(), streamBuilderService);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
