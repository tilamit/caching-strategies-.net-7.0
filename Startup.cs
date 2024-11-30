using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleAppWithCaching.Interface;
using SampleAppWithCaching.Repository;
using SampleAppWithCaching.UserDbContext;
using EasyCaching.Core.Configurations;

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
            services.AddControllers();
            services.AddResponseCaching();

            //Get connection string from appsettings.json
            var ConnectionString = Configuration.GetConnectionString("DbConnectionString");
            var ConnectionStringDocker = Configuration.GetConnectionString("DbConnectionStringDocker");

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
            services.AddScoped<IUserSqlServerCache, UserSqlServerCacheRepository>();

            //Configure the redis caching provider  
            //Configuration 1
            services.AddEasyCaching(option =>
            {
                //use redis cache
                option.UseRedis(config =>
                {
                    config.DBConfig.Endpoints.Add(new ServerEndPoint("localhost", 6380));
                    config.DBConfig.ConnectionTimeout = 20000;
                    config.DBConfig.AsyncTimeout = 30000;
                    config.DBConfig.AbortOnConnectFail = false;
                    config.SerializerName = "mymsgpack";
                    config.MaxRdSecond = 0;
                }, "redis2")
                .WithMessagePack("mymsgpack")//with messagepack serialization
                ;

                //Configuration 2
                option.UseRedis(config =>
                {
                    config.DBConfig.Endpoints.Add(new ServerEndPoint("localhost", 6382));
                    config.DBConfig.ConnectionTimeout = 20000;
                    config.DBConfig.AsyncTimeout = 30000;
                    config.DBConfig.AbortOnConnectFail = false;
                    config.SerializerName = "mymsgpack";
                    config.MaxRdSecond = 0;
                }, "redis4")
                .WithMessagePack("mymsgpack")//with messagepack serialization
                ;
            });

            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = ConnectionStringDocker;
                options.SchemaName = "dbo";
                options.TableName = "CacheData";
            });
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
