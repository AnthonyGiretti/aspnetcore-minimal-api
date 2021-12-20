// Configure services
using Microsoft.AspNetCore.Http;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = "https://login.microsoftonline.com/xxxxxxxxxxxxxxxxxxxxxxxxxx";
    options.Audience = "xxxxxxxxxxxxxxxxxxxxxxxxx";
    options.TokenValidationParameters.ValidateLifetime = false;
    options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
});

builder.Services.AddAuthorization();

builder.Services.AddCors();

builder.Services.AddScoped<IHelloService, HelloService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,

            },
            new List<string>()
        }});
});

// Configure and enable middlewares
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));

app.UseCors(p =>
{
    p.AllowAnyOrigin();
    p.WithMethods("GET");
    p.AllowAnyHeader();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/Hello", ([FromQuery] bool? isHappy, IHelloService service, ClaimsPrincipal user) =>
{
    if (isHappy is null)
        return Results.BadRequest("Please tell if you are happy or not :-)");

    return Results.Ok(service.Hello(user, (bool)isHappy));
})
.Produces<string>((int)HttpStatusCode.OK) // Response description
.Produces((int)HttpStatusCode.BadRequest) // Response description
.WithName("Hello 1") // Response Id
.WithTags("Hellos"); // group description

app.MapGet("/Hello2", () =>
{
    return Results.Ok("Hello again !");
})
.Produces<string>((int)HttpStatusCode.OK) // Response description
.Produces((int)HttpStatusCode.BadRequest) // Response description
.WithName("Hello 2") // Response Id
.WithTags("Hellos"); // group description


// Run the app
app.Run();