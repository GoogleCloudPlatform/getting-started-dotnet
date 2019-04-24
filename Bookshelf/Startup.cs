using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookshelf.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bookshelf
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
            services.AddSingleton<Services.ImageUploader>(provider  => 
                new Services.ImageUploader(Configuration["Bucket"]));
            // Choose a BookStoreBackend.
            BookStoreBackend backend = Enum.Parse<BookStoreBackend>(
                Configuration["BookStore"], ignoreCase: true);
            switch (backend)
            {
                case BookStoreBackend.Fake:
                    services.AddSingleton<IBookStore, FakeBookStore>();
                    break;
                case BookStoreBackend.Firestore:
                    services.AddSingleton<IBookStore, FirestoreBookStore>();
                    break;
                case BookStoreBackend.SqlServer:
                    services.AddEntityFrameworkSqlServer()
                        .AddDbContext<BookStoreDbContext>(options =>
                            options.UseSqlServer(Configuration.GetConnectionString("SqlServer")));
                    services.AddSingleton<IBookStore, DbBookStore>();
                    break;
                default:
                    throw new NotImplementedException(backend.ToString());
            }
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
