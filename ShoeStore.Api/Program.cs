using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Extensions.Logging;
using ShoeStore.Api.Mapping;
using ShoeStore.Api.Middleware;
using ShoeStore.Services;
using ShoeStoreData.Contexts;

var logger = LogManager.Setup()
    .LoadConfigurationFromFile("nlog.config")
    .GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Logging.AddNLog();

    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddApplicationServices();
    builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(o =>
    {
        o.IdleTimeout = TimeSpan.FromHours(2);
        o.Cookie.HttpOnly = true;
        o.Cookie.IsEssential = true;
    });

    builder.Services.AddCors(o => o.AddPolicy("WebPolicy", p =>
        p.WithOrigins("https://localhost:7098", "http://localhost:5211")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials()));

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<ExceptionMiddleware>();
    app.UseHttpsRedirection();
    app.UseCors("WebPolicy");
    app.UseSession();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Приложение упало при старте");
    throw;
}
finally
{
    LogManager.Shutdown();
}