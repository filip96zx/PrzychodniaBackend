
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Przychodnia.Interfaces;
using Przychodnia.Models;
using Przychodnia.Npgsql;
using Przychodnia.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Przychodnia.Transfer.Email;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.HttpOverrides;

namespace Przychodnia
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
                options.AddPolicy("Default",
                    builder => {
                        #if DEBUG
                        builder.WithOrigins("https://localhost:3000", "http://localhost:3000");
#endif
#if !DEBUG
                        builder.WithOrigins("https://http://vps-63e72677.vps.ovh.net", "http://http://vps-63e72677.vps.ovh.net");
#endif
                        //    builder.AllowAnyOrigin();
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                        builder.AllowCredentials();
                    });
            });

            services.AddDbContext<DatabaseContext>(options =>
#if DEBUG
                options.UseNpgsql(Configuration.GetConnectionString("DbConnection")
#endif
#if !DEBUG
                options.UseNpgsql(Configuration.GetConnectionString("DbConnection2")
#endif
                ));



            //services.AddIdentityCore<User>(cfg =>
            //{
            //    cfg.User.RequireUniqueEmail = true;
            //}).AddRoles<Role>().AddSignInManager().AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();

            services.Configure<EmailSetting>(Configuration.GetSection("Email"));


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                            .GetBytes(Configuration["Jwt:Key"]))
                    };
                });
            services.AddIdentity<User, Role>(options =>
            {
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            })
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders();


            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IVisitService, VisitService>();
            services.AddSingleton<IEmailSender, EmailSender>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //});

            app.UseRouting();

            app.UseCors("Default");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthorization();
            app.UseAuthentication();
            //app.UseHttpsRedirection();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            //app.UseMvc();
        }
    }
}
