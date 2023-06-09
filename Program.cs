using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ThienASPMVC08032023.Database;
using ThienASPMVC08032023.Models;
using ThienASPMVC08032023.Repository.InterfaceRepo;
using ThienASPMVC08032023.Repository.Repo;
using ThienASPMVC08032023.Services;

namespace ThienASPMVC08032023
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            //Add Identity Services

            builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
                            .AddRoles<IdentityRole>()
                            .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.AddRazorPages();

            //Repository Pattern Register
            builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
            

            builder.Services.AddTransient<IEmailSender, EmailSender>();
            //Configure Get  Sendgrid ApiKey in appsetting.json
            builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration.GetSection("Sendgrid"));


            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;

                //Login setting
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login/";
                options.LogoutPath = "/logout/";
                options.AccessDeniedPath = "/AccessDenied/";
            });



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

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{Id?}")
                ;


            app.MapControllerRoute(
                name: "profileUser",
                pattern: "/{userName}/{action}",
                defaults: new
                {
                    controller = "Profile",
                    action = "index",
                    userName = "default",
                });



            app.Run();
        }
    }
}