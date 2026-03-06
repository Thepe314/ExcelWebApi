using ExcelUploadApi.Data;
using Microsoft.EntityFrameworkCore;
public class DbPasswordSeeder : IHostedService
{
    private readonly IServiceProvider _provider;
    private readonly IConfiguration _configuration;

    public DbPasswordSeeder(IServiceProvider provider, IConfiguration configuration)
    {
        _provider = provider;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        string encryptionKey = Environment.GetEnvironmentVariable("ENV_ENCRYPTION_KEY")
            ?? throw new Exception("ENV_ENCRYPTION_KEY not found in .env");

        using var scope = _provider.CreateScope();

        // ✅ Use BootstrapDbContext (Windows Auth) — not ApplicationDbContext
        var bootstrapDb = scope.ServiceProvider.GetRequiredService<BootstrapDbContext>();

        // Check if password exists
        var existing = await bootstrapDb.AppSecrets
            .FirstOrDefaultAsync(a => a.AppUser == "ExcelUser", cancellationToken);

        if (existing == null)
        {
            string plainDbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD")
                ?? throw new Exception("DB_PASSWORD not found in .env");

            string encrypted = EncryptionHelper.Encrypt(plainDbPassword, encryptionKey);

            bootstrapDb.AppSecrets.Add(new AppSecret
            {
                AppUser = "ExcelUser",
                EncryptedPassword = encrypted
            });

            await bootstrapDb.SaveChangesAsync(cancellationToken);
            Console.WriteLine("✅ Encrypted DB password inserted into AppSecrets!");

            existing = await bootstrapDb.AppSecrets
                .FirstOrDefaultAsync(a => a.AppUser == "ExcelUser", cancellationToken);
        }

        // Decrypt and inject into ExcelUserConnection
        string dbPassword = EncryptionHelper.Decrypt(existing!.EncryptedPassword, encryptionKey);

        var current = _configuration["ConnectionStrings:ExcelUserConnection"]!;
        _configuration["ConnectionStrings:ExcelUserConnection"] =
            current.Replace("Password=PLACEHOLDER", $"Password={dbPassword}");

        Console.WriteLine("✅ Connection string updated with decrypted password.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}