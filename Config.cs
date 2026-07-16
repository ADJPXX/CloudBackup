namespace CloudBackup;

public class Config
{
    public List<string> BackupFolders { get; set; } = [];
    public List<string> CloudBackupFolders { get; set; } = [];
    public List<string> ExcludedFolders { get; set; } = [];
}