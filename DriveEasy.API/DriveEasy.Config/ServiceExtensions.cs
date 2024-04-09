using DriveEasy.API.DriveEasy.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

namespace DriveEasy.API.DriveEasy.Config
{
    public  static class ServiceExtensions
    {

        /* JWT Config */
        public static void ConfigureJwt(this IServiceCollection services, IConfiguration config)
        {
            var jwtSettings = config.GetSection("JWT");
            var key = jwtSettings.GetSection("SKEY").Value;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateActor = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.GetSection("Issuer").Value,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents()
                    {
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";

                            var forbiddenError = new ViewApiResponse
                            {
                                ResponseStatus = 403,
                                ResponseMessage = $"Forbidden",
                                ResponseData = "You are not authorized to access this resource"
                            };

                            return context.Response.WriteAsync(JsonSerializer.Serialize(forbiddenError));
                        },

                        OnChallenge = context =>
                        {
                            context.HandleResponse();

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            /* Ensure we always have an error and error description. */
                            if (string.IsNullOrEmpty(context.Error))
                                context.Error = "Unauthorized";

                            if (string.IsNullOrEmpty(context.ErrorDescription))
                                context.ErrorDescription = "This request requires a valid JWT access token to be provided";

                            /* Add some extra context for expired JWT Tokens. */
                            if (context.AuthenticateFailure != null && context.AuthenticateFailure.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                var authenticationException = context.AuthenticateFailure as SecurityTokenExpiredException;
                                context.Error = "Token Expired";
                                context.Response.Headers.Append("x-token-expired", authenticationException.Expires.ToString("o"));
                                context.ErrorDescription = $"The token expired on {authenticationException.Expires:o}";
                            }

                            var error = new ViewErrorResponse
                            {
                                ResponseStatus = StatusCodes.Status401Unauthorized,
                                ResponseMessage = context.Error,
                                ResponseData = context.ErrorDescription
                            };

                            return context.Response.WriteAsync(JsonSerializer.Serialize(error));
                        }
                    };
                });
        }
    }
}
