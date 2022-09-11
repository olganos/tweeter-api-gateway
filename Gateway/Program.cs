using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = Environment.GetEnvironmentVariable("IDENTITY_SERVER_URI")
            ?? builder.Configuration.GetValue<string>("IdentityServer:Uri");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "tweeter-api");
    });
});

// todo: uncomment and tst if I decide to call the endpoints directly via react app
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("corsPolicy", options =>
//    {
//        options.WithOrigins(builder.Configuration.GetValue<string>("AllowedOrigins:Uris"));
//    });
//});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseRouting();

//app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("", ctx => ctx.Response.WriteAsync("YAG"));
    endpoints.MapReverseProxy();
});

app.Run();
