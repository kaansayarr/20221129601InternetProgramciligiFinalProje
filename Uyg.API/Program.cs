using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Uyg.API.Models;
using Uyg.API.Repositories;
using Uyg.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("sqlCon"));
});

// Add Identity
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireUppercase = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
    options.Lockout.MaxFailedAccessAttempts = 3;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.FromMinutes(500)
    };
});

// Add Authorization
builder.Services.AddAuthorization();

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OguzKaanErdogduAPI",
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Add Repositories and Services
builder.Services.AddScoped(typeof(GenericRepository<>));
builder.Services.AddScoped<LessonsRepository>();
builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddScoped<CourseRepository>();
builder.Services.AddScoped<StudentRepository>();
builder.Services.AddScoped<EnrollmentService>();

// Add AutoMapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// Add CORS
builder.Services.AddCors(p => p.AddPolicy("corspolicy", opt =>
{
    opt.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("corspolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Create admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        // Önce mevcut admin kullanıcısını silelim
        var existingAdmin = await userManager.FindByNameAsync("admin");
        if (existingAdmin != null)
        {
            await userManager.DeleteAsync(existingAdmin);
            logger.LogInformation("Existing admin user deleted.");
        }

        // Admin rolünü oluştur
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new AppRole { Name = "Admin" });
            logger.LogInformation("Admin role created.");
        }

        // Yeni admin kullanıcısı oluştur
        var adminUser = new AppUser
        {
            UserName = "admin",
            Email = "admin@example.com",
            FullName = "System Admin",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            PhotoUrl = "profil.jpg" // UserController'da beklenen varsayılan değer
        };

        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
            logger.LogInformation("Admin user created and role assigned successfully.");
            
            // Kullanıcı bilgilerini logla
            var createdUser = await userManager.FindByNameAsync("admin");
            if (createdUser != null)
            {
                logger.LogInformation($"Admin user created with ID: {createdUser.Id}");
                var roles = await userManager.GetRolesAsync(createdUser);
                logger.LogInformation($"Admin user roles: {string.Join(", ", roles)}");
            }
        }
        else
        {
            logger.LogError("Failed to create admin user: {0}", string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating admin user and roles.");
    }
}

app.Run();
