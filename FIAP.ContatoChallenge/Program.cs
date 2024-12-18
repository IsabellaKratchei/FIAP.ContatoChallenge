using FIAP.ContatoChallenge.Data;
using FIAP.ContatoChallenge.Repository;
using FIAP.ContatoChallenge.Service;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Prometheus.SystemMetrics;
using System.Diagnostics;

namespace FIAP.ContatoTechChallenge
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder);

            var app = builder.Build();

            await MigrateDatabaseAsync(app);

            ConfigureMiddleware(app);

            _ = AtualizaUsoDeMemoria(); // Métrica de uso de memória

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

            // Banco de Dados
            builder.Services.AddEntityFrameworkSqlServer()
                .AddDbContext<BDContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Serviço HTTP para Regiões
            builder.Services.AddHttpClient<IRegiaoRepository, RegiaoAPIClient>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiRegiao:BaseUrl"] ?? "https://localhost:7294/api/");
            });

            builder.Services.AddHttpClient<IContatoRepository, ContatoAPIClient>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiContato:BaseUrl"] ?? "https://localhost:7245/api/");
            });

            // Prometheus
            builder.Services.AddSystemMetrics();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Debug);
        }

        private static async Task MigrateDatabaseAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                var context = services.GetRequiredService<BDContext>();
                await GaranteCriacaoDatabaseAsync(context, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao verificar ou criar o banco de dados.");
            }
        }

        private static async Task GaranteCriacaoDatabaseAsync(BDContext context, ILogger logger)
        {
            if (!await context.Database.CanConnectAsync())
            {
                logger.LogInformation("Banco de dados não encontrado, criando...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Migration(s) aplicada(s) com sucesso.");
            }
            else
            {
                logger.LogInformation("Banco de dados encontrado.");
            }
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contato API v1");
                    c.RoutePrefix = "swagger"; // Swagger na raiz
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            // Prometheus
            app.UseHttpMetrics();   // Métricas HTTP
            app.UseMetricServer();  // Expondo métricas em "/metrics"

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // Certifique-se de que os controllers estão sendo mapeados
            });
        }

        private static async Task AtualizaUsoDeMemoria()
        {
            var memoryGauge = Metrics.CreateGauge("process_resident_memory_bytes", "Resident memory usage of the process in bytes.");

            while (true)
            {
                var process = Process.GetCurrentProcess();
                memoryGauge.Set(process.WorkingSet64); // Métrica de memória
                await Task.Delay(10000);              // Atualiza a cada 10 segundos
            }
        }
    }
}
