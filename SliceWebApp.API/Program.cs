using Carter;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SliceWebApp.API.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultPostgreConnection");

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddMediatR(config => 
    config.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddCarter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();

app.UseHttpsRedirection();

app.Run();
