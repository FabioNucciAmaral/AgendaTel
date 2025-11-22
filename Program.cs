using AgendaTel.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

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

            var app = builder.Build();
            app.MapControllers();
            app.Run();
        }
    }
}
