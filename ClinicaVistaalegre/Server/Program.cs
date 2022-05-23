using ClinicaVistaalegre.Server.Data;
using ClinicaVistaalegre.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); ;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));;

// Add services to the container.
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer(
    options =>
{
    options.LicenseKey = "eyJhbGciOiJQUzI1NiIsImtpZCI6IklkZW50aXR5U2VydmVyTGljZW5zZWtleS83Y2VhZGJiNzgxMzA0NjllODgwNjg5MTAyNTQxNGYxNiIsInR5cCI6ImxpY2Vuc2Urand0In0.eyJpc3MiOiJodHRwczovL2R1ZW5kZXNvZnR3YXJlLmNvbSIsImF1ZCI6IklkZW50aXR5U2VydmVyIiwiaWF0IjoxNjUyODUzOTUxLCJleHAiOjE2ODQzODk5NTEsImNvbXBhbnlfbmFtZSI6IkphdmllciDDgWx2YXJleiBDdWV2YXMiLCJjb250YWN0X2luZm8iOiJqYXZpZXJhbHZhcmV6Y3VAZ21haWwuY29tIiwiZWRpdGlvbiI6IkNvbW11bml0eSJ9.UY0Msp__UVpLSZbG_QrITQ93dShGsMbpxZz_hy9YZ-1WOywIPZ4BXqkkuhVpm6N4lNCkCzvVNfGOotMUQXjPH0q6YLT6G2B9LeNPIxEVQkFH50Jze8X-5EBigZJxheW8Q-wfHS8YrwmMPylkrZjm0VPn4WrS4sTy1z6MhhsC5KNtwxckRwwLGF8TQIInfwofO46DPcSS4n90fJnKG4Nen7GUhPypMWITBe1dddUexZmIeMcXUMz816qtbYi8T4A9ZVleoNnraK9wdDD1cFWFuBQ4hu4jDOMLOahmx7sPpLHsG-L5fZ_RJ9mDKTsqckOsOtktSfdDsRorN2Et1zx15w";
}
)
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options => {
        options.IdentityResources["openid"].UserClaims.Add("role");
        options.ApiResources.Single().UserClaims.Add("role");
    });
builder.Services.AddAuthentication()
    .AddIdentityServerJwt();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();


RolesData.SeedRoles(builder.Services.BuildServiceProvider()).Wait();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();