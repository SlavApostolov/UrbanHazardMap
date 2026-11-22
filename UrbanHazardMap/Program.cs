using Microsoft.AspNetCore.Identity;
using UrbanHazardMap.Data;
using UrbanHazardMap.Services;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using UrbanHazardMap.Infrastructure;

namespace UrbanHazardMap
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IIssueReportService, IssueReportService>();
            builder.Services.AddScoped<IPhotoStorageService,PhotoStorageService>();
            builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("Email"));

if (!string.IsNullOrWhiteSpace(
    builder.Configuration["Email:Host"]))
{
    builder.Services.AddScoped<
        IEmailService,
        SmtpEmailService>();
}
else
{
    builder.Services.AddScoped<
        IEmailService,
        ConsoleEmailService>();
}
            var app = builder.Build();
            
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();
               
                await context.Database.MigrateAsync();

                Console.WriteLine("Starting database seeding...");

                await DbInitializer.SeedDatabase(context, seedDemoData: app.Environment.IsDevelopment());

                var institutionCount = await context.Institutions.CountAsync();
                var categoryCount = await context.Categories.CountAsync();

                Console.WriteLine($"Institutions: {institutionCount}");
                Console.WriteLine($"Categories: {categoryCount}");
                Console.WriteLine("Database seeding completed!");
            }

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var adminEmail = builder.Configuration["Admin:Email"];
                var adminPassword = builder.Configuration["Admin:Password"];
                if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
                {
                    Console.WriteLine("Admin user email or password is not configured.");
                }
                else
                {
                    await IdentitySeeder.SeedRolesAsync(roleManager, userManager, adminEmail, adminPassword);
                }
            }

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

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = new List<CultureInfo> { new CultureInfo("en-US") },
                SupportedUICultures = new List<CultureInfo> { new CultureInfo("en-US") }
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
