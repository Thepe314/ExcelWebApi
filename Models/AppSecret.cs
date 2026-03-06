namespace ExcelUploadApi.Data
{
    public class AppSecret
    {
        public int Id { get; set; }
        public string AppUser { get; set; } = null!;
        public string EncryptedPassword { get; set; } = null!;
    }
}