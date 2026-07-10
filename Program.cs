using HuTest.Data;
using HuTest.Infrastructure;
using HuTest.Models.Entities;
using HuTest.Models.Options;
using HuTest.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// Carga secretos locales desde .env a variables de entorno (antes de leer la configuración).
DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

// MVC + Razor
builder.Services.AddControllersWithViews();

// Configuración tipada (spec.md §3 / plan.md §8)
builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection(AuthOptions.SectionName));
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.SectionName));

// Persistencia (EF Core + SQL Server)
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Hashing de contraseñas y servicios de dominio
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddScoped<INotificationService, EmailNotificationService>();
builder.Services.AddScoped<IAccountLockService, AccountLockService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IActivationService, ActivationService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();

// Autenticación por cookies con expiración deslizante = inactividad de sesión (RN-9)
var inactividad = builder.Configuration.GetValue<int?>("Auth:Sesion:InactividadMinutos") ?? 20;
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(inactividad);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.Name = "HuTest.Auth";
    });

var app = builder.Build();

// Crear/sembrar la base de datos de prueba
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();
    await DbSeeder.SeedAsync(db, hasher);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
