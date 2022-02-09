using System.Net;
using api;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Ory.Hydra.Client.Api;
using Ory.Hydra.Client.Client;
using Ory.Hydra.Client.Model;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

builder.Services.AddScoped<IdentityInfo>();

var app = builder.Build();

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
        grantTypes: new List<string>(){ "authorization_code", "refresh_token" },
        responseTypes: new List<string>() { "code" },
        scope: "openid,offline"
    ));
    return HttpStatusCode.OK;
});

app.MapGet("/", () => "Hello");


app.Run();
