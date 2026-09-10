namespace CloudBackup;

public static class Config
{
    public static ConfigDto Configs { get; private set; } = new();

    public static bool Initialized { get; set; } = false;

    public static void Initialize(ConfigDto configDto)
    {
        Configs = configDto;
        Initialized = true;
    }
}


public class ConfigDto
{
    public List<string> BackupFolders { get; init; } = [];

    public List<string> CloudBackupFolders { get; init; } = [];
    
    public string CloudBackupPath { get; init; } = string.Empty;

    public List<string> ExcludedFolders { get; init; } = [];
}