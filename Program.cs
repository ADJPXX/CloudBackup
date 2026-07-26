using System.Diagnostics;
using System.Security.Principal;
using System.Text.Json;

namespace CloudBackup;

public static class Program
{
    private static Config? _config;
    
    private const string BackupDrive = @"D:\Backups\";
    
    private const string BackupCodigos = @"D:\Codigos\";
    
    private const string BackupDriveLetter = @"D:\";
    
    private const string DevDrive = @"E:\";
    
    private const string CloudBackup = @"G:\Meu Drive\BackupCloud\";
    
    private static readonly string TudoExiste = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "TUDO");
    
    public static async Task Main(string[] args)
    {
        if (!IsAdmin())
        {
            ElevarAdmin();
            return;
        }

        LerJson();
        
        var backupDrive = await FazerBackupAsync();
        
        Console.WriteLine(backupDrive);

        if (backupDrive.Contains("BACKUP FEITO"))
        {
            var backupNuvem = await FazerBackupNuvemAsync();
        
            Console.WriteLine(backupNuvem);
        }
        else
        {
            Console.WriteLine("Erro no backup local, backup na nuvem não foi realizado!");
        }
    }


    private static async Task<string> FazerBackupAsync()
    {
        try
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            
            var repositoriesPath = Path.Combine(DevDrive, "Repositories");
            
            var excludedFolders = string.Join(' ', _config!.ExcludedFolders.Select(folder => $"\"{folder}\""));
            
            foreach (var directory in Directory.GetDirectories(documents))
            {
                foreach (var dir in _config.BackupFolders)
                {
                    if (!Path.GetFileName(directory).Equals(dir, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var nomePasta = Path.GetFileName(directory);

                    var documentsBackup = Process.Start(new ProcessStartInfo
                    {
                        FileName = "robocopy",
                        Arguments =
                            $"\"{directory}\" \"{BackupDrive}{nomePasta}\" /E /COPY:DAT /XD {excludedFolders} /R:3 /W:5",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });

                    await documentsBackup?.WaitForExitAsync()!;

                    if (documentsBackup is { ExitCode: > 3 })
                    {
                        Console.WriteLine($"Erro ao copiar: {directory}");
                    }
                }
            }

            if (Directory.Exists(repositoriesPath))
            {
                foreach (var directory in Directory.GetDirectories(repositoriesPath))
                {
                    var nomePasta = Path.GetFileName(directory);

                    var repositories = Process.Start(new ProcessStartInfo
                    {
                        FileName = "robocopy",
                        Arguments = $"\"{directory}\" \"{BackupCodigos}{nomePasta}\" /E /COPY:DAT /R:3 /W:5",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });

                    await repositories?.WaitForExitAsync()!;

                    if (repositories is { ExitCode: > 3 })
                    {
                        Console.WriteLine($"Erro ao copiar repo: {directory}");
                    }
                }

                var publishOrigem = Path.Combine(DevDrive, "Repositories", "C#");

                var publishDestino = Path.Combine(BackupCodigos, "C#");

                var publishBackup = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{publishOrigem}\" \"{publishDestino}\" publish.txt /COPY:DAT /R:3 /W:5",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });

                await publishBackup?.WaitForExitAsync()!;
            }
            
            if (Directory.Exists(TudoExiste))
            {
                var tudoDestino = Path.Combine(BackupDriveLetter, "Backups", "TUDO");
                
                var downloadsBackup = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{TudoExiste}\" \"{tudoDestino}\" /E /COPY:DAT /R:3 /W:5",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
                
                await downloadsBackup?.WaitForExitAsync()!;
            }
            else
            {
                Console.WriteLine("NÃO CONTEM A PASTA \"TUDO\"");
            }

            return "BACKUP FEITO DE TODOS OS ARQUIVOS";
        }
        catch (Exception ex)
        {
            return $"ERRO: {ex.Message}";
        }
    }
    

    private static async Task<string> FazerBackupNuvemAsync()
    {
        try
        {
            var drivePronto = await EsperarDriveNuvem(@"G:\");

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

                    await backupNuvem?.WaitForExitAsync()!;
                }
            }

            return "BACKUP NA NUVEM CONCLUIDO";
        }

        catch (Exception ex)
        {
            return $"ERRO: {ex.Message}";
        }
    }

    
    private static async Task<bool> EsperarDriveNuvem(string drive, int tentativas = 10)
    {
        for (var i = 0; i < tentativas; i++)
        {
            if (Directory.Exists(drive))
            {
                return true;
            }

            await Task.Delay(5000);
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