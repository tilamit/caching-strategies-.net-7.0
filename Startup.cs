using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleAppWithCaching.Interface;
using SampleAppWithCaching.Repository;
using SampleAppWithCaching.UserDbContext;

namespace SampleAppWithCaching
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add serices to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddControllersWithViews();
            //services.AddMvc();
            services.AddControllers();
            services.AddResponseCaching();

            //Fetching Connection string from APPSETTINGS.JSON  
            var ConnectionString = Configuration.GetConnectionString("DbConnectionString");

            //Entity Framework  
            services.AddDbContext<UsersDbContext>(options => options.UseSqlServer(ConnectionString));
            
            services.AddMemoryCache();
            services.AddScoped<ICacheProvider, CacheProvider>();

            services.AddControllers(options =>
            {
                options.CacheProfiles.Add("UserCacheForOneMin",
                    new CacheProfile()
                    {
                        Duration = 60,
                        Location = ResponseCacheLocation.Any
                    });
            });
            services.AddScoped<IUser, UserRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseResponseCaching();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllers();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }
    }
}
