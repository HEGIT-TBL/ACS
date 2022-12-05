using ACS.Core.Contracts.Services;
using ACS.Core.Models.Events;
using ACS.Core.Models;
using ACS.Core.Services.Repositories;
using ACS.Core.Services.Repositories.Events;
using ACS.Core.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add builder.Services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContextFactory<AccessControlDbContext>(options =>
    options.UseNpgsql("Server=localhost;Port=5432;Database=ACS_DB;User Id=Admin;Password=MyCoolP@$$w0rd;Include Error Detail=true"));
builder.Services.AddSingleton<IGenericRepositoryAsync<User>, UserRepository>();
builder.Services.AddSingleton<IGenericRepositoryAsync<KeyCard>, KeyCardRepository>();
builder.Services.AddSingleton<IGenericRepositoryAsync<AccessPoint>, AccessPointRepository>();
builder.Services.AddSingleton<IGenericRepositoryAsync<Camera>, CamerasRepository>();
builder.Services.AddSingleton<IGenericRepositoryAsync<ParkingLot>, ParkingLotRepository>();
builder.Services.AddSingleton<IGenericRepositoryAsync<AccessEvent>, AccessEventRepository>();
builder.Services.AddSingleton<IGenericRepositoryAsync<FaceRecognizedEvent>, FaceRecognizedEventRepository>();
builder.Services.AddSingleton<IGenericRepositoryAsync<ParkingLotStateChangedEvent>, ParkingLotStateChangedEventRepository>();
builder.Services.AddSingleton(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
