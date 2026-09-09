using System.Text;

namespace CloudBackup.Services;

public static class CloudBackupService
{
    public static async Task<StringBuilder> MakeCloudBackupAsync()
    {
        var log = new StringBuilder();
        
        try
        {
            LogService.StartExecution("CLOUD BACKUP");
            
            if (!Directory.Exists(PathsService.CloudBackup))
            {
                var isDriveReady = await WaitCloudDrive(PathsService.CloudBackup);

                if (!isDriveReady)
                {
                    LogService.AddLog("DRIVE NÃO ENCONTRADO PARA BACKUP NA NUVEM. O BACKUP NA NUVEM NÃO FOI FEITO!");
                    
                    LogService.EndExecution();
                    
                    log.AppendLine("DRIVE NÃO ENCONTRADO PARA BACKUP NA NUVEM. O BACKUP NA NUVEM NÃO FOI FEITO!");

                    return log;
                }
            }
            
            foreach (var folderName in Config.Configs.CloudBackupFolders)
            {
                var directory = Path.Combine(PathsService.BackupDriveLetter, folderName);
                
                if (!Directory.Exists(directory))
                {
                    LogService.AddLog($"A PASTA \"{folderName}\" NÃO FOI ENCONTRADA NO CAMINHO: {directory}");
                    
                    log.AppendLine($"A PASTA \"{folderName}\" NÃO FOI ENCONTRADA NO CAMINHO: {directory}");
                    
                    continue;
                }
                
                var cloudStatus = await RobocopyService.CopyAsync($"\"{directory}\" \"{PathsService.CloudBackup}\\{folderName}\" /E /COPY:DAT /R:3 /W:5");
                
                LogService.AddLog($"{folderName} STATUS: {cloudStatus.Item1}");
                
                log.AppendLine($"{folderName} STATUS: {cloudStatus.Item1}");

                if (string.IsNullOrEmpty(cloudStatus.Item2))
                {
                    continue;
                }
                
                LogService.AddLog($"{folderName} ERRO: {cloudStatus.Item2}");
                
                log.AppendLine($"{folderName} ERRO: {cloudStatus.Item2}");
            }
            
            LogService.AddLog("BACKUP NA NUVEM CONCLUIDO");
            
            LogService.EndExecution();
            
            return log.AppendLine("BACKUP NA NUVEM CONCLUIDO");
        }
        catch (Exception ex)
        {
            return log.AppendLine($"ERRO NA FUNÇÃO \"MakeCloudBackupAsync()\": {ex.Message}");
        }
    }
    
    
    private static async Task<bool> WaitCloudDrive(string drive, int tentativas = 10)
    {
        for (var i = 0; i < tentativas; i++)
        {
            await Task.Delay(18000);
            
            if (Directory.Exists(drive))
            {
                return true;
            }
        }

        return false;
    }
}