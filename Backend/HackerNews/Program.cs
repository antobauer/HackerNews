using HackerNews.Application.BackgroundServices;
using HackerNews.Application.Domain;
using HackerNews.Application.Interfaces;
using HackerNews.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200");
        });
});

builder.Services.AddTransient<IHackerNewsService, HackerNewsService>();
builder.Services.AddSingleton<IAppStartLogic, AppStartLogic>();
builder.Services.AddHostedService<CacheRefreshService>();

var app = builder.Build();

var startService = app.Services.GetService<IAppStartLogic>()!;

await startService.Start();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
