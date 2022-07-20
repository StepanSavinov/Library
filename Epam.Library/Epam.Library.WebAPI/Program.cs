using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using AutoMapper;
using Epam.Library.BLL.Interfaces;
using Epam.Library.DependencyConfig;
using Epam.Library.Entities;
using Epam.Library.WebAPI.Helpers;
using Epam.Library.WebAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateActor = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("User", builderClaims =>
    {
        builderClaims.RequireAuthenticatedUser();
        builderClaims.RequireAssertion(a =>
            a.User.HasClaim(ClaimTypes.Role, "Admin") ||
            a.User.HasClaim(ClaimTypes.Role, "Librarian") ||
            a.User.HasClaim(ClaimTypes.Role, "User"));
    });
    options.AddPolicy("Librarian", builderClaims =>
    {
        builderClaims.RequireAuthenticatedUser();
        builderClaims.RequireAssertion(a =>
            a.User.HasClaim(ClaimTypes.Role, "Librarian") ||
            a.User.HasClaim(ClaimTypes.Role, "Admin"));
    });
    options.AddPolicy("Admin", builderClaims =>
    {
        builderClaims.RequireAuthenticatedUser();
        builderClaims.RequireClaim(ClaimTypes.Role, "Admin");
    });
});

builder.Services.AddEndpointsApiExplorer();

Config.RegisterServices(builder.Services);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// builder.Services.AddCors(options =>
// {
//     options.AddDefaultPolicy(policyBuilder =>
//     {
//         policyBuilder.AllowAnyHeader();
//         policyBuilder.AllowAnyMethod();
//         policyBuilder.AllowAnyOrigin();
//     });
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/login",
        Login)
    .Accepts<UserLogin>("application/json")
    .Produces<string>();

app.MapPost("/register",
        Register)
    .Accepts<UserLogin>("application/json")
    .Produces<User>();

app.MapPost("/update-user",
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        (int id, UserLogin user, IUserLogic service) => UpdateUser(id, user, service))
    .Produces<User>();

app.MapPost("/delete-user",
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        (int id, IUserLogic service) => DeleteUser(id, service))
    .Produces<bool>();
            

app.MapGet("/get-all-users",
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        (IUserLogic service) => GetAllUsers(service))
    .Produces<List<User>>();

app.MapGet("/get-all-library",
        GetAllLibrary)
    .Produces<List<Polygraphy>>();

// methods

IResult GetAllUsers(IUserLogic service)
{
    var users = service.GetAllUsers();
    
    return Results.Ok(users);
}

IResult Login(UserLogin user, IUserLogic service)
{
    if (!string.IsNullOrEmpty(user.Username) &&
        !string.IsNullOrEmpty(user.Password))
    {
        var loggedInUser = new User(user.Username.ToLower(), 
            GetHashedPassword(user.Password), 
            service.GetUserByUsername(user.Username).Role);
        
        if (loggedInUser is null) return Results.NotFound("User not found");

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, loggedInUser.Username),
            new Claim(ClaimTypes.Role, loggedInUser.Role)
        };

        var token = new JwtSecurityToken
        (
            issuer: builder.Configuration["Jwt:Issuer"],
            audience: builder.Configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(60),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                SecurityAlgorithms.HmacSha256)
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Results.Ok(tokenString);
    }
    return Results.BadRequest("Invalid user credentials");
}

IResult DeleteUser(int id, IUserLogic service)
{
    if (service.RemoveUser(id, out var errors))
    {
        return Results.Ok();
    }
    
    return Results.BadRequest(errors.Select(e => e.Message));
}

IResult Register(UserLogin user, IUserLogic service)
{
    if (service.Register(new User(user.Username.ToLower(), GetHashedPassword(user.Password)), out var errors))
    {
        return Results.Ok();
    }

    return Results.BadRequest(errors.Select(e => e.Message));
}

IResult UpdateUser(int id, UserLogin user, IUserLogic service)
{
    var fetchedUser = service.GetUserById(id);
    if (fetchedUser is null)
    {
        return Results.BadRequest("User not found");
    }

    fetchedUser.Username = user.Username.ToLower();
    fetchedUser.Password = GetHashedPassword(user.Password);
    fetchedUser.Role = user.Role;

    if (service.UpdateUser(fetchedUser, out var errors))
    {
        return Results.Ok();
    }
    
    return Results.BadRequest(errors.Select(e => e.Message));
}

IResult GetAllLibrary(IMapper mapper, IAuthorLogic authorLogic, ILibraryLogic libraryLogic)
{
    var polygraphies = libraryLogic.GetAllLibrary();
    var mappedPolygraphies = CommonMethods.MapPolygraphyList(mapper, libraryLogic, authorLogic, polygraphies);
    
    return Results.Ok(mappedPolygraphies);
}

app.Run();

string GetHashedPassword(string password)
{
    using var sha = SHA512.Create();
    var sb = new StringBuilder();
    foreach (var item in sha.ComputeHash(Encoding.Unicode.GetBytes(password)))
    {
        sb.Append(item.ToString("X2"));
    }

    return sb.ToString();
}
