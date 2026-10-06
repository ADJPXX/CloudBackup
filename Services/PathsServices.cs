namespace CloudBackup.Services;

public static class PathsService
{
    private const string DevDrive = @"E:\";
    
    public const string BackupDriveLetter = @"D:\";

    public const string BackupDrive = @"D:\Backups\";

    public const string BackupCodes = @"D:\Codigos\";

    private static readonly string ExeDirectory = AppDomain.CurrentDomain.BaseDirectory;
    
    public static readonly string JsonPath = Path.Combine(ExeDirectory, "CloudBackupConfig.json");

    public static readonly string LogPath = Path.Combine(ExeDirectory, "CloudBackupLog.log");
    
    public static string ExcludedFolders
    {
        get
        {
            var result = string.Join(' ', Config.Configs.ExcludedFolders.Select(folder => $"\"{folder}\""));

            return result;
        }
    }
    
    public static readonly string Documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    public static readonly string ScriptsFolderInC = Path.Combine(Documents, "SCRIPTS");

    public static readonly string ScriptsFolderInD = Path.Combine(BackupDriveLetter, "SCRIPTS");

    public static readonly string CsInC = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "steamapps", "common", "Counter-Strike Global Offensive", "game", "csgo", "cfg");

    public static readonly string CsInD = Path.Combine(BackupDrive, "CS", "CFGS");

    public static readonly string LmuInC = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "steamapps", "common", "Le Mans Ultimate", "UserData");

    public static readonly string LmuInD = Path.Combine(BackupDrive, "LMU", "UserData");

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

    public static readonly string TudoInDownloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "TUDO");

    public static readonly string TudoCopied = Path.Combine(BackupDrive, "TUDO");
    
    public static readonly string ObsSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "obs-studio");

    public static readonly string ObsDestination = Path.Combine(BackupDrive, "obs-studio");
    
    public static readonly string DuckStationSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DuckStation");

    public static readonly string DuckStationDestination = Path.Combine(BackupDrive, "DuckStation");

    public static readonly string AfterburnerInC = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "MSI Afterburner", "Profiles");

    public static readonly string AfterburnerInD = Path.Combine(BackupDrive, "Afterburner", "Profiles");

    public static readonly string RivaProfilesInC = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "RivaTuner Statistics Server", "Profiles");

    public static readonly string RivaProfilesInD = Path.Combine(BackupDrive, "Riva", "Profiles");

    public static readonly string RivaPluginsInC = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "RivaTuner Statistics Server", "Plugins");

    public static readonly string RivaPluginsInD = Path.Combine(BackupDrive, "Riva", "Plugins");
}