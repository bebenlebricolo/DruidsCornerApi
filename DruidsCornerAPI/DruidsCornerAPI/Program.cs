using DruidsCornerAPI.AuthenticationHandlers;
using DruidsCornerAPI.Models.DiyDog.RecipeDb;
using DruidsCornerAPI.Services;
using DruidsCornerAPI.Models.Config;
using DruidsCornerAPI.Tools;
using DruidsCornerAPI.Tools.Logging;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;

using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

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
using System.Reflection;
using DruidsCornerAPI.DatabaseHandlers;
using Google;
using DruidsCornerAPI.Models.Exceptions;

namespace DruidsCornerAPI
{
    /// <summary>
    /// Base program class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Custom OAuth2 authentication Scheme name
        /// Used when authenticating user through the passed token
        /// </summary>
        protected static readonly string OAuth2Scheme = "OAuth2";

        private static void AddOpenApi(IServiceCollection services)
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

                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            });
        }

        private static void SetAuthentication(WebApplicationBuilder builder)
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
                options.Audience = System.Environment.GetEnvironmentVariable("CLIENT_ID");
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

        /// <summary>
        /// Main entry poiint for app startup
        /// </summary>
        /// <param name="args"></param> 
        public static void Main(string[] args)
        {
            var webAppOptions = new WebApplicationOptions
            {
                Args = args
            };
            var builder = WebApplication.CreateBuilder(webAppOptions);
            
            // That's where we can add a new JsonConfig file if need be
            //builder.Configuration.AddJsonFile("appsettings.local.json");
            
            // We need custom converters because some types within the Recipe object are polymorphic (DataRecord, FileRecord, CloudRecord)
            // So we need custom converters in order to represent them nicely
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
            builder.Services.AddSingleton<SearchService>();

            var slidingPolicy = "sliding";
            var rateLimitingOptions = new WebRateLimits();
            rateLimitingOptions.FromConfig(builder.Configuration);
            builder.Services.AddRateLimiter(_ => _
                .AddSlidingWindowLimiter(policyName: slidingPolicy, options =>
                {
                    options.PermitLimit = rateLimitingOptions.PermitLimit;
                    options.Window = TimeSpan.FromSeconds(rateLimitingOptions.WindowSeconds);
                    options.SegmentsPerWindow = rateLimitingOptions.SegmentsPerWindow;
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = rateLimitingOptions.QueueLimit;
                }));

            var app = builder.Build();

            // This one should be provided at all times
            if(System.Environment.GetEnvironmentVariable("CLIENT_ID") == null)
            {
                throw new ConfigException("The CLIENT_ID environment variable was not set, won't be able to proceed operations.");
            }
            
            // Configure the HTTP request pipeline.
            app.UseRateLimiter();
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
            app.MapControllers().RequireRateLimiting(slidingPolicy);
            
            // Set the logger factory config to ApplicationLogging util
            ApplicationLogging.LoggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
            app.Run();
        }
    }
}