using AgendaTel.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;

namespace AgendaTel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddControllers();
            builder.Services.AddScoped<IAgendaRepository, AgendaRepository>();
            builder.Services.AddDbContext<AgendaDbContext>();
            builder.Services.AddEndpointsApiExplorer();

            var xmlCommentsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            
            builder.Services.AddSwaggerGen(x =>
            {
                x.IncludeXmlComments(xmlCommentsPath);
                x.EnableAnnotations();
            });

            var app = builder.Build();
            app.MapControllers();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.Run();
        }
    }
}
