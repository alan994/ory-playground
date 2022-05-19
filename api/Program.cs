using System.Net;
using System.Runtime.CompilerServices;
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
    foreach (var header in httpContext.Request.Headers)
    {
        logger.LogInformation("Header arrived: {Key} - {@Value}", header.Key, header.Value);
    }

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

app.MapPost("/kratos-create", (HttpContext httpContext, [FromBody] UserCreatedEvnet payload, [FromServices] ILogger<Program> logger) =>
{
    logger.LogInformation("Payload from Ory Kratos registration webhook: {@Payload}", payload);

    httpContext.Response.StatusCode = 200;
    return;
});


app.MapPost("/hydrator", async (HttpContext httpContext, [FromBody] OryPayload payload, [FromServices] ILogger<Program> logger) =>
{
    var targetCompanyIdHeader = "X-Tl-Target-Company-Id";

    var targetCOmpanyIdStr = httpContext.Request.Headers.FirstOrDefault(x => x.Key == targetCompanyIdHeader).Value.ToString();

    logger.LogInformation("Payload from Ory: {@Payload}", payload);

    httpContext.Response.StatusCode = 200;

    if (payload.Header is null)
    {
        payload.Header = new Dictionary<string, object>();
    }

    payload.Header.Add("X-Tl-User-Id", new string[] { "alan994" });
    payload.Header.Add("X-Tl-Company-Id", new string[] { "aj-solutions", "talentlyft" });

    logger.LogInformation("Returning payload to Ory: {@Payload}", payload);

    return payload;
});
app.MapGet("/", () => "Hello");


app.MapGet("/check-cookie", async (HttpContext httpContext, [FromServices] ILogger<Program> logger) =>
{
    var cookie = httpContext.Request.Cookies["ory_kratos_session"];
    
    var baseAddress = new Uri("http://localhost:4433");
    var cookieContainer = new CookieContainer();
    using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
    using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
    {
        // var content = new FormUrlEncodedContent(new[]
        // {
        //     new KeyValuePair<string, string>("foo", "bar"),
        //     new KeyValuePair<string, string>("baz", "bazinga"),
        // });
        cookieContainer.Add(baseAddress, new Cookie("ory_kratos_session", cookie));
        var result = await client.GetAsync("/sessions/whoami");
        
        var response = await result.Content.ReadAsStringAsync();
        logger.LogInformation("Kratos response ({StatusCode}): {@Response}", result.StatusCode.ToString(), response);
    }
    
    logger.LogInformation("Test");
    return "Test";
});

app.Run();

public class OryPayload
{
    //public string Company { get; set; }
    //public string IdentityId { get; set; }
    public string Subject { get; set; }
    public Dictionary<string, object> Extra { get; set; }
    public Dictionary<string, object> Header { get; set; }
}

public class UserCreatedEvnet
{
    public string Company { get; set; }

    public string ExternalIdentityId { get; set; }

    public string CompanyUrl { get; set; }

    public string Phone { get; set; }

    public string Token { get; set; }

    public string FullName { get; set; }

    public string Email { get; set; }
    /*
     * company: ctx.identity.traits.company,
        externalIdentityId: ctx.identity.id,
        companyUrl: ctx.identity.traits.companyUrl,
        phone: ctx.identity.traits.phone,
        token: "tl-ory-secret-token-abe456cd-9ef2-4205-90a5-d0ad037d7a69",
        fullName: ctx.identity.traits.fullName
     */
}