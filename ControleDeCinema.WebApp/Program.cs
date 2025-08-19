using ControleDeCinema.Aplicacao.ModuloAutenticacao;
using ControleDeCinema.Aplicacao.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloGeneroFilme;
using ControleDeCinema.WebApp.ActionFilters;
using ControleDeCinema.WebApp.DependencyInjection;
using ControleDeCinema.WebApp.Orm;

namespace ControleDeCinema.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configuração de serviços personalizados
        builder.Services.AddScoped<AutenticacaoAppService>();

        builder.Services.AddScoped<GeneroFilmeAppService>();
        builder.Services.AddScoped<IRepositorioGeneroFilme, RepositorioGeneroFilmeEmOrm>();

        builder.Services.AddEntityFrameworkConfig(builder.Configuration);
        builder.Services.AddIdentityProviderConfig();
        builder.Services.AddJwtAuthenticationConfig();

        builder.Services.AddSerilogConfig(builder.Logging, builder.Configuration);

        // Configuração de serviços da Microsoft
        builder.Services.AddControllersWithViews(options =>
        {
            options.Filters.Add<ValidarModeloAttribute>();
        });

        builder.Services.AddHealthChecks()
            .AddDbContextCheck<ControleDeCinemaDbContext>();

        // Build das dependências
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.ApplyMigrations();

            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/erro");
        }

        app.UseAntiforgery();
        app.UseStaticFiles();
        app.UseRouting();

        app.MapHealthChecks("/health");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapDefaultControllerRoute();

        app.Run();
    }
}
