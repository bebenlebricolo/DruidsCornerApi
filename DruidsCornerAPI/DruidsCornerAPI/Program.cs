using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

namespace DruidsCornerAPI
{
    public class Program
    {

        public static void AddOpenApi(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Druid's Corner API",
                    Description = "Web service for Druid's Corner application stacks. Provides various services around recipes such as viewing, editing, converting, querying, etc."
                });
                //options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    In = ParameterLocation.Header,
                //    Description = "Please enter a valid token",
                //    Name = "Authorization",
                //    Type = SecuritySchemeType.Http,
                //    BearerFormat = "JWT",
                //    Scheme = "Bearer"
                //});

                options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
                {
                    Description = "Please enter a valid token",
                    Name = "OAuth2 authorization",
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/auth"),
                            TokenUrl = new Uri("https://oauth2.googleapis.com/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "https://www.googleapis.com/auth/cloud-platform", "Cloud platform access (required to fetch GCP resources)" },
                                { "https://www.googleapis.com/auth/userinfo.email", "User email (required for JWT) and Cloud Run functionalities" },
                                { "https://www.googleapis.com/auth/userinfo.profile", "User profile (required for JWT) and Cloud Run functionalities" },
                                { "openid", "OpenID scope" }
                            }
                        }
                    },
                    Scheme = "OAuth2",
                }) ;

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    // Dictionary<OpenApiSecurityScheme, IList<string>> 
                    //{
                    //    new OpenApiSecurityScheme
                    //    {
                    //        Reference = new OpenApiReference
                    //        {
                    //            Type=ReferenceType.SecurityScheme,
                    //            Id="Bearer"
                    //        }
                    //    },
                    //    new string[]{}
                    //},
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="OAuth2"
                            }
                        },
                        new string[]{}
                    }
                });
            });
        }

        public static void SetAuthentication(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(options =>
            {
                // If an authentication cookie is present, use it to get authentication information
                // from https://developer.okta.com/blog/2019/07/12/secure-your-aspnet-core-app-with-oauth
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                // If authentication is required, and no cookie is present, use Okta (configured below) to sign in
                options.DefaultChallengeScheme = "OAuth2";

                //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                //     .AddJwtBearer(options =>
                // {
                //     options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                //     {
                //         ValidateIssuer = true,
                //         ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                //         ValidateAudience = true,
                //         ValidAudience = builder.Configuration["JwtSettings:Audience"],
                //         ValidateLifetime = true,
                //         ClockSkew = TimeSpan.Zero,
                //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!))
                //     };
                // })
                .AddCookie()
                .AddGoogle("OAuth2", options =>
            {
                options.AccessType = "offline";
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            }); ;
        }


        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            AddOpenApi(builder.Services);
            // builder.Services.AddSwaggerGenNewtonsoftSupport();

            builder.Services.AddCors(o =>
            {
                o.AddPolicy("AllowAll", a => a.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
                o.AddPolicy("AllowMore", configure =>
                {
                    configure
                        .WithHeaders(
                           //Microsoft minimum set recommended 
                           "Accept", "Content-Type", "Origin",
                           //Swagger headers
                           "api_key", "authorization", "x-requested-with",
                           "Access-Control-Allow-Origin")
                        .WithOrigins(new string[] {"https://oauth2.googleapis.com"});
                });
            });

            //builder.Services.AddIdentityCore<IdentityUser>()
            //    .AddRoles<IdentityRole>()
            //    .AddEntityFrameworkStores<CarListDbContext>();
            SetAuthentication(builder);
            
            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.OAuthAppName("Swagger client");
                options.OAuthClientId(builder.Configuration["Authentication:Google:ClientId"]);
            });

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}