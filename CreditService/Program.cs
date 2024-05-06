using CreditService;
using CreditService.BL;
using CreditService.BL.Services;
using CreditService.Common.System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.ConfigureAppDb();

builder.ConfigureAppServices();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestInterceptorMiddleware>();
app.UseMiddleware<MetricMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Configurator.Migrate(app.Services);
app.Run();
