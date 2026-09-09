namespace CloudBackup.Services;

public static class PathsService
{
    public const string CloudBackup = @"G:\Meu Drive\BackupCloud\";

    private const string DevDrive = @"E:\";
    
    public const string BackupDriveLetter = @"D:\";

    public const string BackupDrive = @"D:\Backups\";

    public const string BackupCodes = @"D:\Codigos\";

    private static readonly string ExeDirectory = AppDomain.CurrentDomain.BaseDirectory;
    
    public static readonly string JsonPath = Path.Combine(ExeDirectory, "CloudBackupConfig.json");

    public static readonly string LogPath = Path.Combine(ExeDirectory, "CloudBackup.log");
    
    public static readonly string ExcludedFolders = string.Join(' ', Config.Configs.ExcludedFolders.Select(folder => $"\"{folder}\""));
    
    public static readonly string Documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    public static readonly string CsInC = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "steamapps", "common", "Counter-Strike Global Offensive", "game", "csgo", "cfg");

    public static readonly string CsInD = Path.Combine(BackupDrive, "CS", "CFGS");

    public static readonly string TarkovInC = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Battlestate Games", "Escape from Tarkov", "Settings");

    public static readonly string TarkovInD = Path.Combine(BackupDrive, "Tarkov");
    
    public static readonly string CSharpDevDrive = Path.Combine(DevDrive, "Repositories", "C#");
    
    public static readonly string CSharpBackup = Path.Combine(BackupCodes, "C#");
    
    public static readonly string RepositoriesPath = Path.Combine(DevDrive, "Repositories");
    
    public static readonly string RocketLeagueSource = Path.Combine(Documents, "My Games", "Rocket League");
    
    public static readonly string RocketLeagueDestination = Path.Combine(BackupDrive, "My Games", "Rocket League");
    
    public static readonly string DotGithubSource = Path.Combine(CSharpDevDrive, ".github");

    public static readonly string DotGithubDestination = Path.Combine(CSharpBackup, ".github");
    
    public static readonly string DavinciSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Blackmagic Design");

    public static readonly string DavinciDestination = Path.Combine(BackupDrive, "DaVinci Resolve", "Blackmagic Design");
    
    public static readonly string ObsSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "obs-studio");

    public static readonly string ObsDestination = Path.Combine(BackupDrive, "obs-studio");
    
    public static readonly string DuckStationSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DuckStation");

    public static readonly string DuckStationDestination = Path.Combine(BackupDrive, "DuckStation");
}