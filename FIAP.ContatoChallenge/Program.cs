using FIAP.ContatoChallenge.Data;
using FIAP.ContatoChallenge.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

            _ = AtualizaUsoDeMemoria(); // Chama a função para atualizar as métricas de memória sem bloquear o restante do código

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            builder.Services.AddEntityFrameworkSqlServer()
                    //.AddDbContext<BDContext>(o => o.UseSqlServer(@"Server=Bruno_PC\SQLEXPRESS;Database=FIAP;Trusted_Connection=True;TrustServerCertificate=true;"));
                    .AddDbContext<BDContext>(o => o.UseSqlServer(@"Server=DESKTOP-O0E2FJ0\SQLSERVER2022;Database=FIAP;Trusted_Connection=True;TrustServerCertificate=true;"));
            builder.Services.AddScoped<IContatoRepository, ContatoRepository>();
            builder.Services.AddScoped<IRegiaoRepository, RegiaoRepository>();
            builder.Services.AddSystemMetrics();
        }

        private static async Task MigrateDatabaseAsync(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<BDContext>();
                    await GaranteCriacaoDatabaseAsync(context);
                    await PopulaDDDSeVazioAsync(context);
                }
                catch (Exception ex)
                {
                    // Usar monitoramento de logs aqui, invés de WriteLine
                    Console.WriteLine($"Erro ao verificar ou criar o banco de dados: {ex.Message}");
                }
            }
        }

        private static async Task GaranteCriacaoDatabaseAsync(BDContext context)
        {
            if (!context.Database.CanConnect())
            {
                Console.WriteLine("Banco de dados não encontrado, criando...");
                context.Database.Migrate(); // Cria o banco e aplica as migrations
                Console.WriteLine("Migration(s) aplicada(s) com sucesso.");
            }
            else
            {
                Console.WriteLine("Banco de dados encontrado.");
            }
        }

        private static async Task PopulaDDDSeVazioAsync(BDContext context)
        {
            var existemRegistros = await context.DDDs.AnyAsync();

            if (!existemRegistros)
            {
                var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts", "ScriptDDD.sql");
                if (File.Exists(scriptPath))
                {
                    Console.WriteLine("Executando script para popular tabela de DDDs...");

                    var scriptSql = await File.ReadAllTextAsync(scriptPath);
                    await context.Database.ExecuteSqlRawAsync(scriptSql);
                    Console.WriteLine("Tabela de DDDs populada com sucesso.");
                }
                else
                {
                    Console.WriteLine($"O arquivo de script '{scriptPath}' não foi encontrado.");
                }
            }
            else
            {
                Console.WriteLine("Tabela de DDDs já está populada.");
            }
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            // Adiciona o middleware do Prometheus para expor as métricas
            app.UseHttpMetrics(); // Adiciona métricas HTTP automáticas
            app.UseMetricServer(); // Expondo métricas em "/metrics"

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }

        private static async Task AtualizaUsoDeMemoria()
        {
            var memoryGauge = Metrics.CreateGauge("process_resident_memory_bytes", "Resident memory usage of the process in bytes.");

            while (true)
            {
                var process = Process.GetCurrentProcess();
                memoryGauge.Set(process.WorkingSet64); // Atualiza a métrica com o uso atual de memória
                Console.WriteLine($"Atualizando memória residente: {process.WorkingSet64} bytes"); // Log para verificação
                await Task.Delay(10000); // Atualiza a cada 10 segundos
            }
        }
    }
}
