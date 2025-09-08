using BixWeb.Models;
using BixWeb.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var supportedCultures = new[] { new CultureInfo("pt-BR") };

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("pt-BR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Registro do serviço
builder.Services.AddScoped<IPageGeneratorService, PageGeneratorService>();
builder.Services.AddScoped<IPageGeneratorService1, PageGeneratorService1>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DbPrint>(options =>
                   options.UseSqlServer(builder.Configuration.GetConnectionString("DBPrintWEB")));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Cookies";
    options.DefaultChallengeScheme = "Cookies";
    options.DefaultSignInScheme = "Cookies";
}).AddCookie("Cookies", options =>
{
    options.LoginPath = "/Login/";
    options.AccessDeniedPath = "/Login/AccessDenied";

    // Aqui está a correção:
    options.ExpireTimeSpan = TimeSpan.FromDays(7); // ou o tempo desejado
    options.SlidingExpiration = true; // renova o tempo a cada requisição válida
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.AddSingleton<MercadoPagoService>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SomenteAdmin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Gerente", policy => policy.RequireRole("Gerente"));
    options.AddPolicy("Funcionario", policy => policy.RequireRole("Funcionario"));
    options.AddPolicy("Cliente", policy => policy.RequireRole("Cliente"));
});
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<IPageGeneratorService, PageGeneratorService>();
var app = builder.Build();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
                Path.Combine(builder.Environment.ContentRootPath, "Empresas")),
    RequestPath = "/Empresas"
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DbPrint>();
    context.Database.EnsureCreated();
    // DbInitializer.Initialize(context);
}

app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404)
    {
        context.Request.Path = "/not-found";
        await next();
    }
});
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Configura a localização
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.MapControllerRoute(
    name: "eventoDetails",
    pattern: "Eventos/Details/{idOrSlug}",
    defaults: new { controller = "Eventos", action = "Details" });

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
