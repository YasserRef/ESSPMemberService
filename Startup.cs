using ESSPMemberService.Data;
using ESSPMemberService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oracle.EntityFrameworkCore;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
       // services.AddControllersWithViews().AddNewtonsoftJson(); // if you're using Newtonsoft

        services.AddDistributedMemoryCache();

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30); // Set timeout
            options.Cookie.HttpOnly = true; // Make the session cookie HTTP only
            options.Cookie.IsEssential = true; // Make the session cookie essential
        });

        services.AddControllersWithViews();

        // Add session services
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseOracle(Configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPermissionService, PermissionService>();


    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var context = app.ApplicationServices.GetService<ApplicationDbContext>();
        context.Database.EnsureCreated(); // Creates the database without using migrations

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }        

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        // Add the session middleware before using any session-dependent functionality
        app.UseSession();

        app.UseRouting();       

        app.UseAuthentication();
        app.UseAuthorization();        

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });


    }
}
