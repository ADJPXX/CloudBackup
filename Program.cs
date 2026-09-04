using System.Diagnostics;
using System.Security.Principal;
using System.Text;
using System.Text.Json;

namespace CloudBackup;

public static class Program
{
    private static Config? _config;
    
    private const string BackupDrive = @"D:\Backups\";
    
    private const string BackupCodes = @"D:\Codigos\";
    
    private const string BackupDriveLetter = @"D:\";
    
    private const string DevDrive = @"E:\";
    
    private const string CloudBackup = @"G:\Meu Drive\BackupCloud\";
    
    private static readonly string TudoExists = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "TUDO");
    
    public static async Task Main(string[] args)
    {
        if (!IsAdmin())
        {
            ElevarAdmin();
            return;
        }

        LerJson();
        
        var backupDrive = MakeDriveBackup();
        
        Console.WriteLine(backupDrive);
        
        if (backupDrive.ToString().Contains("BACKUP CONCLUIDO."))
        {
            var backupNuvem = MakeCloudBackup();
        
            Console.WriteLine(backupNuvem);
        }
        else
        {
            Console.WriteLine("Erro no backup local, backup na nuvem não foi realizado!");
        }
    }


    private static StringBuilder MakeDriveBackup()
    {
        var log = new StringBuilder();
        
        try
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var repositoriesPath = Path.Combine(DevDrive, "Repositories");

            var excludedFolders = string.Join(' ', _config.ExcludedFolders.Select(folder => $"\"{folder}\""));
            
            foreach (var directory in Directory.GetDirectories(documents))
            {
                foreach (var dir in _config.BackupFolders)
                {
                    if (!Path.GetFileName(directory).Equals(dir, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var folderName = Path.GetFileName(directory);

                    if (folderName.Contains("My Games"))
                    {
                        var rocketLeagueSource = Path.Combine(documents, "My Games", "Rocket League");

                        var rocketLeagueDestination = Path.Combine(BackupDrive, "My Games", "Rocket League");
                        
                        var rocketLeagueBackup = Process.Start(new ProcessStartInfo
                        {
                            FileName = "robocopy",
                            Arguments = $"\"{rocketLeagueSource}\" \"{rocketLeagueDestination}\" /E /COPY:DAT /XD {excludedFolders} /R:3 /W:5",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden
                        });
                        
                        rocketLeagueBackup?.WaitForExit();

                        continue;
                    }
                    
                    var documentsBackup = Process.Start(new ProcessStartInfo
                    {
                        FileName = "robocopy",
                        Arguments =
                            $"\"{directory}\" \"{BackupDrive}{folderName}\" /E /COPY:DAT /XD {excludedFolders} /R:3 /W:5",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });

                    documentsBackup?.WaitForExit();

                    if (documentsBackup is { ExitCode: > 3 })
                    {
                        log.AppendLine($"Erro ao copiar: {directory}");
                    }
                }
            }

            if (!Directory.Exists(repositoriesPath))
            {
                log.AppendLine($"A SEGUINTE PASTA NÃO FOI ENCONTRADA: {repositoriesPath}");
            }
            else
            {
                foreach (var directory in Directory.GetDirectories(repositoriesPath))
                {
                    var folderName = Path.GetFileName(directory);

                    var repositories = Process.Start(new ProcessStartInfo
                    {
                        FileName = "robocopy",
                        Arguments = $"\"{directory}\" \"{BackupCodes}{folderName}\" /E /COPY:DAT /R:3 /W:5",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });

                    repositories?.WaitForExit();

                    if (repositories is { ExitCode: > 3 })
                    {
                        log.AppendLine($"Erro ao copiar repo: {directory}");
                    }
                }

                var publishSource = Path.Combine(DevDrive, "Repositories", "C#");

                var publishDestination = Path.Combine(BackupCodes, "C#");

                var publishBackup = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{publishSource}\" \"{publishDestination}\" publish.txt /COPY:DAT /R:3 /W:5",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });

                publishBackup?.WaitForExit();

                var dotGithubSource = Path.Combine(DevDrive, "Repositories", "C#", ".github");

                var dotGithubDestination = Path.Combine(BackupCodes, "C#", ".github");

                var dotGithubBackup = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{dotGithubSource}\" \"{dotGithubDestination}\" /E /COPY:DAT /R:3 /W:5",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });

                dotGithubBackup?.WaitForExit();
            }

            var davinciSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Blackmagic Design");

            var davinciDestination = Path.Combine(BackupDrive, "DaVinci Resolve", "Blackmagic Design");

            if (Directory.Exists(davinciSource))
            {
                var davinciBackup = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{davinciSource}\" \"{davinciDestination}\" /E /COPY:DAT /XD {excludedFolders} /R:3 /W:5",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });

                davinciBackup?.WaitForExit();
            }
            else
            {
                log.AppendLine($"A SEGUINTE PASTA NÃO FOI ENCONTRADA: {davinciSource}");
            }

            var obsSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "obs-studio");

            var obsDestination = Path.Combine(BackupDrive, "obs-studio");

            if (Directory.Exists(obsSource))
            {
                var obsBackup = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{obsSource}\" \"{obsDestination}\" /E /COPY:DAT /XD {excludedFolders} /R:3 /W:5",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });

                obsBackup?.WaitForExit();
            }
            else
            {
                log.AppendLine($"A SEGUINTE PASTA NÃO FOI ENCONTRADA: {obsSource}");
            }

            var duckStationSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DuckStation");

            var duckStationDestination = Path.Combine(BackupDrive, "DuckStation");

            if (Directory.Exists(duckStationSource))
            {
                var duckStationBackup = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{duckStationSource}\" \"{duckStationDestination}\" /E /COPY:DAT /XD {excludedFolders} /R:3 /W:5",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });

                duckStationBackup?.WaitForExit();
            }
            else
            {
                log.AppendLine($"A SEGUINTE PASTA NÃO FOI ENCONTRADA: {duckStationSource}");
            }
            
            if (Directory.Exists(TudoExists))
            {
                var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "TUDO");

                var downloadsBackup = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{downloadsPath}\" \"{BackupDriveLetter}Backups\\TUDO\" /E /MOVE /R:3 /W:5",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });

                downloadsBackup?.WaitForExit();
            }
            else
            {
                log.AppendLine($"NÃO CONTEM A PASTA \"TUDO\" NO SEGUINTE CAMINHO: {TudoExists}");
            }
            
            return log.AppendLine("BACKUP CONCLUIDO.");
        }
        catch (Exception ex)
        {
            return log.AppendLine($"ERRO: {ex.Message}");
        }
    }
    

    private static string MakeCloudBackup()
    {
        try
        {
            var drivePronto = EsperarDriveNuvem(@"G:\");

            if (!drivePronto)
            {
                return "Drive não encontrado para backup na nuvem";
            }

            foreach (var directory in Directory.GetDirectories(BackupDriveLetter))
            {
                foreach (var dir in _config!.CloudBackupFolders)
                {
                    if (!Path.GetFileName(directory).Equals(dir, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var nomePasta = Path.GetFileName(directory);

                    var destino = Path.Combine(CloudBackup, nomePasta);

                    var backupNuvem = Process.Start(new ProcessStartInfo
                    {
                        FileName = "robocopy",
                        Arguments = $"\"{directory}\" \"{destino}\" /E /COPY:DAT /R:3 /W:5",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });

                    backupNuvem?.WaitForExitAsync();
                }
            }

            return "BACKUP NA NUVEM CONCLUIDO";
        }

        catch (Exception ex)
        {
            return $"ERRO: {ex.Message}";
        }
    }

    
    private static bool EsperarDriveNuvem(string drive, int tentativas = 10)
    {
        for (var i = 0; i < tentativas; i++)
        {
            if (Directory.Exists(drive))
            {
                return true;
            }

            Thread.Sleep(5000);
        }

        return false;
    }


    private static void LerJson()
    {
        try
        {
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CloudBackupConfig.json");

            var json = File.ReadAllText(jsonPath);

            _config = JsonSerializer.Deserialize<Config>(json);

            if (_config == null)
            {
                Console.WriteLine("Erro ao carregar as configurações.");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }


    private static bool IsAdmin()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }


    private static void ElevarAdmin()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = Process.GetCurrentProcess().MainModule!.FileName,
            UseShellExecute = true,
            Verb = "runas"
        };

        try
        {
            Process.Start(startInfo);
        }
        catch
        {
            Console.WriteLine("Permissão de administrador negada.");
        }
    }
}