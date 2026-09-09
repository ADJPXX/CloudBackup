using System.Text;

namespace CloudBackup.Services;

public static class DriveBackupService
{
    public static async Task<StringBuilder> MakeDriveBackupAsync()
    {
        var log = new StringBuilder();
        
        try
        {
            LogService.StartExecution("BACKUP");
            
            await DocumentsBackupAsync(log);

            await RepositoriesBackupAsync(log);

            await DaVinciBackupAsync(log);

            await ObsBackupAsync(log);

            await DuckStationBackupAsync(log);

            await CsBackupAsync(log);

            await TarkovBackupAsync(log);

            log.AppendLine("BACKUP CONCLUIDO");
            
            LogService.AddLog("BACKUP CONCLUIDO");
            
            LogService.EndExecution();
            
            return log;
        }
        catch (Exception ex)
        {
            return log.AppendLine($"ERRO NA FUNÇÃO \"MakeDriveBackupAsync()\": {ex.Message}");
        }
    }


    private static async Task DocumentsBackupAsync(StringBuilder log)
    {
        try
        {
            foreach (var folderName in Config.Configs.BackupFolders)
            {
                var directory = Path.Combine(PathsService.Documents, folderName);

                if (!Directory.Exists(directory))
                {
                    LogService.AddLog($"A PASTA \"{folderName}\" NÃO FOI ENCONTRADA NO CAMINHO \"{PathsService.Documents}\"");
                    
                    log.AppendLine($"A PASTA \"{folderName}\" NÃO FOI ENCONTRADA NO CAMINHO \"{PathsService.Documents}\"");

                    continue;
                }

                if (folderName.Equals("My Games", StringComparison.OrdinalIgnoreCase))
                {
                    var rocketLeagueStatus = await RobocopyService.CopyAsync($"\"{PathsService.RocketLeagueSource}\" \"{PathsService.RocketLeagueDestination}\" /E /COPY:DAT /XD {PathsService.ExcludedFolders} /R:3 /W:5");

                    LogService.AddLog($"Rocket League STATUS: {rocketLeagueStatus.Item1}");
                    
                    log.AppendLine($"Rocket League STATUS: {rocketLeagueStatus.Item1}");

                    if (!string.IsNullOrEmpty(rocketLeagueStatus.Item2))
                    {
                        LogService.AddLog($"Rocket League ERRO: {rocketLeagueStatus.Item2}");
                        
                        log.AppendLine($"Rocket League ERRO: {rocketLeagueStatus.Item2}");
                    }
                    
                    continue;
                }

                var backupFoldersStatus = await RobocopyService.CopyAsync($"\"{directory}\" \"{PathsService.BackupDrive}{folderName}\" /E /COPY:DAT /XD {PathsService.ExcludedFolders} /R:3 /W:5");
                
                LogService.AddLog($"{folderName} STATUS: {backupFoldersStatus.Item1}");
                
                log.AppendLine($"{folderName} STATUS: {backupFoldersStatus.Item1}");

                if (string.IsNullOrEmpty(backupFoldersStatus.Item2))
                {
                    continue;
                }
                
                LogService.AddLog($"{folderName} ERRO: {backupFoldersStatus.Item2}");
                
                log.AppendLine($"{folderName} ERRO: {backupFoldersStatus.Item2}");
            }
        }
        catch (Exception ex)
        {
            log.AppendLine($"ERRO NA FUNÇÃO \"DocumentsBackupAsync()\": {ex.Message}");
        }
    }


