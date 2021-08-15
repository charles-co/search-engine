using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Document.Data;
using Document.Repositories;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

namespace searchEngine
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
            DotNetEnv.Env.Load(".envs/.local/.postgres");
            var Host = DotNetEnv.Env.GetString("POSTGRES_HOST");
            var Port = DotNetEnv.Env.GetInt("POSTGRES_PORT");
            var Db = DotNetEnv.Env.GetString("POSTGRES_DB");
            var Username = DotNetEnv.Env.GetString("POSTGRES_USER");
            var Password = DotNetEnv.Env.GetString("POSTGRES_Password");
            var connection = $"Host={Host};Server=127.0.0.1;Port={Port};Database={Db};Username={Username};Password={Password}";
            services.AddDbContext<DataContext>(options => options.UseNpgsql(connection));
            services.AddScoped<IDataContext>(provider => provider.GetService<DataContext>());
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Search Engine", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "searchEngine v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
