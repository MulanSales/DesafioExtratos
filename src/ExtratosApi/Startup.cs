using System;
using ExtratosApi.Extensions;
using ExtratosApi.Models.Database;
using ExtratosApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace ExtratosApi
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Adding Swagger Generator Service
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "Extratos-Api", 
                    Version = "v1",
                    Description = "API para extratos pessoais",
                    Contact = new OpenApiContact {
                        Name = "Luan Sales",
                        Email = "luanmsacc@gmail.com",
                        Url = new Uri("https://github.com/MulanSales")
                    }
                });
            });

            // Injecting database settings to corresponding object
            services.Configure<DatabaseConnectorSettings>(Configuration.GetSection(nameof(DatabaseConnectorSettings)));
            services.AddSingleton<IDatabaseConnectorSettings>(sp => { 
                return sp.GetRequiredService<IOptions<DatabaseConnectorSettings>>().Value;
            });

            // Injecting releases service
            services.AddSingleton<ReleasesService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            // Add swagger documentation to pipeline
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Extrato-Api v1");
                SubmitMethod[] methods = {
                    SubmitMethod.Get, 
                    SubmitMethod.Post, 
                    SubmitMethod.Options, 
                    SubmitMethod.Delete, 
                    SubmitMethod.Put
                };
                c.SupportedSubmitMethods(methods);
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.ConfigureExceptionHandler();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
