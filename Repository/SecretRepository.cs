using ExcelUploadApi.Data;
using Microsoft.EntityFrameworkCore;

public class SecretRepository
{
    private readonly ApplicationDbContext _context;

    public SecretRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetEncryptedDbPassword(string appUser)
    {
        var secret = await _context.AppSecrets
            .FirstOrDefaultAsync(a => a.AppUser == appUser);
        return secret?.EncryptedPassword;
    }

    public async Task SaveEncryptedDbPassword(string appUser, string encryptedPassword)
    {
        _context.AppSecrets.Add(new AppSecret
        {
            AppUser = appUser,
            EncryptedPassword = encryptedPassword
        });
        await _context.SaveChangesAsync();
    }
}