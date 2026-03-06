using DotNetEnv;
using ExcelUploadApi.Data;
using ExcelUploadApi.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load .env for encryption key
Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
string encryptionKey = Environment.GetEnvironmentVariable("ENV_ENCRYPTION_KEY")
                       ?? throw new Exception("ENV_ENCRYPTION_KEY not found in .env");

// Load config files
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile("appsettings.ExcelUser.json", optional: false, reloadOnChange: true);

// OpenAPI / Controllers
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

// DI Repositories
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<SecretRepository>();

// Add hosted service to seed encrypted password if missing
builder.Services.AddHostedService<DbPasswordSeeder>();

// Retrieve the connection string from appsettings
var connectionString = builder.Configuration.GetConnectionString("ExcelUserConnection")
                       ?? throw new Exception("ExcelUserConnection not found");

builder.Services.AddDbContext<BootstrapDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("BootstrapConnection")
    )
);

// NOTE: We will replace the password at runtime after decrypting from DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.CommandTimeout(30); // Optional: Command timeout in case it's a large query
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();