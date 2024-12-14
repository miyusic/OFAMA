using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OFAMA.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using OFAMA.Service;
using System.Configuration;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//追記
// SendMailParams を構成ファイルから読み込むよう設定
builder.Services.Configure<SendMailParams>(builder.Configuration.GetSection("SendMailParams"));
//builder.Services.Configure<SendMailParams>(builder.Configuration);
builder.Services.AddScoped<IEmailSender, MailSender>();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

//追記
builder.Services.AddRazorPages();
//追記1214
builder.Services.AddControllers();
//

builder.Services.AddControllersWithViews();

var app = builder.Build();

//追記1214
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await DbInitializer.InitializeRolesAndAdminUserAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        // ログ出力やエラー処理
        Console.WriteLine($"Error initializing roles and users: {ex.Message}");
    }
}

////////////////

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
app.MapRazorPages();

app.Run();
