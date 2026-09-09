using CloudBackup.Services;

namespace CloudBackup;

public static class Program
{
    public static async Task Main(string[] args)
    {
        if (!InitializerService.IsAdmin())
        {
            InitializerService.ElevateToAdmin();
            return;
        }
        
        InitializerService.CheckLogFile();

        InitializerService.ReadJson();
        
        var backupDrive = await DriveBackupService.MakeDriveBackupAsync();
        
        Console.WriteLine(backupDrive);
        
        if (backupDrive.ToString().Contains("BACKUP CONCLUIDO."))
        {
            var backupNuvem = await CloudBackupService.MakeCloudBackupAsync();
        
            Console.WriteLine(backupNuvem);
        }
        else
        {
            Console.WriteLine("Erro no backup local, backup na nuvem não foi realizado!");
        }
    }
}