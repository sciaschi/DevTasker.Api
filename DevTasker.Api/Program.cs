using DevTasker.Api.Hubs;
using DevTasker.Api.Middleware;
using DevTasker.Domain.Interface;
using DevTasker.Domain.ServiceLayer;
using DevTasker.Infrastructure;
using DevTasker.Infrastructure.Repository;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Debugging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
                     .AddEnvironmentVariables();

var serverVersion = new Version(8, 4, 7);

builder.Services.AddDbContext<DevTaskerDbContext>(options =>
       options.UseMySQL(builder.Configuration.GetConnectionString("MySQLConnection")));
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();
builder.Services.AddScoped<IWorkLogRepository, WorkLogRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskItemService, TaskItemService>();
builder.Services.AddScoped<IWorkLogService, WorkLogService>();
builder.Services.AddSignalR();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        var error = new ErrorResponse
        {
            Code = "VALIDATION_ERROR",
            Message = "One or more validation errors occurred.",
            Details = errors
        };

        return new BadRequestObjectResult(new { error });
    };
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
HttpMessageHandler? handler = null;

#if DEBUG
handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
    {
        Console.WriteLine($"[DEBUG] Splunk SSL errors: {errors}");
        return true; // DEV ONLY: accept any cert
    }
};
#endif


var configuration = builder.Configuration;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.EventCollector(
        splunkHost: configuration["Splunk:Host"],
        eventCollectorToken: configuration["Splunk:Token"],
        index: "testindex",
        uriPath: "services/collector/event")
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    SelfLog.Enable(msg =>
    {
        Console.Error.WriteLine("SERILOG-SELFLOG: " + msg);
    });

    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseCors("AllowFrontend");

app.MapHub<NotificationHub>("/hubs/notifications");

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
