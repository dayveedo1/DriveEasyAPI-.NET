using DriveEasy.API;
using DriveEasy.API.DriveEasy.Config;
using DriveEasy.API.DriveEasy.Interface;
using DriveEasy.API.DriveEasy.Models;
using DriveEasy.API.DriveEasy.Repo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

// Add services to the container.


builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(swagger =>
{
    swagger.EnableAnnotations();

    swagger.SwaggerDoc(name: "v1", new OpenApiInfo
    {
        Title = "DriveEasyAPI",
        Version = "v1",
        Description = "DriveEasy API",
        Contact = new OpenApiContact
        {
            Name = "DriveEasy API",
            //Email = "Input Email"
            //Url = new Uri("Input URL")
        },

        License = new OpenApiLicense
        {
            Name = "DriveEasy License",
            //Url = new Uri("Input URL")
        }
    });
    
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter Token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });

   
});


/* Database Config */
builder.Services.AddDbContextPool<DriveEasyApiDbContext>((serviceProvider, options) =>
{
    options.UseSqlServer(config.GetConnectionString("DriveEasyAPI"),
            sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 10,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);

                sqlOptions.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds);
            });
    options.UseInternalServiceProvider(serviceProvider);

});

/* CORS Config */
string? AllowOrigins = builder.Configuration["Cors"];
builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowOrigins,
               builder =>
               {
                   builder.WithOrigins(AllowOrigins)
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .SetIsOriginAllowed(origin => true)  /* allow any origin */
                       .AllowCredentials();
                });
});


/* Identity Config */
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<DriveEasyApiDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddEntityFrameworkSqlServer();


/* DI Mapper for Interfaces & their Implementations */
builder.Services.AddTransient<IAuth, AuthImpl>();
builder.Services.AddTransient<ICar, CarImpl>();


/* Global Exception Handler Middleware | reference: GlobalExceptionHandler class */
builder.Services.AddTransient<GlobalExceptionHandler>();


/* Authentication */
builder.Services.AddAuthentication();

/* JWT Configuration | reference: ServiceExtensions class */
builder.Services.ConfigureJwt(config);



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(AllowOrigins);  /* CORS */

app.UseAuthentication();   /* Authentication */

app.UseAuthorization();   /* Authorization */

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionHandler>();  /* Global Exception Handler Middleware */

app.MapControllers();

app.Run();
