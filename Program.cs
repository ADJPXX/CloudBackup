using System.Diagnostics;
using System.Security.Principal;
using System.Text.Json;

namespace CloudBackup;

public static class Program
{
    private static bool IsAdmin()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
    
    
    private static Config? _config;
    
    private const string BackupDriveLetter = @"D:\";
    
    private const string CloudBackup = @"G:\Meu Drive\BackupCloud\";
    
    public static async Task Main(string[] args)
    {
        if (!IsAdmin())
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

            return;
        }
        
        try
        {
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CloudBackupConfig.json");

            var json = File.ReadAllText(jsonPath);

            _config = JsonSerializer.Deserialize<Config>(json);

            if (_config == null)
            {
                Console.WriteLine("Erro ao carregar as configurações.");
                return;
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
        var resultado = await FazerBackupNuvem();
        
        Console.WriteLine(resultado);
    }

    private static async Task<string> FazerBackupNuvem()
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
}