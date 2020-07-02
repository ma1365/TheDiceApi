using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NSwag;
using System;
using TheDiceApi.Managers;

namespace TheDiceApi
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
            services.AddCors(options =>
            {
                options.AddPolicy("CustomPolicy",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .SetPreflightMaxAge(TimeSpan.FromSeconds(2520));
                    });
            });

            //services.AddSwaggerGen(options =>
            //{
            //    options.SwaggerDoc("v1", new OpenApiInfo
            //    {
            //        Version = "v1",
            //        Title = "Dice Roller API",
            //        Description = "A simple API for rolling dice digitally",
            //        Contact = new OpenApiContact
            //        {
            //            Name = "TODO: Add Contact"
            //        },
            //        License = new OpenApiLicense
            //        {
            //            Name = "TODO: Add License"
            //        },
            //        // Extensions = 
            //        // TermsOfService = 
            //    });

            //    var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
            //    foreach (var xmlFile in xmlFiles)
            //    {
            //        options.IncludeXmlComments(x);
            //    }
            //});

            services.AddSwaggerDocument(options =>
            {
                options.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Dice Roller API";
                    document.Info.Description = "A simple API for rolling dice digitally";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new OpenApiContact
                    {
                        Name = "TODO: Add name"
                    };
                    document.Info.License = new OpenApiLicense
                    {
                        Name = "TODO: Add license"
                    };
                };
            });

            services.AddMvc()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
                

            services.AddControllers();

            services.AddTransient<IDiceRollManager, DiceRollManager>();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseSwagger();

            //app.UseSwaggerUI(options =>
            //{
            //    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Dice Roller API");
            //});

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseAuthorization();

            app.UseCors("CustomPolicy");


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}