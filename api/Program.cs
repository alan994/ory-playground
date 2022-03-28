using System.Net;
using System.Text.Json;
using api;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Ory.Hydra.Client.Api;
using Ory.Hydra.Client.Client;
using Ory.Hydra.Client.Model;
using Serilog;
using Serilog.Enrichers.Span;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("X-UserId");
    logging.RequestHeaders.Add("X-CompanyName");
    logging.RequestHeaders.Add("x-tl-user-id");
    logging.RequestHeaders.Add("x-tl-company-id");
});
builder.Services.AddScoped<IdentityInfo>();

builder.Host.UseSerilog((HostBuilderContext hosting, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    
    loggerConfiguration.Enrich.WithSpan();
    
    loggerConfiguration.WriteTo.Console();
});


var app = builder.Build();

app.UseHttpLogging();

app.Use(async (context, next) =>
{
    var identityInfo = context.RequestServices.GetRequiredService<IdentityInfo>();
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    if (identityInfo is null)
    {
        logger.LogError("PROBLEMI!!!");
    }

    var userId = context.Request.Headers["X-UserId"].FirstOrDefault();
    var companyName = context.Request.Headers["X-CompanyName"].FirstOrDefault();

    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(companyName))
    {

        logger.LogError("KORISNIK NIJE AUTHENTICIRAN!!!");
    }

    identityInfo.UserId = userId;
    identityInfo.CompanyName = companyName;
    await next(context);
});


app.MapGet("/secure", (HttpContext httpContext, [FromServices] ILogger<Program> logger, [FromServices] IdentityInfo identityInfo) =>
    {
        return identityInfo;
    });

app.MapGet("/hydra/create", (HttpContext httpContext) =>
{
    IAdminApi adminApi = new AdminApi("http://localhost:4445");

    adminApi.CreateOAuth2Client(new HydraOAuth2Client(
        clientId: "alan",
        clientName: "alan client",
        clientSecret: "alanory",
        grantTypes: new List<string>() { "authorization_code", "refresh_token" },
        responseTypes: new List<string>() { "code" },
        scope: "openid,offline"
    ));
    return HttpStatusCode.OK;
});
app.MapPost("/ory", async (HttpContext httpContext, [FromBody] OryPayload payload, [FromServices] ILogger<Program> logger) =>
{
    logger.LogInformation("Payload from Ory: {@Payload}", payload);

    
    
    httpContext.Response.StatusCode = 200;

    if(payload.Extra is null)
    {
        payload.Extra = new Dictionary<string, object>();
    }

    if (payload.Header is null)
    {
        payload.Header = new Dictionary<string, object>();
    }

    //payload.Extra.Add("x-tl-user-id", "alan994");
    //payload.Extra.Add("x-tl-company-id", "aj-solutions talentlyft");

    payload.Header.Add("x-tl-user-id", new string[] { "alan994" });
    payload.Header.Add("x-tl-company-id", new string[] { "aj-solutions", "talentlyft" });
    
    return payload;
});
app.MapGet("/", () => "Hello");


app.Run();

public class OryPayload
{
    //public string Company { get; set; }
    //public string IdentityId { get; set; }
    public string Subject { get; set; }
    public Dictionary<string, object> Extra { get; set; }
    public Dictionary<string, object> Header { get; set; }
}
