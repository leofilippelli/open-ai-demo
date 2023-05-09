using Chat.Api.Config;
using Chat.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// use multiple appsettings files
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
    .AddEnvironmentVariables();

// add service as singleton so we use in-memory storage
builder.Services.AddSingleton<IOpenAiService, OpenAiService>();

// map service config
builder.Services.Configure<OpenAiServiceConfig>(builder.Configuration.GetSection("OpenAiService"));

// standard procedure
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();