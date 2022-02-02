using api;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

    //var cultureQuery = context.Request.Query["culture"];
    //if (!string.IsNullOrWhiteSpace(cultureQuery))
    //{
    //    var culture = new CultureInfo(cultureQuery);

    //    CultureInfo.CurrentCulture = culture;
    //    CultureInfo.CurrentUICulture = culture;
    //}



    // Call the next delegate/middleware in the pipeline.
    await next(context);
});


app.MapGet("/secure", (HttpContext httpContext, [FromServices] ILogger<Program> logger, [FromServices] IdentityInfo identityInfo) =>
    {
        return identityInfo;
    });

app.MapGet("/", () => "Hello");

app.Run();
