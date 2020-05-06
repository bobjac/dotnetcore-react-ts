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
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using DbUp;
using QandA.Data;
using QandA.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using QandA.Authorization;

namespace TodoApi
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
            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            EnsureDatabase.For.SqlDatabase(connectionString);

            var upgrader = DeployChanges.To.SqlDatabase(connectionString, null)
                .WithScriptsEmbeddedInAssembly(System.Reflection.Assembly
                .GetExecutingAssembly())
                .WithTransaction()
                .Build();

            string executedStripts = string.Empty;

            if (upgrader.IsUpgradeRequired())
            {
                var result = upgrader.PerformUpgrade();
                foreach (var script in result.Scripts)
                {
                    if (!string.IsNullOrEmpty(executedStripts))
                    {
                        executedStripts += ", ";
                    }

                    executedStripts += script;
                }
            }

            //services.AddDbContext<TodoContext>(opt =>
            //    opt.UseInMemoryDatabase("TodoList"));
            services.AddControllers();

            services.AddScoped<IDataRepository, DataRepository>();

            services.AddCors(options =>
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins("http://localhost:3000")
                        .AllowCredentials()));

            services.AddSignalR();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = Configuration["Auth0:Authority"];
                options.Audience = Configuration["Auth0:Audience"];
            });

            services.AddHttpClient();
            services.AddAuthorization(options =>
                options.AddPolicy("MustBeQuestionAuthor", policy =>
                    policy.Requirements.Add(new MustBeQuestionAuthorRequirement()))
            );

            services.AddScoped<IAuthorizationHandler, MustBeQuestionAuthorHandler>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<QuestionsHub>("/questionshub");
            });
        }
    }
}