    private static async Task RepositoriesBackupAsync(StringBuilder log)
    {
        try
        {
            if (!Directory.Exists(PathsService.RepositoriesPath))
            {
                LogService.AddLog($"A PASTA \"Repositories\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.RepositoriesPath}");
                
                log.AppendLine($"A PASTA \"Repositories\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.RepositoriesPath}");
            }
            else
            {
                foreach (var directory in Directory.GetDirectories(PathsService.RepositoriesPath))
                {
                    var folderName = Path.GetFileName(directory);

                    var repositoriesStatus = await RobocopyService.CopyAsync($"\"{directory}\" \"{PathsService.BackupCodes}{folderName}\" /E /COPY:DAT /R:3 /W:5");
                    
                    LogService.AddLog($"{folderName} STATUS: {repositoriesStatus.Item1}");
                    
                    log.AppendLine($"{folderName} STATUS: {repositoriesStatus.Item1}");

                    if (string.IsNullOrEmpty(repositoriesStatus.Item2))
                    {
                        continue;
                    }
                    
                    LogService.AddLog($"{folderName} ERRO: {repositoriesStatus.Item2}");
                        
                    log.AppendLine($"{folderName} ERRO: {repositoriesStatus.Item2}");
                }

                if (Directory.Exists(PathsService.DotGithubSource))
                {
                    var dotGitStatus = await RobocopyService.CopyAsync($"\"{PathsService.DotGithubSource}\" \"{PathsService.DotGithubDestination}\" /E /COPY:DAT /R:3 /W:5");
                    
                    LogService.AddLog($".github STATUS: {dotGitStatus.Item1}");
                    
                    log.AppendLine($".github STATUS: {dotGitStatus.Item1}");

                    if (!string.IsNullOrEmpty(dotGitStatus.Item2))
                    {
                        LogService.AddLog($".github ERRO: {dotGitStatus.Item2}");
                    
                        log.AppendLine($".github ERRO: {dotGitStatus.Item2}");
                    }
                }
                else
                {
                    LogService.AddLog($"A PASTA \".github\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.DotGithubSource}");
                    
                    log.AppendLine($"A PASTA \".github\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.DotGithubSource}");
                }

                if (File.Exists(Path.Combine(PathsService.CSharpDevDrive, ".gitignore")))
                {
                    var gitignoreStatus = await RobocopyService.CopyAsync($"\"{PathsService.CSharpDevDrive}\" \"{PathsService.CSharpBackup}\" .gitignore /COPY:DAT /R:3 /W:5");
                    
                    LogService.AddLog($".gitignore STATUS: {gitignoreStatus.Item1}");
                    
                    log.AppendLine($".gitignore STATUS: {gitignoreStatus.Item1}");

                    if (!string.IsNullOrEmpty(gitignoreStatus.Item2))
                    {
                        LogService.AddLog($".gitignore ERRO: {gitignoreStatus.Item2}");
                    
                        log.AppendLine($".gitignore ERRO: {gitignoreStatus.Item2}");
                    }
                }
                else
                {
                    LogService.AddLog($"O ARQUIVO \".gitignore\" NÃO FOI ENCONTRADO NO CAMINHO: {PathsService.CSharpDevDrive}");
                    
                    log.AppendLine($"O ARQUIVO \".gitignore\" NÃO FOI ENCONTRADO NO CAMINHO: {PathsService.CSharpDevDrive}");
                }
            }
        }
        catch (Exception ex)
        {
            log.AppendLine($"ERRO NA FUNÇÃO \"RepositoriesBackupAsync()\": {ex.Message}");
        }
    }


    private static async Task DaVinciBackupAsync(StringBuilder log)
    {
        try
        {
            if (Directory.Exists(PathsService.DavinciSource))
            {
                var daVinciStatus = await RobocopyService.CopyAsync($"\"{PathsService.DavinciSource}\" \"{PathsService.DavinciDestination}\" /E /COPY:DAT /XD {PathsService.ExcludedFolders} /R:3 /W:5");
                
                LogService.AddLog($"DaVinci Resolve STATUS: {daVinciStatus.Item1}");
                    
                log.AppendLine($"DaVinci Resolve STATUS: {daVinciStatus.Item1}");

                if (!string.IsNullOrEmpty(daVinciStatus.Item2))
                {
                    LogService.AddLog($"Blackmagic Design ERRO: {daVinciStatus.Item2}");
                    
                    log.AppendLine($"Blackmagic Design ERRO: {daVinciStatus.Item2}");
                }
            }
            else
            {
                LogService.AddLog($"A PASTA \"Blackmagic Design\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.DavinciSource}");
                
                log.AppendLine($"A PASTA \"Blackmagic Design\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.DavinciSource}");
            }
        }
        catch (Exception ex)
        {
            log.AppendLine($"ERRO NA FUNÇÃO \"DaVinciBackupAsync()\": {ex.Message}");
        }
    }


