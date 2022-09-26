
using HotelListing.API.Core.Abstract;
using HotelListing.API.Core.Configuration;
using HotelListing.API.Core.Middleware;
using HotelListing.API.Core.Repository;
using HotelListing.API.Data;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");

#region Services
builder.Services.AddDbContext<HotelListingDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Hotel Listing API", Version = "v1" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer schema.
                        Enter 'Bearer' [space] and then your token in the text input below.
                        Example: 'Bearer 234kd34fn3sdkf234'",
        Name="Authorization",
        In=ParameterLocation.Header,
        Type=SecuritySchemeType.ApiKey,
        Scheme= JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference=new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id=JwtBearerDefaults.AuthenticationScheme
                },
                Scheme="0auth2",
                Name=JwtBearerDefaults.AuthenticationScheme,
                In=ParameterLocation.Header
            },
            new List<string>()
        }

    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",x=>x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
});
//cachleme
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024;//1mb
    options.UseCaseSensitivePaths=true;
});
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
//AutoMapper.Extensions.Microsoft.DependencyInjection paketini eklmek gerek
builder.Services.AddAutoMapper(typeof(MapperConfig));
//versiyonlama
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("X-Version"),
        new MediaTypeApiVersionReader("ver")

        );
});
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddIdentityCore<ApiUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<ApiUser>>("HotelListingApi")
    .AddEntityFrameworkStores<HotelListingDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //gizli keyimizi kullansýnmý
        ValidateIssuerSigningKey = true,
        ValidateIssuer=true,
        ValidateAudience=true,
        ValidateLifetime=true,
        ClockSkew=TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
    };
});

#endregion

#region Host
//appsetting json ayarlar yapýldý dosyaya seq urline ve console a yazdýrýldý
builder.Host.UseSerilog((context, loggerconfiguration) => loggerconfiguration.WriteTo.Console().ReadFrom.Configuration(context.Configuration));
builder.Services.AddControllers().AddOData(options =>
{
    options.Select().Filter().OrderBy();
});
#endregion

#region App
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
//cache middleware
app.UseResponseCaching();
app.Use(async (context, next) =>
{
    context.Response.GetTypedHeaders().CacheControl =
    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
    {
        Public = true,
        MaxAge = TimeSpan.FromSeconds(10)
    };
    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = new string[] { "Accept-Encoding" };
    await next();
});
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
#endregion