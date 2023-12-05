using BankWebApp.Services;

namespace BankWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddSingleton<MySignInManager>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "custom";
                options.DefaultSignInScheme = "custom";
                options.DefaultSignOutScheme = "custom";
                options.DefaultChallengeScheme = "custom";
            })
                .AddCookie("custom", options =>
            {
                options.LoginPath = "/Home/Login";
                options.LogoutPath = "/Home/Logout";
                options.AccessDeniedPath = "/Home/AccessDenied";
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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}