    private static async Task ObsBackupAsync(StringBuilder log)
    {
        try
        { 
            if (Directory.Exists(PathsService.ObsSource))
            {
                var obsStatus = await RobocopyService.CopyAsync($"\"{PathsService.ObsSource}\" \"{PathsService.ObsDestination}\" /E /COPY:DAT /XD {PathsService.ExcludedFolders} /R:3 /W:5");
                
                LogService.AddLog($"obs-studio STATUS: {obsStatus.Item1}");
                    
                log.AppendLine($"obs-studio STATUS: {obsStatus.Item1}");

                if (!string.IsNullOrEmpty(obsStatus.Item2))
                {
                    LogService.AddLog($"obs-studio ERRO: {obsStatus.Item2}");
                    
                    log.AppendLine($"obs-studio ERRO: {obsStatus.Item2}");
                }
            }
            else
            {
                LogService.AddLog($"A PASTA \"obs-studio\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.ObsSource}");
                
                log.AppendLine($"A PASTA \"obs-studio\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.ObsSource}");
            }
        }
        catch(Exception ex)
        {
            log.AppendLine($"ERRO NA FUNÇÃO \"ObsBackupAsync()\": {ex.Message}");
        }
    }


    private static async Task DuckStationBackupAsync(StringBuilder log)
    {
        try
        {
            if (Directory.Exists(PathsService.DuckStationSource))
            {
                var duckStationStatus = await RobocopyService.CopyAsync($"\"{PathsService.DuckStationSource}\" \"{PathsService.DuckStationDestination}\" /E /COPY:DAT /XD {PathsService.ExcludedFolders} /R:3 /W:5");
                
                LogService.AddLog($"DuckStation STATUS: {duckStationStatus.Item1}");
                    
                log.AppendLine($"DuckStation STATUS: {duckStationStatus.Item1}");

                if (!string.IsNullOrEmpty(duckStationStatus.Item2))
                {
                    LogService.AddLog($"DuckStation ERRO: {duckStationStatus.Item2}");
                    
                    log.AppendLine($"DuckStation ERRO: {duckStationStatus.Item2}");
                }
            }
            else
            {
                LogService.AddLog($"A PASTA \"DuckStation\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.DuckStationSource}");
                
                log.AppendLine($"A PASTA \"DuckStation\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.DuckStationSource}");
            }
        }
        catch (Exception ex)
        {
            log.AppendLine($"ERRO NA FUNÇÃO \"DuckStationBackupAsync()\": {ex.Message}");
        }
    }


    private static async Task CsBackupAsync(StringBuilder log)
    {
        try
        {
            if (!Directory.Exists(PathsService.CsInC))
            {
                LogService.AddLog($"A PASTA \"cfg\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.CsInC}");
                
                log.AppendLine($"A PASTA \"cfg\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.CsInC}");

                return;
            }
            
            var csCfgStatus = await RobocopyService.CopyAsync($"\"{PathsService.CsInC}\" \"{PathsService.CsInD}\" *.cfg /COPY:DAT /R:3 /W:5");
            
            LogService.AddLog($"Cs cfg STATUS: {csCfgStatus.Item1}");
                    
            log.AppendLine($"Cs cfg STATUS: {csCfgStatus.Item1}");
            
            if (!string.IsNullOrEmpty(csCfgStatus.Item2))
            {
                LogService.AddLog($"Cs cfg ERRO: {csCfgStatus.Item2}");
                    
                log.AppendLine($"Cs cfg ERRO: {csCfgStatus.Item2}");
            }
        }
        catch (Exception ex)
        {
            log.AppendLine($"ERRO NA FUNÇÃO \"CSBackupAsync()\": {ex.Message}");
        }
    }


    private static async Task TarkovBackupAsync(StringBuilder log)
    {
        try
        {
            if (!Directory.Exists(PathsService.TarkovInC))
            {
                LogService.AddLog($"A PASTA \"Settings\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.TarkovInC}");
                
                log.AppendLine($"A PASTA \"Settings\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.TarkovInC}");

                return;
            }
            
            var tarkovStatus = await RobocopyService.CopyAsync($"\"{PathsService.TarkovInC}\" \"{PathsService.TarkovInD}\" *.ini /COPY:DAT /R:3 /W:5");
            
            LogService.AddLog($"Tarkov STATUS: {tarkovStatus.Item1}");
                    
            log.AppendLine($"Tarkov STATUS: {tarkovStatus.Item1}");
            
            if (!string.IsNullOrEmpty(tarkovStatus.Item2))
            {
                LogService.AddLog($"Tarkov ERRO: {tarkovStatus.Item2}");
                    
                log.AppendLine($"Tarkov ERRO: {tarkovStatus.Item2}");
            }
        }
        catch (Exception ex)
        {
            log.AppendLine($"ERRO NA FUNÇÃO \"TarkovBackupAsync()\": {ex.Message}");
        }
    }
}