using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;
using WebApi.Common;
using WebApi.Common.Interfaces;
using WebApi.Data;
using WebApi.Data.User;
using WebApi.Middlewares;

namespace WebApi
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
            //注入mvc，view,api,json,controller,razor等等
            services.AddMvc();
            services.AddEntityFrameworkSqlServer();
            services.AddIdentity<ApplicationUser, IdentityRole>(config=> {
                config.Password.RequiredLength = 3;
                config.Password.RequireLowercase = false;
                config.Password.RequireUppercase = false;
                config.Password.RequiredUniqueChars = 0;
                config.Password.RequireNonAlphanumeric = false;
                
               
            }).AddEntityFrameworkStores<APIDbContext>()
            .AddDefaultTokenProviders();
            
            //通过依赖注入注入数据库上下文,调用AddDbContext来预配DI容器的DbContext类，默认情况下，该方法将服务生存期设置为Scoped,
            //表示上下文对象的生存期与web请求生存期一致，并在web请求结束时自动调用dispose方法
            services.AddDbContext<APIDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultDbName")));
            services.AddScoped<DbSeeder>();
            services.AddCors(options => options.AddPolicy("any",policy=>policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader()));
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer("Bearer", option =>
                 {
                     option.TokenValidationParameters = new TokenValidationParameters
                     {
                         IssuerSigningKey = TokenHelper.securityKey,
                         ValidateIssuerSigningKey = true,
                         ValidIssuer = TokenHelper.Issuer,
                         ValidateIssuer = true,
                         ValidateAudience = true
                     };
                 });




            services.AddSingleton<TokenHelper>();
         
            services.AddTransient<IDTOMapper, DTOMapper>();
        }
       
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, DbSeeder dbSeeder,UserManager<ApplicationUser> userManager, TokenHelper tokenHelper, APIDbContext db)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("any");
            //在当前路径中开启默认文件映射
            app.UseDefaultFiles();
            //启用给定选项的静态文件服务。
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = (context) => {
                    context.Context.Response.Headers["Cache-Control"] =
Configuration["StaticFiles:Headers:Cache-Control"];
                    context.Context.Response.Headers["Pragma"] =
                    Configuration["StaticFiles:Headers:Pragma"];
                    context.Context.Response.Headers["Expires"] =
                    Configuration["StaticFiles:Headers:Expires"];
                }
            });
            //将中间件添加到请求管道中
            app.UseJWTMiddleware();
            app.UseAuthentication();
            app.UseMvc();

            ////生成数据库执行此方法
            //try
            //{
            //    dbSeeder.SeedAsync().Wait();
            //}
            ////在应用程序执行期间响应一个或多个错误
            //catch (AggregateException e)
            //{
            //    throw new Exception(e.InnerException.ToString());
            //}
        }
    }
}
