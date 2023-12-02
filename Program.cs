using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OFAMA.Data;
using OFAMA.Models;
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
builder.Services.Configure<SendMailParams>(builder.Configuration);
builder.Services.AddScoped<IEmailSender, MailSender>();
//builder.Services.AddTransient<IEmailSender, MailSender>();
//

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
//追記
builder.Services.AddRazorPages();
//
//�������܂ł�Microsoft�̓z�ƈꏏ
builder.Services.AddControllersWithViews();


var app = builder.Build();

//�����������Microsoft�̓z�ƈꏏ
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
