using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using WebApiAuthors.Filters;
using WebApiAuthors.Middlewares;
using WebApiAuthors.Services;
using WebApiAuthors.Utilities;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace WebApiAuthors
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container.

            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ExceptionFilter));
                options.Conventions.Add(new GroupByVersionSwagger());
            }).AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WebApiAuthors",
                    Version = "v1",
                    Description = "This is a web api learned in a course made for Master Felipe Gavilan in udemy (https://www.udemy.com/user/felipegaviln/?src=sac&kw=Felipe+GAvilan)",
                    Contact = new OpenApiContact()
                    {
                        Email = "herrera_payano@outlook.com",
                        Name = "Santo Herrera",
                        Url = new Uri("https://localhost:7092/swagger/index.html")
                    }
                });


                c.SwaggerDoc("v2", new OpenApiInfo { Title = "WebApiAuthors", Version = "v2" });
                c.OperationFilter<AddParameterHATEOAS>();
                c.OperationFilter<AddParametersXVersion>();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });


                var fileXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var routeXml = Path.Combine(AppContext.BaseDirectory, fileXml);

                c.IncludeXmlComments(routeXml);
            });

            services.AddResponseCaching();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option => option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWTkey"])),
                ClockSkew = TimeSpan.Zero
            });

            services.AddAutoMapper(typeof(Startup));

            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();



            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdmin", politic => politic.RequireClaim("IsAdmin"));
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    //builder.WithOrigins("").AllowAnyMethod().AllowAnyHeader();
                    builder.WithExposedHeaders(new string[] { "TotalRegistrationAmount" });
                });
            });

            services.AddDataProtection();
            services.AddTransient<HashService>();
            services.AddTransient<LinksGenerator>();
            services.AddTransient<HATESOUASAuthorFilterAttribute>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseHttpLogAnswer();
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiAuthors v1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "WebApiAuthors v2");
            });

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
