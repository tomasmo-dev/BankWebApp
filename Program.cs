using BankWebApp.env;
using BankWebApp.Services;

namespace BankWebApp
{
    /// <summary>
    /// The Program class is the entry point of the application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The Main method is responsible for setting up and running the web application.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the application.</param>
        public static void Main(string[] args)
        {
            // Initialize a new instance of the WebApplication builder with the provided command-line arguments.
            var builder = WebApplication.CreateBuilder(args);
            
            // Set the connection string from appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
            Envs.ConnectionString = connectionString!;

            // Register MVC controllers and views to the services.
            builder.Services.AddControllersWithViews();

            // Register application-specific services to the services container.
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddSingleton<MySignInManager>();
            builder.Services.AddSingleton<TransferService>();
            builder.Services.AddSingleton<DatabaseHealthService>();
            builder.Services.AddSingleton<DiskHealthService>();
            builder.Services.AddSingleton<MemoryHealthService>();

            // Register health checks to the services container.
            builder.Services.AddHealthChecks()
                .AddCheck<DatabaseHealthService>(nameof(DatabaseHealthService))
                .AddCheck<DiskHealthService>(nameof(DiskHealthService))
                .AddCheck<MemoryHealthService>(nameof(MemoryHealthService));

            // Configure authentication services with a custom scheme and cookie settings.
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

            // Build the web application instance.
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                // In non-development environments, use the exception handler middleware to handle exceptions globally.
                app.UseExceptionHandler("/Home/Error");
                // Use HSTS middleware to add the Strict-Transport-Security header to HTTP responses.
                app.UseHsts();
            }

            // Use HTTPS redirection middleware to redirect HTTP requests to HTTPS.
            app.UseHttpsRedirection();
            // Use static files middleware to serve static files.
            app.UseStaticFiles();

            // Use routing middleware to route requests to the correct endpoint.
            app.UseRouting();

            // Use authorization middleware to authorize users based on their roles and claims.
            app.UseAuthorization();
            
            // maps health checks to /health
            app.MapHealthChecks("/health");

            // Map the default controller route.
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Run the application.
            app.Run();
        }
    }
}