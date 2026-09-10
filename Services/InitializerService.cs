using System.Diagnostics;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace CloudBackup.Services;

public static class InitializerService
{
    public static void ReadJson()
    {
        try
        {
            if (!File.Exists(PathsService.JsonPath))
            {
                CreateDefaultSettings(PathsService.JsonPath);
            }

            var json = File.ReadAllText(PathsService.JsonPath);

            var config = JsonSerializer.Deserialize<ConfigDto>(json);

            if (config == null)
            {
                LogService.StartExecution("LENDO JSON");
                
                LogService.AddLog("ARQUIVO .json INVÁLIDO");
                
                LogService.EndExecution();
                
                throw new Exception("ARQUIVO .json INVÁLIDO");
            }

            Config.Initialize(config);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    
    private static void CreateDefaultSettings(string jsonPath)
    {
        try
        {
            var configs = new ConfigDto
            {
                
                BackupFolders =
                [
                    "AC Content Manager",
                    "Assetto Corsa",
                    "Assetto Corsa Competizione",
                    "Automobilista 2",
                    "iRacing",
                    "My Games",
                    "PCSX2"
                ],


                CloudBackupFolders =
                [
                    "Backups",
                    "Book do globis",
                    "Codigos",
                    "Contratos apartamentos",
                    "Fotos Steam",
                    "Instaladores",
                    "Jogos e emuladores",
                    "SCRIPTS",
                    "Vídeos",
                    "Wallpapers"
                ],
                
                
                CloudBackupPath = @"G:\Meu Drive\BackupCloud\",


                ExcludedFolders =
                [
                    "log",
                    "cache",
                    "replay",
                    "logs",
                    "caches",
                    "replays"
                ]
            };

            var jsonWrite = JsonSerializer.Serialize(configs, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            File.WriteAllText(jsonPath, jsonWrite);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO NA FUNÇÃO \"CreateDefaultSettings()\": {ex.Message}");
        }
    }
    
    
    public static void CheckLogFile()
    {
        if (!File.Exists(PathsService.LogPath))
        {
            File.Create(PathsService.LogPath).Dispose();
        }
    }
    
    
    public static bool IsAdmin()
    {
        try
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO NA FUNÇÃO \"IsAdmin()\": {ex.Message}");
            return false;
        }
    }


    public static void ElevateToAdmin()
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO NA FUNÇÃO \"ElevateToAdmin()\": {ex.Message}");
        }
    }
}