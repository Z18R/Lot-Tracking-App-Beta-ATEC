using LotTrackingApp.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure IDbConnection using the connection string from appsettings.json
builder.Services.AddScoped<IDbConnection>(provider =>
    new SqlConnection(builder.Configuration.GetConnectionString("MES_ATEC_Connection")));

// Register LotTrackingService with both IConfiguration and IDbConnection
builder.Services.AddScoped<LotTrackingService>(provider =>
    new LotTrackingService(
        builder.Configuration,
        provider.GetRequiredService<IDbConnection>()
    ));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "lotTracking",
    pattern: "LotTracking/{action=Index}/{id?}",
    defaults: new { controller = "LotTracking" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
