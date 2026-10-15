using System.Text;
using System.Text.Json.Serialization;
using AssetSystem.Assets.Api;
using AssetSystem.Assets.Application;
using AssetSystem.Assets.Infrastructure;
using AssetSystem.BuildingBlocks.Infrastructure.Files;
using AssetSystem.BuildingBlocks.Infrastructure.Persistence;
using AssetSystem.BuildingBlocks.Web;
using AssetSystem.Consignments.Api;
using AssetSystem.Consignments.Application;
using AssetSystem.Consignments.Infrastructure;
using AssetSystem.Host;
using AssetSystem.Identity.Api;
using AssetSystem.Identity.Application;
using AssetSystem.Identity.Infrastructure;
using AssetSystem.Identity.Infrastructure.Security;
using AssetSystem.Organization.Api;
using AssetSystem.Organization.Application;
using AssetSystem.Organization.Infrastructure;
using AssetSystem.SharedKernel.Abstractions;
using FluentValidation;
using JasperFx.CodeGeneration.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.FluentValidation;
using Wolverine.Postgresql;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var connectionString = config.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("ConnectionStrings:Postgres is not configured.");

// ---- Cross-cutting services -------------------------------------------------------------------
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();
builder.Services.AddSingleton<AuditingInterceptor>();
builder.Services.Configure<FileStoreOptions>(config.GetSection(FileStoreOptions.Section));
builder.Services.AddSingleton<IFileStore, LocalFileStore>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks().AddCheck<PostgresHealthCheck>("postgres", tags: ["ready"]);
builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    o.KnownIPNetworks.Clear(); // behind the Docker network's reverse proxy
    o.KnownProxies.Clear();
});

// ---- Authentication / authorization -----------------------------------------------------------
builder.Services.AddOptions<JwtOptions>().Bind(config.GetSection(JwtOptions.Section))
    .Validate(o => o.SigningKey.Length >= 32, "Jwt:SigningKey must be at least 32 characters.")
    .ValidateOnStart();

var jwt = config.GetSection(JwtOptions.Section).Get<JwtOptions>() ?? new JwtOptions();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false; // keep short claim names: sub, name, role, company_id
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey.PadRight(32))),
            NameClaimType = "name",
            RoleClaimType = "role",
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAppAuthorization();
builder.Services.AddAppRateLimiting();

// ---- Modules ----------------------------------------------------------------------------------
builder.Services
    .AddIdentityApplication().AddIdentityInfrastructure(connectionString)
    .AddOrganizationApplication().AddOrganizationInfrastructure(connectionString)
    .AddAssetsApplication().AddAssetsInfrastructure(connectionString)
    .AddConsignmentsApplication().AddConsignmentsInfrastructure(connectionString);

builder.Services.AddHostedService<ModuleInitializerService>();

// ---- Wolverine: in-process mediator + durable messaging between modules -----------------------
builder.Host.UseWolverine(opts =>
{
    // Handlers live in each module's Application project.
    opts.Discovery.IncludeAssembly(typeof(IdentityApplicationModule).Assembly);
    opts.Discovery.IncludeAssembly(typeof(OrganizationApplicationModule).Assembly);
    opts.Discovery.IncludeAssembly(typeof(AssetsApplicationModule).Assembly);
    opts.Discovery.IncludeAssembly(typeof(ConsignmentsApplicationModule).Assembly);

    // Messages and domain events are stored in the same PostgreSQL database, in the same
    // transaction as the data change (transactional outbox), and processed from durable local queues.
    opts.PersistMessagesWithPostgresql(connectionString, "wolverine");
    opts.UseEntityFrameworkCoreTransactions();
    opts.Policies.UseDurableLocalQueues();

    opts.UseFluentValidation();

    // ASP.NET Core Identity's UserManager is wired through IServiceProvider, which Wolverine cannot inline
    // into generated code. Everything else is resolved without service location.
    opts.ServiceLocationPolicy = ServiceLocationPolicy.AllowedButWarn;
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapHealthChecks("/health/live", new() { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new() { Predicate = r => r.Tags.Contains("ready") });

if (app.Environment.IsDevelopment() || config.GetValue<bool>("Api:ExposeDocs"))
{
    app.MapOpenApi();
    app.MapScalarApiReference("/docs");
}

// Every module contributes its endpoints (one class per feature slice).
app.MapEndpoints(IdentityApiModule.Assembly)
   .MapEndpoints(OrganizationApiModule.Assembly)
   .MapEndpoints(AssetsApiModule.Assembly)
   .MapEndpoints(ConsignmentsApiModule.Assembly);

app.Run();

/// <summary>Entry point marker for integration tests.</summary>
public partial class Program;
