using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WebsiteSellingSport.Models;
using WebsiteSellingSport.Repostitory;

namespace WebsiteSellingSport
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<QLBHQAContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<CategoryProductRepository>();
            services.AddScoped<CategoryRepository>();
            services.AddScoped<ProductRepository>();
            services.AddScoped<ProductImageRepository>();
            services.AddScoped<LoginRepository>();
            services.AddScoped<ProductColorSizeRepository>();
            services.AddScoped<OrderRepository>();
            services.AddScoped<ReportRepository>();
            services.AddScoped<CustomerRepository>();
            services.AddMvc().AddSessionStateTempDataProvider();
            services.AddAuthentication("MyCookie").AddCookie("MyCookie", config =>
            {
                config.Cookie.Name = "MyCookie";
                config.LoginPath = "/Admin/Home/Login";
                config.ReturnUrlParameter = "itworkingggggg";
            }).AddCookie("sthelse", config =>
            {
                config.Cookie.Name = "sthelse";
            });
            services.AddAuthorization(options =>
            {              
                options.AddPolicy("AdminOnly", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Admin"); // Yêu cầu người dùng có vai trò "Admin"
                });
                options.AddPolicy("EmployeePolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Employee");
                });
                options.AddPolicy("CustomerPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Customer");
                });
            });
            services.AddSession();
      
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                 name: "areas",
                 pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
               );

            });


        }
    }
}
