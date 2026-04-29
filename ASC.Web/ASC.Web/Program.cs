using ASC.Web.Data;
using Microsoft.EntityFrameworkCore;
using ASC.DataAccess.Repository;
using Microsoft.AspNetCore.Identity;
using ASC.Web.Navigation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ASC.Web.Configuration.ApplicationSettings>(
    builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<ASC.Web.Configuration.AdminUserSettings>(
    builder.Configuration.GetSection("AdminUser"));
builder.Services.Configure<ASC.Web.Configuration.EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        var googleAuthSection = builder.Configuration.GetSection("Authentication:Google");

        options.ClientId = googleAuthSection["ClientId"] ?? string.Empty;
        options.ClientSecret = googleAuthSection["ClientSecret"] ?? string.Empty;
    });
builder.Services.AddScoped<IUnitOfWork>(serviceProvider =>
{
    var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
    return new UnitOfWork(context);
});
builder.Services.AddScoped<IIdentitySeed, IdentitySeed>();
builder.Services.AddTransient<ASC.Web.Services.IEmailSender, ASC.Web.Services.AuthMessageSender>();
builder.Services.AddTransient<ASC.Web.Services.ISmsSender, ASC.Web.Services.AuthMessageSender>();

builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, ASC.Web.Services.AuthMessageSender>();
// Add services to the container.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<INavigationCacheOperations, NavigationCacheOperations>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await context.Database.MigrateAsync();

    var identitySeed = scope.ServiceProvider.GetRequiredService<IIdentitySeed>();
    await identitySeed.SeedAsync();
}

app.Run();
