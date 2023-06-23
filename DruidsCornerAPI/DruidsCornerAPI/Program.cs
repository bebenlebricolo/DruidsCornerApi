using DruidsCornerAPI.AuthenticationHandlers;
using DruidsCornerAPI.Models.DiyDog;
using DruidsCornerAPI.Services;
using DruidsCornerAPI.Tools;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace DruidsCornerAPI
{
    public class Program
    {
        public static readonly string OAuth2Scheme = "OAuth2";


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
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                }) ;

                options.AddSecurityDefinition(OAuth2Scheme, new OpenApiSecurityScheme
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
                    Scheme = OAuth2Scheme,
                    In = ParameterLocation.Header
                }) ;

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    // Dictionary<OpenApiSecurityScheme, IList<string>> 
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id=OAuth2Scheme
                            }
                        },
                        new string[]{}
                    }
                }                
                );
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

                // If authentication is required, and no cookie is present use Jwt decoding
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddCookie()
              .AddScheme<BasicAuthenticationOptions, GoogleOauth2AuthenticationHandler>(OAuth2Scheme, options =>
              {
                  //
              })
              //.AddGoogle(OAuth2Scheme, options =>
              //{
              //    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
              //    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
              //})
              .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Audience = builder.Configuration["Authentication:Google:ClientId"];
                options.Authority = "https://accounts.google.com";
                options.ClaimsIssuer = "https://accounts.google.com";
                //options.TokenValidationParameters = new TokenValidationParameters
                //{
                //    RequireAudience = true,
                //    RequireExpirationTime = true,
                //};
            });
              //.AddScheme<BasicAuthenticationOptions, GoogleOauth2AuthenticationHandler>("Custom", null);
        }



        public static void Main(string[] args)
        {
            var webAppOptions = new WebApplicationOptions
            {
                Args = args
            };
            var builder = WebApplication.CreateBuilder(webAppOptions);
            
            builder.Services.AddControllers().AddJsonOptions(options =>
             {
                 options.JsonSerializerOptions.WriteIndented = true;
                 options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                 options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                 options.JsonSerializerOptions.Converters.Add(new DataRecordPolymorphicConverter());
             });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            AddOpenApi(builder.Services);

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
                        .WithOrigins(new string[] {"https://localhost"});
                });
            });

            SetAuthentication(builder);
            
            // Nice questions : https://stackoverflow.com/questions/72966528/can-api-key-and-jwt-token-be-used-in-the-same-net-6-webapi
            builder.Services.AddAuthorization(options =>
            {
                var jwtPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
                options.FallbackPolicy = jwtPolicy;
                options.DefaultPolicy = jwtPolicy;

                var googleOauthPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(OAuth2Scheme)
                    .RequireAuthenticatedUser()
                    .Build();

                // Custom policy to handle google cloud related stuff
                options.AddPolicy(OAuth2Scheme, googleOauthPolicy);
            });

            builder.Logging.AddConsole();

            // Registering available services
            builder.Services.AddSingleton<RecipeService>();


            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            app.UseCors("AllowAll");
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.OAuthAppName("Swagger client");
                options.OAuthClientId(builder.Configuration["Authentication:Google:ClientId"]);
            });

            app.UseHttpsRedirection();
            app.UseHttpLogging();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}