using CourseLibrary.Api.Blue.Services;
using CourseLibrary.API.Blue.DbContexts;
using CourseLibrary.API.Blue.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Blue
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
            services.AddControllers(setupAction => setupAction.ReturnHttpNotAcceptable = true)
                .AddNewtonsoftJson(setupAction =>
                {
                    setupAction.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlDataContractSerializerFormatters()
                .ConfigureApiBehaviorOptions(setupAction =>
                {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Type = "https://courselibrary.com/modelvalidationproblem",
                        Title = "One or more model validation errors occurred.",
                        Status = StatusCodes.Status422UnprocessableEntity,
                        Detail = "See the errors property for details.",
                        Instance = context.HttpContext.Request.Path
                    };

                    problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

                    return new UnprocessableEntityObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();

            services.AddDbContext<CourseLibraryContext>(options =>
            {
                options.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=CourseLibraryDB;Trusted_Connection=True;");
            });
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
                app.UseExceptionHandler(appBulder =>
                {
                    appBulder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. please try again later");
                    });
                });
            }
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
