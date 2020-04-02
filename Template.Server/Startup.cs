using System;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Template.Db;
using Template.Server.Authentication;
using Template.Server.Dto;

namespace Template.Server {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllersWithViews();

            // TODO Use this code block to enabled [Authorize] by default
            /*
            services.AddMvc(options => {
                    options.EnableEndpointRouting = false;
                    options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser().Build()));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            */

            var keyString = Configuration.GetSection("AppSettings")["JwtSecret"] ?? "NotSetAtAll";
            LogonService.SecretString = keyString;

            services.AddAutoMapper(exp => exp.AddCollectionMappers(), typeof(MapperProfile));

            services.AddTransient<IUnitOfWork, UnitOfWork>(
                _ => new UnitOfWork(Configuration.GetConnectionString("DefaultConnectionString")));

            services.AddScoped(_ =>
                new ApiTokenFilterMiddleware("12345"));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSwaggerDocument();
            services.AddAuthentication(x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x => {
                x.Events = new JwtBearerEvents {
                    OnMessageReceived = context => {
                        context.Token = context.Request.Headers["Authorization"];
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context => {
                        Console.WriteLine("Authentication failed");
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(keyString)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            app.UseCors(builder => {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
            });

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days.
                // You may want to change this for production scenarios, see
                // https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseMiddleware(typeof(ApiTokenFilterMiddleware));

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    "default",
                    "api/{controller}/{action=Index}/{id?}");
            });
        }
    }
}