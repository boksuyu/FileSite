namespace FileSite.Data.ViewModels;

public class AccountRecoveryVM
{
    public string? NewPassword { get; set; }
    public string? Link { get; set; }
    public string? Email { get; set; }
    
    public long? Date { get; set; }
}