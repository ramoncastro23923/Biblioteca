using Biblioteca.Data;
using Biblioteca.Repositorio;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.Extensions.Hosting;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Kestrel
builder.WebHost.ConfigureKestrel(serverOptions => 
{
    serverOptions.ListenAnyIP(5000);
    serverOptions.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps());
});

// Configuração de serviços
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// Configuração do banco de dados
builder.Services.AddDbContext<BibliotecaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registro dos repositórios
builder.Services.AddScoped<ILivroRepository, LivroRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ILocacaoRepository, LocacaoRepository>();

// Background Service para cálculo de multas
builder.Services.AddHostedService<MultasBackgroundService>();

// Configuração de autenticação e autorização
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.LogoutPath = "/Account/Logout";
        options.Cookie.HttpOnly = true;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorization(options => 
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Administrador"));
});

var app = builder.Build();

// Pipeline de middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Inicialização do banco de dados
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<BibliotecaContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao inicializar o banco de dados");
    }
}

app.Run();

// Classe do Background Service
public class MultasBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<MultasBackgroundService> _logger;

    public MultasBackgroundService(IServiceProvider services, ILogger<MultasBackgroundService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _services.CreateScope())
            {
                var locacaoRepo = scope.ServiceProvider.GetRequiredService<ILocacaoRepository>();
                try
                {
                    await locacaoRepo.CalcularMultasAtrasadasAsync();
                    _logger.LogInformation("Multas calculadas em: {time}", DateTime.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao calcular multas");
                }
            }
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}