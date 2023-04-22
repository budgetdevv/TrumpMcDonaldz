using Microsoft.AspNetCore.Builder;
// ReSharper disable once RedundantUsingDirective
using Microsoft.AspNetCore.Hosting; //Don't remove, it is for RELEASE build
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var services = builder.Services;

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

#if RELEASE
builder.WebHost
       .UseKestrel()
       .UseUrls("http://*:80");
#endif

var app = builder.Build();

#if DEBUG
app.UseSwagger();
app.UseSwaggerUI();
#endif

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();