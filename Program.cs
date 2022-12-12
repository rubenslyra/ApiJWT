using System.Security.Claims;
using System.Text;
using ApiJWT;
using ApiJWT.Models;
using ApiJWT.Repositories;
using ApiJWT.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var key = Encoding.ASCII.GetBytes(Settings.Secret); // <-- Here is the key used to sign the token and validate the token
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.RequireHttpsMetadata = false;
    opt.SaveToken = true;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("Admin", policy => policy.RequireRole("manager"));
    opt.AddPolicy("Employee", policy => policy.RequireRole("employee"));

    opt.AddPolicy("Bearer", new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
        .RequireAuthenticatedUser().Build());

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseHttpsRedirection();

// Importante lembrar que o UseAuthentication deve ser chamado antes do UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

// Endpoints

app.MapPost("/login", (User model) =>
{
    var user = UserRepository.Get(model.Username, model.Password);
    if (user == null)
        return Results.NotFound(new { message = "Usuário ou senha inválidos" });

    var token = TokenService.GenerateToken(user);

    user.Password = "";

    return Results.Ok(new
    {
        user = user,
        token = token
    });
});


app.MapGet("/anonymous", () => "Bemvinde a API JWT. Para acessar os dados, faça login e utilize o token gerado para acessar os dados.").AllowAnonymous();



app.MapGet("/authenticated", (ClaimsPrincipal user) =>
{
    Results.Ok(new { message = $"Olá, {user.Identity.Name}" });
}).RequireAuthorization();


app.Run();


/**
1. Para gerar o token, utilize o endpoint /login
2. Para acessar os dados, utilize o endpoint /api/usuario   (não esqueça de passar o token gerado no header da requisição)

Bemvinde a API JWT. Para acessar os dados, faça login e utilize o token gerado para acessar os dados. Para acessar os dados, utilize o endpoint /api/usuario

Olá, você está acessando os dados do usuário.

*/